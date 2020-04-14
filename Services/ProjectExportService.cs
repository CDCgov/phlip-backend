using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using esquire;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Esquire.Services
{
    /// <summary>
    /// This class runs as a transient service and handles request for exports of project data in numeric and textual
    /// formats. It also handles export a codebook which defines all the possible values that could be exported in a
    /// data export. For more information on exports check out the documentation on IIU Confluence Esquire page.
    /// </summary>
    public class ProjectExportService : IProjectExportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        private ILogger<ProjectExportService> _logger;

        private CsvWriter _csvWriter;
        private long _projectId;
        private Project _project;
        private ProjectJurisdiction _projectJurisdiction;
        private List<SchemeQuestionWithChildQuestionsDto> _schemeTree;
        private ICollection<SchemeAnswer> _schemeCategories;
        private SchemeAnswer _schemeCategory;
        private ICollection<CodedAnswer> _codedCategories;
        private bool _codedTextExport;
        private bool _codebookExport;
        private bool _processingCategoryQuestions;
        private string _outlineNumber;
        private ExportResult _result;
        private long _userId;
        private User _codedUser;
        private bool _validated;

        private const string QuestionHeaderPrefix = "q";
        private const string AnswerHeaderPrefix = "_a";
        private const string CategoryHeaderPrefix = "_t";
        private const string CategoryNotSelectedAnswer = ".";

        public ProjectExportService(IUnitOfWork unitOfWork, IWebHostEnvironment environment, ILogger<ProjectExportService> logger)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
            _logger = logger;
            _csvWriter = null;
            _projectId = 0;
            _project = null;
            _projectJurisdiction = null;
            _schemeTree = null;
            _codedTextExport = false;
            _schemeCategories = null;
            _codedCategories = null;
            _schemeCategory = null;
            _outlineNumber = "";
            _processingCategoryQuestions = false;
            _userId = -1;
            _codedUser = null;
            _validated = true;

        }

        /// <summary>
        /// The entry point of the export service.
        /// </summary>
        /// <param name="projectId">The project ID of the project to be exported.</param>
        /// <param name="type">The type of export desired.</param>
        /// <param name="userId">optional userid of the coder data to be exported</param>
        /// <returns>An ExportResult object</returns>
        public async Task<ExportResult> ExportProjectData(long projectId, string type, long userId = -1)
        {
            _result = new ExportResult();
            _projectId = projectId;
            _userId = userId;
            _validated = _userId == -1;
            _project = await _unitOfWork.Projects.GetProjectWithScheme(p => p.Id == projectId);
            if (!_validated)
            {
                _codedUser = await _unitOfWork.Users.SingleAsync(u => u.Id == _userId);
            }
            if (_project == null)
                return new ExportResult(ExportResult.ProjectNotFound);
            _schemeTree = _unitOfWork.Schemes.GetSchemeQuestionsAsTree(_project.Scheme);
            if (_schemeTree == null || _schemeTree.Count == 0)
                return new ExportResult(ExportResult.SchemeNotFound);
            if (type.Equals("text"))
                _codedTextExport = true;
            else if (type.Equals("codebook"))
                _codebookExport = true;
            // make sure we have a valid filename and replaces spaces with dashes
            var projectName = _validated ? _project.Name: _project.Name+"-"+_codedUser.GetFullName() ;
            projectName = projectName.Replace(" ", "-");
            if (projectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                projectName = "project-" + _project.Id.ToString();
            }

            // create paths for saving file on server
            var filePath = "";
            var projDataDirInfo = GetProjectExportDirectoryInfo(projectName);

            // check for codebook export
            if (_codebookExport)
            {
                filePath = Path.Combine(projDataDirInfo.FullName, projectName + "-codebook-export.csv");
                var result = WriteCodebookFile(filePath);
            }
            else
            {
                // check for a text export, otherwise do numeric 
                if (_codedTextExport)
                    filePath = Path.Combine(projDataDirInfo.FullName, projectName + "-text-export.csv");
                else
                    filePath = Path.Combine(projDataDirInfo.FullName, projectName + "-numeric-export.csv");
                var result = await WriteExportFile(filePath);   
            }
            
            return _result;
            
        }

        // Export the project codebook, which servers as a guide to how all answer are coded
        // in numeric and text based exports 
        public async Task<ExportResult> ExportProjectCodebook(long projectId)
        {
            
            _result = new ExportResult();
            _projectId = projectId;
            _project = await _unitOfWork.Projects.GetProjectWithScheme(p => p.Id == projectId);
            if (_project == null)
                return new ExportResult(ExportResult.ProjectNotFound);
                
            _schemeTree = _unitOfWork.Schemes.GetSchemeQuestionsAsTree(_project.Scheme);
            if (_schemeTree == null || _schemeTree.Count == 0)
                return new ExportResult(ExportResult.SchemeNotFound);

            // make sure we have a valid filename and replaces spaces with dashes
            var projectName = _project.Name;
            projectName = projectName.Replace(" ", "-");
            if (projectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                projectName = "project-" + _project.Id.ToString();
            }

            // do file stuff
            var projDataDirInfo = GetProjectExportDirectoryInfo(projectName);
            var filePath = Path.Combine(projDataDirInfo.FullName, projectName + "-stat-export.csv");
            var result = WriteCodebookFile(filePath);
            
            return _result;
            
        }


        private DirectoryInfo GetProjectExportDirectoryInfo(string projectName)
        {
            var projectDataDir = Path.Combine(_environment.WebRootPath, "exports", "projects", projectName);
            var projectDataDirectoryInfo = new DirectoryInfo(projectDataDir);

            if (!projectDataDirectoryInfo.Exists)
            {
                // try to create the directory for the project data exports
                projectDataDirectoryInfo.Create();
            }

            return projectDataDirectoryInfo;
        }

        #region Headers
        /// <summary>
        /// The methods in this region handle the headers of both types of data exports, numeric and textual.
        /// </summary>

        
        private string QuestionHeaderString()
        {
            return QuestionHeaderPrefix + _outlineNumber;
        }

        /// <summary>
        /// Writes CSV header row entry
        /// </summary>
        /// <param name="currentQuestionHeader"></param>
        private void WriteHeader(string currentQuestionHeader)
        {
            var header = currentQuestionHeader;

            _csvWriter.WriteField(header);

            if (_codedTextExport)
            {
                _csvWriter.WriteField(header + " pincite");
            }

        }

        /// <summary>
        /// Writes the comment header for text exports
        /// </summary>
        private void WriteCommentHeader()
        {
            if (_codedTextExport)
            {
                _csvWriter.WriteField(QuestionHeaderString() + " comment");
            }

        }
        /// <summary>
        /// Writes the annotation header for text exports
        /// </summary>
        private void WriteAnnotationHeader(string header)
        {
            if (_codedTextExport)
            {
                    _csvWriter.WriteField(header + " annotation"); 
            }

        }

        /// <summary>
        /// Given a binary scheme question, write the header
        /// </summary>
        /// <param name="schemeQuestion"></param>
        private void WriteBinaryQuestionHeader(SchemeQuestionWithChildQuestionsDto schemeQuestion)
        {
            var header = "";
            if (_schemeCategory == null)
                header = QuestionHeaderString();
            else
                header = QuestionHeaderString() + CategoryHeaderPrefix + _schemeCategory.Order;

            WriteHeader(header);
            WriteAnnotationHeader(header);
            if (schemeQuestion.IncludeComment)
            {
                WriteCommentHeader();
            }
        }

        /// <summary>
        /// Given a category scheme question, write the header
        /// </summary>
        /// <param name="schemeQuestion"></param>
        private int WriteCategoryQuestionHeader(SchemeQuestionWithChildQuestionsDto schemeQuestion)
        {
            var categoryCount = schemeQuestion.PossibleAnswers.Count;

            for (var index = 1; index <= categoryCount; index++)
            {
                var header = "";
                try
                {
                    var answer = schemeQuestion.PossibleAnswers.Single(pa => pa.Order == index);
                    header = QuestionHeaderString() + CategoryHeaderPrefix + answer.Order;
                }
                catch (InvalidOperationException)
                {
                    _result.Code = ExportResult.CategoryHeaderInvalidOperation;
                    return _result.Code;
                }
                catch (ArgumentNullException)
                {
                    _result.Code = ExportResult.CategoryHeaderArgumentNull;
                    return _result.Code;                    
                }

                WriteHeader(header);
                WriteAnnotationHeader(header);
            }

            if (schemeQuestion.IncludeComment)
            {
                WriteCommentHeader();
            }

            return ExportResult.NoError;
        }

        /// <summary>
        /// Given a checkbox scheme question, write the header
        /// </summary>
        /// <param name="schemeQuestion"></param>
        private int WriteCheckboxQuestionHeader(SchemeQuestionWithChildQuestionsDto schemeQuestion)
        {
            var choicesCount = schemeQuestion.PossibleAnswers.Count;

            for (var index = 1; index <= choicesCount; index++)
            {
                try
                {
                    var header = "";
                    var answer = schemeQuestion.PossibleAnswers.Single(pa => pa.Order == index);
                    if (_schemeCategory == null)
                        header = QuestionHeaderString() + AnswerHeaderPrefix + answer.Order;
                    else
                        header = QuestionHeaderString() + AnswerHeaderPrefix + answer.Order
                                 + CategoryHeaderPrefix + _schemeCategory.Order;
                    WriteHeader(header);
                    WriteAnnotationHeader(header);
                }
                catch (InvalidOperationException)
                {
                    _result.SetError(ExportResult.CheckboxHeaderInvalidOperation);
                    return _result.Code;
                }
                catch (ArgumentNullException)
                {
                    _result.SetError(ExportResult.CheckboxHeaderArgumentNull);
                    return _result.Code;                    
                }

            }

            if (schemeQuestion.IncludeComment)
            {
                WriteCommentHeader();
            }

            return ExportResult.NoError;
        }

        /// <summary>
        /// Given a multiple choice scheme question, write the header
        /// </summary>
        /// <param name="schemeQuestion"></param>
        private void WriteMultipleChoiceQuestionHeader(SchemeQuestionWithChildQuestionsDto schemeQuestion)
        {
            var header = "";

            if (_schemeCategory == null)
                header = QuestionHeaderString();
            else
                header = QuestionHeaderString() + CategoryHeaderPrefix + _schemeCategory.Order;

            WriteHeader(header);
            WriteAnnotationHeader(header);
            if (schemeQuestion.IncludeComment)
            {
                WriteCommentHeader();
            }

        }

        /// <summary>
        /// Given a text field scheme question, write the header
        /// </summary>
        /// <param name="schemeQuestion"></param>
        private void WriteTextFieldQuestionHeader(SchemeQuestionWithChildQuestionsDto schemeQuestion)
        {
            var header = "";

            if (_schemeCategory == null)
                header = QuestionHeaderString();
            else
                header = QuestionHeaderString() + CategoryHeaderPrefix + _schemeCategory.Order;

            WriteHeader(header);
            WriteAnnotationHeader(header);
            if (schemeQuestion.IncludeComment)
            {
                WriteCommentHeader();
            }

        }

        /// <summary>
        /// Determines the type of question and then calls the appropriate method to write the header
        /// </summary>
        /// <param name="schemeQuestion"></param>
        private int WriteSchemeQuestionHeader(SchemeQuestionWithChildQuestionsDto schemeQuestion)
        {
            var errorCode = ExportResult.NoError;
            
            switch (schemeQuestion.QuestionType)
            {
                case QuestionType.Binary:
                    WriteBinaryQuestionHeader(schemeQuestion);
                    break;

                case QuestionType.Category:
                    errorCode = WriteCategoryQuestionHeader(schemeQuestion);
                    break;

                case QuestionType.Checkbox:
                    errorCode = WriteCheckboxQuestionHeader(schemeQuestion);
                    break;

                case QuestionType.MultipleChoice:
                    WriteMultipleChoiceQuestionHeader(schemeQuestion);
                    break;

                case QuestionType.TextField:
                    WriteTextFieldQuestionHeader(schemeQuestion);
                    break;

                default:
                    errorCode = ExportResult.HeaderInvalidQuestionType;
                    break;
            }

            return errorCode;
        }
        
        /// <summary>
        /// Write the header for each category for a given question
        /// </summary>
        /// <param name="schemeQuestion"></param>
        private int WriteSchemeCategoryQuestionHeader(SchemeQuestionWithChildQuestionsDto schemeQuestion)
        {
            foreach (var category in _schemeCategories)
            {
                _schemeCategory = category;
                var error = WriteSchemeQuestionHeader(schemeQuestion);
                if (error < ExportResult.NoError)
                    return error;
            }

            return ExportResult.NoError;
        }
        
        /// <summary>
        /// Goes thru all questions and child questions recursively and calls the appropriate method to write the header
        /// </summary>
        /// <param name="schemeQuestions"></param>
        private int WriteSchemeQuestionHeaders(List<SchemeQuestionWithChildQuestionsDto> schemeQuestions)
        {
            var currentQuestionOutlinePosition = 1;
            var outlinePrefixNumber = _outlineNumber;
            var error = ExportResult.NoError;

            foreach (var schemeQuestion in schemeQuestions)
            {
                _outlineNumber = outlinePrefixNumber + currentQuestionOutlinePosition.ToString();

                // set categories if a category question
                if (schemeQuestion.QuestionType == QuestionType.Category)
                {
                    _schemeCategories = schemeQuestion.PossibleAnswers;
                    error = WriteSchemeQuestionHeader(schemeQuestion);
                }
                else if (schemeQuestion.IsCategoryQuestion)
                {
                    error = WriteSchemeCategoryQuestionHeader(schemeQuestion);
                }
                else // no longer doing catgeory questions
                {
                    _schemeCategories = null;
                    _schemeCategory = null;
                    error = WriteSchemeQuestionHeader(schemeQuestion);
                }

                if (schemeQuestion.ChildQuestions.Count > 0 && error == ExportResult.NoError)
                {
                    _outlineNumber = _outlineNumber + "c";
                    error = WriteSchemeQuestionHeaders(schemeQuestion.ChildQuestions);
                }

                if (error != ExportResult.NoError)
                    return error;
                
                currentQuestionOutlinePosition += 1;
            }

            return error;
        }

        private int WriteExportHeaders()
        {
           
            // write jurisdiction info
            _csvWriter.WriteField("jurisdiction");
            _csvWriter.WriteField("fips");
            _csvWriter.WriteField("gnis");
            _csvWriter.WriteField("start_date");
            _csvWriter.WriteField("end_date");

            var error = WriteSchemeQuestionHeaders(_schemeTree);

            if (error != ExportResult.NoError)
                return error;
            
            _outlineNumber = "";
            _csvWriter.NextRecord();

            return ExportResult.NoError;

        }

        #endregion


        #region Answers

                // checks to see if the current scheme category was selected and coded
        private bool IsCurrentCategoryCoded()
        {
            // if current 
            if (_codedCategories == null)
                return false;
            
            if (_codedCategories.Any(x => x.SchemeAnswer.Id == _schemeCategory.Id))
                return true;

            return false;
        }


        private void WritePincite(CodedAnswer answer)
        {
            // don't write pinsite for non-statistical export
            if (_codedTextExport == false)
                return;

            // if there is an answer write out the pinsite, 
            if (answer != null)
                _csvWriter.WriteField(answer.Pincite);
            else
            {
                _csvWriter.WriteField("");
            }
        }
        
        private void WriteAnnotation(CodedAnswer answer)
        {
            // don't write comment for non-statistical export
            if (_codedTextExport == false)
                return;

            // if there is a coded answer write out the annotation text, 
            if (answer != null)
            {
                var annotationList = Mapper.Map<string, List<Annotation>>(answer.Annotations).ToList();
                //get list of distinct document name from the list
                var docNames = annotationList.Select(d => d.DocName).Distinct().ToList();
                var combinedText = new StringBuilder(string.Empty);
                foreach (var docuName in docNames)  // build list of annotations for each document
                {
                    combinedText.AppendFormat("Document: {0}",docuName); // write the document name for the list of annotations
                    combinedText.Append("\r\n\n");
                    var listOfText = string.Join("\r\n\n", annotationList.Where(item =>item.DocName == docuName).Select(item => " - " + item.Text.Trim()));
                    combinedText.Append(listOfText);
                    combinedText.Append("\r\n\n");
                }
                _csvWriter.WriteField(combinedText.ToString());
            }
            else
            {
                _csvWriter.WriteField("");
            }       
        }

        private void WriteQuestionComment(CodedQuestionBase codedQuestion)
        {
            // don't write comment for non-statistical export
            if (_codedTextExport == false)
                return;

            // if there is a coded question write out the comment, 
            if (codedQuestion != null)
                _csvWriter.WriteField(codedQuestion.Comment);
            else
            {
                _csvWriter.WriteField("");
            }

            
        }
    
        private bool WroteCategoryNotSelectedAnswer()
        {
            // if we are writing category questions check to see if the category was select 
            // and write out the "skip logic" symbol
            if (_processingCategoryQuestions)
            {
                if (_codedCategories == null)
                {
                    _csvWriter.WriteField("NA");
                    WritePincite(null);
                    return true;                    
                }
                else if (IsCurrentCategoryCoded() == false)
                {
                    _csvWriter.WriteField(CategoryNotSelectedAnswer);
                    WritePincite(null);
                    WriteAnnotation(null);
                    return true;
                }
            }

            return false;
        }

        private int WriteBinaryQuestionAnswer(CodedQuestionBase codedQuestion)
        {
            var field = "NA";
            var error = ExportResult.NoError;

            if (WroteCategoryNotSelectedAnswer())
                return error;

            if (codedQuestion != null && codedQuestion.CodedAnswers.Count != 0)
            {
                try
                {
                    var codedAnswer = codedQuestion.CodedAnswers.Single();
                    var order = codedAnswer.SchemeAnswer.Order;

                    if (order == 1)
                        field = _codedTextExport ? codedAnswer.SchemeAnswer.Text : "1";
                    else if (order == 2)
                        field = _codedTextExport ? codedAnswer.SchemeAnswer.Text : "0";
                    
                    _csvWriter.WriteField(field);
                    WritePincite(codedAnswer);
                    WriteAnnotation(codedAnswer);
                } 
                catch (InvalidOperationException)
                {
                    _result.SetError(ExportResult.BinaryAnswerInvalidOperation);
                    return _result.Code;
                }
                catch (ArgumentNullException)
                {
                    _result.SetError(ExportResult.BinaryAnswerArgumentNull);
                    return _result.Code;
                }

            }
            else
            {
                _csvWriter.WriteField(field);
                WritePincite(null);
                WriteAnnotation(null);
            }

            return error;
        }


        private void WriteCategoryQuestionAnswer(SchemeQuestionWithChildQuestionsDto schemeQuestion,
            CodedQuestionBase codedQuestion)
        {
            var orderedSchemeAnswers = schemeQuestion.PossibleAnswers.OrderBy(x => x.Order).ToList();

            foreach (var schemeAnswer in orderedSchemeAnswers)
            {
                var field = "NA";
                if (codedQuestion != null && codedQuestion.CodedAnswers.Count != 0)
                {
                    var codedAnswer = codedQuestion.CodedAnswers.SingleOrDefault(x => x.SchemeAnswerId == schemeAnswer.Id);
                    if (codedAnswer != null)
                        field = _codedTextExport ? schemeAnswer.Text : "1";
                    else
                        field = _codedTextExport ? "" : "0";

                    _csvWriter.WriteField(field);
                    WritePincite(codedAnswer);
                    WriteAnnotation(codedAnswer);

                }
                else
                {
                    _csvWriter.WriteField(field);
                    WritePincite(null);
                    WriteAnnotation(null);

                }
            }
        }

        private int WriteCheckboxQuestionAnswer(SchemeQuestionWithChildQuestionsDto schemeQuestion,
            CodedQuestionBase codedQuestion)
        {
            var orderedSchemeAnswers = schemeQuestion.PossibleAnswers.OrderBy(x => x.Order).ToList();

            foreach (var schemeAnswer in orderedSchemeAnswers)
            {
                var field = "NA";

                if (WroteCategoryNotSelectedAnswer())
                    continue;

                if (codedQuestion == null || codedQuestion.CodedAnswers.Count == 0)
                {
                    _csvWriter.WriteField(field);
                    WritePincite(null);
                    WriteAnnotation(null);
                    continue;
                }

                try
                {
                    var codedAnswer = codedQuestion.CodedAnswers.SingleOrDefault(x => x.SchemeAnswerId == schemeAnswer.Id);
                    if (codedAnswer != null)
                        field = _codedTextExport ? schemeAnswer.Text : "1";
                    else
                        field = _codedTextExport ? "" : "0";

                    _csvWriter.WriteField(field);
                    WritePincite(codedAnswer);
                    WriteAnnotation(codedAnswer);
                }
                catch (InvalidOperationException)
                {
                    _result.SetError(ExportResult.BinaryAnswerInvalidOperation);
                    return _result.Code;
                }
                catch (ArgumentNullException)
                {
                    _result.SetError(ExportResult.BinaryAnswerArgumentNull);
                    return _result.Code;
                }
            }
            
            return ExportResult.NoError;

        }

        private int WriteMultipleChoiceQuestionAnswer(CodedQuestionBase codedQuestion)
        {
            var field = "NA";
            var error = ExportResult.NoError;

            if (WroteCategoryNotSelectedAnswer())
                return error;

            if (codedQuestion == null || codedQuestion.CodedAnswers.Count == 0)
            {
                _csvWriter.WriteField(field);
                WritePincite(null);
                WriteAnnotation(null);

                return error;
            }

            try
            {
                var codedAnswer = codedQuestion.CodedAnswers.Single();
                field = _codedTextExport ? codedAnswer.SchemeAnswer.Text : codedAnswer.SchemeAnswer.Order.ToString();
                _csvWriter.WriteField(field);
                WritePincite(codedAnswer);
                WriteAnnotation(codedAnswer);
            }
            catch (InvalidOperationException)
            {
                _result.SetError(ExportResult.MultipleChoiceAnswerInvalidOperation);
                return _result.Code;
            }
            catch (ArgumentNullException)
            {
                _result.SetError(ExportResult.MultipleChoiceAnswerArgumentNull);
                return _result.Code;
            }

            return error;

        }

        private int WriteTextFieldQuestionAnswer(CodedQuestionBase codedQuestion)
        {
            var field = "NA";
            var error = ExportResult.NoError;

            if (WroteCategoryNotSelectedAnswer())
                return error;

            if (codedQuestion == null || codedQuestion.CodedAnswers.Count == 0)
            {
                _csvWriter.WriteField(field);
                WritePincite(null);
                WriteAnnotation(null);
                return error;
            }

            try
            {
                var codedAnswer = codedQuestion.CodedAnswers.Single();
                field = codedAnswer.TextAnswer;
                _csvWriter.WriteField(field);
                WritePincite(codedAnswer);
                WriteAnnotation(codedAnswer);
            } 
            catch (InvalidOperationException)
            {
                _result.SetError(ExportResult.TextAnswerInvalidOperation);
                return _result.Code;
            }
            catch (ArgumentNullException)
            {
                _result.SetError(ExportResult.TextAnswerArgumentNull);
                return _result.Code;
            }

            return error;
        }

        private int WriteQuestionAnswer(SchemeQuestionWithChildQuestionsDto schemeQuestion,
            CodedQuestionBase codedQuestion)
        {
            var errorCode = ExportResult.NoError;

            switch (schemeQuestion.QuestionType)
            {
                case QuestionType.Binary:
                    errorCode = WriteBinaryQuestionAnswer(codedQuestion);
                    break;

                case QuestionType.Category:
                    WriteCategoryQuestionAnswer(schemeQuestion, codedQuestion);
                    break;

                case QuestionType.Checkbox:
                    errorCode = WriteCheckboxQuestionAnswer(schemeQuestion, codedQuestion);
                    break;

                case QuestionType.MultipleChoice:
                    errorCode = WriteMultipleChoiceQuestionAnswer(codedQuestion);
                    break;

                case QuestionType.TextField:
                    errorCode = WriteTextFieldQuestionAnswer(codedQuestion);
                    break;

                default:
                    errorCode = ExportResult.AnswerInvalidQuestionType;
                    break;
            }
            if (schemeQuestion.IncludeComment)
                WriteQuestionComment(codedQuestion);

            return errorCode;
        }

        private async Task<int> WriteCategoriesQuestionAnswer(SchemeQuestionWithChildQuestionsDto schemeQuestion)
        {
            var error = ExportResult.NoError;
            
            foreach (var category in _schemeCategories)
            {
                _schemeCategory = category;
                dynamic codedQuestion;
                if (_validated)
                {
                    codedQuestion = 
                        await _unitOfWork.ValidatedCategoryQuestions.SingleOrDefaultAsync(vcq =>
                            vcq.ProjectJurisdiction.Id == _projectJurisdiction.Id &&
                            vcq.SchemeQuestion.Id == schemeQuestion.Id && vcq.Category.Id == _schemeCategory.Id);
                }
                else
                {
                    codedQuestion = 
                        await _unitOfWork.CodedCategoryQuestions.SingleOrDefaultAsync(vcq =>
                            vcq.ProjectJurisdiction.Id == _projectJurisdiction.Id &&
                            vcq.SchemeQuestion.Id == schemeQuestion.Id && vcq.Category.Id == _schemeCategory.Id && vcq.CodedBy.Id == _userId);
                }

                error = WriteQuestionAnswer(schemeQuestion, codedQuestion);

                if (error != ExportResult.NoError)
                    return error;
            }

            return error;
        }

        private async Task<int> WriteQuestionAnswers(List<SchemeQuestionWithChildQuestionsDto> schemeQuestions)
        {
            var error = ExportResult.NoError;
            
            foreach (var schemeQuestion in schemeQuestions)
            {
                // set categories if a category question
                if (schemeQuestion.QuestionType == QuestionType.Category)
                {
                    _processingCategoryQuestions = true;
                    _schemeCategories = schemeQuestion.PossibleAnswers;

                    dynamic codedQuestion;
                    if (_validated)
                    {
                        codedQuestion =  await _unitOfWork.ValidatedQuestions.SingleOrDefaultAsync(vq =>
                            vq.ProjectJurisdiction.Id == _projectJurisdiction.Id &&
                            vq.SchemeQuestion.Id == schemeQuestion.Id);
                    }
                    else
                    {
                        codedQuestion = await _unitOfWork.CodedQuestions.SingleOrDefaultAsync(vq =>
                            vq.ProjectJurisdiction.Id == _projectJurisdiction.Id &&
                            vq.SchemeQuestion.Id == schemeQuestion.Id &&
                            vq.CodedBy.Id == _userId);
                    }

                    _codedCategories = codedQuestion != null ? codedQuestion.CodedAnswers : null;

                    error = WriteQuestionAnswer(schemeQuestion, codedQuestion);
                }
                else if (schemeQuestion.IsCategoryQuestion)
                {
                    error = await WriteCategoriesQuestionAnswer(schemeQuestion);
                }
                else // no longer doing catgeory questions
                {
                    _processingCategoryQuestions = false;
                    _schemeCategories = null;
                    _schemeCategory = null;
                    dynamic codedQuestion;
                    if (_validated)
                    {
                        codedQuestion =
                           await _unitOfWork.ValidatedQuestions.SingleOrDefaultAsync(vq =>
                                vq.ProjectJurisdiction.Id == _projectJurisdiction.Id &&
                                vq.SchemeQuestion.Id == schemeQuestion.Id);
                    }
                    else { 
                        codedQuestion = await _unitOfWork.CodedQuestions.SingleOrDefaultAsync(vq =>
                            vq.ProjectJurisdiction.Id == _projectJurisdiction.Id &&
                            vq.SchemeQuestion.Id == schemeQuestion.Id &&
                            vq.CodedBy.Id == _userId); 
                    }


                    error = WriteQuestionAnswer(schemeQuestion, codedQuestion);
                }

                if (schemeQuestion.ChildQuestions.Count > 0)
                {
                    error = await WriteQuestionAnswers(schemeQuestion.ChildQuestions);
                }
                
                if (error != ExportResult.NoError)
                    return error;

            }

            return error;
        }

        private async Task<int> WriteJurisdictionAnswers()
        {
            var error = ExportResult.NoError;
            var projectJurisdictions = await _unitOfWork.ProjectJurisdictions.FindAsync(pj => pj.Project.Id == _projectId);

            foreach (var projectJurisdiction in projectJurisdictions)
            {
                // set ProjectJurisdiction for other methods
                _projectJurisdiction = projectJurisdiction;
                _csvWriter.WriteField(projectJurisdiction.Jurisdiction.Name);
                _csvWriter.WriteField(projectJurisdiction.Jurisdiction.FipsCode);
                _csvWriter.WriteField(projectJurisdiction.Jurisdiction.GnisCode);
                _csvWriter.WriteField(projectJurisdiction.StartDate.Date.ToShortDateString());
                _csvWriter.WriteField(projectJurisdiction.EndDate.Date.ToShortDateString());

                error = await WriteQuestionAnswers(_schemeTree);
                await _csvWriter.NextRecordAsync();
            }

            return error;
        }
        
        #endregion
        
        private async Task<int> WriteExportFile(string path)
        {
            var error = ExportResult.NoError;
            using (TextWriter writer = new StreamWriter(path,false,new UTF8Encoding(true)))
            {
                await writer.WriteAsync('\uFEFF');
                using (var csvWriter = new CsvWriter(writer))
                {
                    _csvWriter = csvWriter;
                    if ((error = WriteExportHeaders()) != ExportResult.NoError)
                        return error;
                    if ((error = await WriteJurisdictionAnswers()) != ExportResult.NoError)
                        return error;
                }
            }

            // set successful export result code and export file path
            if (error == ExportResult.NoError)
            {
                _result = new ExportResult(path);
            }
                
            return error;
        }
        
        
        #region Codebook
        
        private void WriteCodebookQuestionEntry(SchemeQuestionWithChildQuestionsDto question, string questionNumber,
            string answerVariableLabel, string questionType, string questionText, string valueLabel, string value)
        {
            
            questionNumber = questionNumber.Replace("c", ".");
            _csvWriter.WriteField(questionNumber);
            
            // write answer variable correctly for children of category/tabbed questions
            if (_schemeCategory != null)
                answerVariableLabel = answerVariableLabel + CategoryHeaderPrefix + _schemeCategory.Order;    
            _csvWriter.WriteField(answerVariableLabel);
            
            _csvWriter.WriteField(questionType);
            _csvWriter.WriteField(questionText);
            if (question.QuestionType == QuestionType.TextField)
                _csvWriter.WriteField("[text]");
            else
                _csvWriter.WriteField(valueLabel);
            _csvWriter.WriteField(value);

            _csvWriter.NextRecord();
            
        }


        private int WriteQuestionCodebookEntries(SchemeQuestionWithChildQuestionsDto question)
        {
            var questionNumber = _outlineNumber;
            var questionText = question.Text;
            var questionVariableLabel ="";
            var questionType = "";
            var possibleAnswers = question.PossibleAnswers.OrderBy(x => x.Order).ToImmutableList();
            

            
            switch (question.QuestionType)
            {
                case QuestionType.Binary:
                    questionType = "Binary";
                    questionVariableLabel = QuestionHeaderString();
                    break;
                
                case QuestionType.Category:
                    questionType = "Tabbed";
                    questionVariableLabel = QuestionHeaderString() + CategoryHeaderPrefix;
                    break;
                
                case QuestionType.Checkbox:
                    questionType = "Checkbox";
                    questionVariableLabel = QuestionHeaderString() + AnswerHeaderPrefix;
                    break;
                
                case QuestionType.MultipleChoice:
                    questionType = "Radio Button";
                    questionVariableLabel = QuestionHeaderString();
                    break;
                
                case QuestionType.TextField:
                    questionType = "Text Field";
                    questionVariableLabel = QuestionHeaderString();
                    break;
                
                default:
                    break;
             
            }

            
            foreach (var answer in possibleAnswers)
            {
                
                switch (question.QuestionType)
                {
                    case QuestionType.Binary:
                        var value = answer.Order == 1 ? "1" : "0";
                        WriteCodebookQuestionEntry(question, questionNumber, questionVariableLabel, questionType, 
                            questionText, answer.Text, value);
                        break;
                    case QuestionType.Category:
                        var answerVariableLabel = questionVariableLabel + answer.Order;
                        WriteCodebookQuestionEntry(question, questionNumber, answerVariableLabel, questionType, 
                            questionText, answer.Text, answer.Order.ToString());
                        break;
                    case QuestionType.Checkbox:
                        answerVariableLabel = questionVariableLabel + answer.Order;
                        WriteCodebookQuestionEntry(question, questionNumber, answerVariableLabel, questionType, 
                            questionText, answer.Text, "1");
                        WriteCodebookQuestionEntry(question, questionNumber, answerVariableLabel, questionType, 
                            questionText, "[blank]", "0");
                        break;
                    default:
                        answerVariableLabel = questionVariableLabel;
                        WriteCodebookQuestionEntry(question, questionNumber, answerVariableLabel, questionType, 
                            questionText, answer.Text, answer.Order.ToString());
                        break;
             
                }

            }
            
            return 0;

        }

        private int WriteCategoryQuestionCodebookEntries(SchemeQuestionWithChildQuestionsDto categoryQuestion)
        {

            foreach (var category in _schemeCategories)
            {
                _schemeCategory = category;
                var error = WriteQuestionCodebookEntries(categoryQuestion);
                if (error < ExportResult.NoError)
                    return error;
            }

            return ExportResult.NoError;

        }

    
        private int WriteCodebookEntries(List<SchemeQuestionWithChildQuestionsDto> schemeQuestions)
        {
            var currentQuestionOutlinePosition = 1;
            var outlinePrefixNumber = _outlineNumber;
            var error = ExportResult.NoError;

            foreach (var schemeQuestion in schemeQuestions)
            {
                _outlineNumber = outlinePrefixNumber + currentQuestionOutlinePosition.ToString();

                // set categories if a category question
                if (schemeQuestion.QuestionType == QuestionType.Category)
                {
                    _schemeCategories = schemeQuestion.PossibleAnswers;
                    error = WriteQuestionCodebookEntries(schemeQuestion);
                }
                else if (schemeQuestion.IsCategoryQuestion)
                {
                    error = WriteCategoryQuestionCodebookEntries(schemeQuestion);
                }
                else // no longer doing catgeory questions
                {
                    _schemeCategories = null;
                    _schemeCategory = null;
                    error = WriteQuestionCodebookEntries(schemeQuestion);
                }

                if (schemeQuestion.ChildQuestions.Count > 0 && error == ExportResult.NoError)
                {
                    _outlineNumber = _outlineNumber + "c";
                    error = WriteCodebookEntries(schemeQuestion.ChildQuestions);
                }

                if (error != ExportResult.NoError)
                    return error;
                
                currentQuestionOutlinePosition += 1;
            }

            return error;
        }

        
        /// <summary>
        /// The main entry point for writing codebook files.
        /// </summary>
        /// <param name="path">The path to location file should be written.</param>
        /// <returns>ExportResult</returns>
        private int WriteCodebookFile(string path)
        {
            var error = ExportResult.NoError;
            
            using (TextWriter writer = new StreamWriter(path))
            {
                using (var csvWriter = new CsvWriter(writer))
                {
                    _csvWriter = csvWriter;
                    
                    // write headers for file
                    _csvWriter.WriteField("Question number");
                    _csvWriter.WriteField("Variable label");
                    _csvWriter.WriteField("Question type");
                    _csvWriter.WriteField("Question text");
                    _csvWriter.WriteField("Value label");
                    _csvWriter.WriteField("Value");
                    _csvWriter.NextRecord();
                    
                    // write codebook entries into file
                    if ((error = WriteCodebookEntries(_schemeTree)) != ExportResult.NoError)
                        return error;
                }
            }

            // set successful export result code and export file path
            if (error == ExportResult.NoError)
            {
                _result = new ExportResult(path);
            }
                
            return error;
        }
        
        
        #endregion
        

    }
    

}