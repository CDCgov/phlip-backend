﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using esquire;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

 namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [Authorize]
    public class CodedQuestionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public CodedQuestionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Get coded questions by User and ProjectJurisdiction
        /// </summary>
        /// <remarks>Get all coded questions for the specified User and ProjectJurisdiction. This route is used to populate the coding screen.</remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the project jurisdiction.</param>
        [HttpGet("users/{userId}/projects/{projectId}/jurisdictions/{projectJurisdictionId}/codedquestions", Name = "GetAllCodedQuestions")]
        public async Task<IActionResult> GetAllCodedQuestions(long userId, long projectId, long projectJurisdictionId)
        {
            var userExists = await _unitOfWork.Users.Exists(u => u.Id == userId);
            var projectJurisdiction = await _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);

            if (!userExists || projectJurisdiction == null || projectJurisdiction.ProjectId != projectId )
            {
                return NotFound();
            }

            var codedQuestions = await _unitOfWork.CodedQuestions.FindAsync(cq =>
                cq.CodedBy.Id == userId && cq.ProjectJurisdiction.Id == projectJurisdictionId);

            var codedCategoryQuestions = await _unitOfWork.CodedCategoryQuestions.FindAsync(ccq =>
                ccq.CodedBy.Id == userId && ccq.ProjectJurisdiction.Id == projectJurisdictionId);

            var returnList = new List<object>();
            
            returnList.AddRange(Mapper.Map<IEnumerable<CodedQuestionReturnDto>>(codedQuestions));
            returnList.AddRange(Mapper.Map<IEnumerable<CodedCategoryQuestionReturnDto>>(codedCategoryQuestions));

            return Ok(returnList);
        }
        
        
        /// <summary>
        /// Get a coded question for a user project jurisdiction
        /// </summary>
        /// <remarks>Get a coded question for the specifed jurisdiction, user, and project.</remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the project jurisdiction.</param>
        /// <param name="schemeQuestionId">The ID of the scheme question.</param>
        [HttpGet("users/{userId}/projects/{projectId}/jurisdictions/{projectJurisdictionId}/codedquestions/{schemeQuestionId}", Name = "GetCodedQuestion")]
        public async Task<IActionResult> GetCodedQuestion(long userId, long projectId, long projectJurisdictionId, long schemeQuestionId)
        {
            var userExists = await _unitOfWork.Users.Exists(u => u.Id == userId);
            var projectExists = await _unitOfWork.Projects.Exists(p => p.Id == projectId);
            var projectJurisdictionExists = await _unitOfWork.ProjectJurisdictions.Exists(pj => pj.Id == projectJurisdictionId);
            var schemeQuestion = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            
            var codedQuestion = await _unitOfWork.CodedQuestions.SingleOrDefaultAsync(cq => 
                cq.CodedBy.Id == userId && cq.ProjectJurisdiction.Id == projectJurisdictionId && 
                cq.SchemeQuestion.Id == schemeQuestionId);
            
            var codedCategoryQuestion = await _unitOfWork.CodedCategoryQuestions.FindAsync(cq => 
                cq.CodedBy.Id == userId && cq.ProjectJurisdiction.Id == projectJurisdictionId && 
                cq.SchemeQuestion.Id == schemeQuestionId);

            
            if (!userExists || !projectExists || !projectJurisdictionExists || schemeQuestion == null)
            {
                return NotFound();
            }


            return schemeQuestion.IsCategoryQuestion ? 
                Ok(Mapper.Map<IEnumerable<CodedCategoryQuestionReturnDto>>(codedCategoryQuestion)) : 
                Ok(Mapper.Map<CodedQuestionReturnDto>(codedQuestion));
        }
        
        /// <summary>
        /// Get all coded questions for a scheme question for a specific jurisdiction
        /// </summary>
        /// <remarks>Get all coded questions for a scheme question for a specific jurisdiction</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the project jurisdiction.</param>
        /// <param name="schemeQuestionId">The ID of the scheme question.</param>
        [HttpGet("projects/{projectId}/jurisdictions/{projectJurisdictionId}/codedquestions/{schemeQuestionId}", Name = "GetAllCodedQuestionsForSchemeQuestion")]
        public async Task<IActionResult> GetAllCodedQuestionsForSchemeQuestion(long projectId, long projectJurisdictionId, long schemeQuestionId)
        {
            var codersDictionary = new Dictionary<UserReturnDto, List<object>>();

            var schemeQuestion = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);

            if (!schemeQuestion.IsCategoryQuestion)
            {
                var codedQuestions = await _unitOfWork.CodedQuestions.FindAsync(cq =>
                    cq.SchemeQuestion.Id == schemeQuestionId && cq.ProjectJurisdiction.Id == projectJurisdictionId);
                
                foreach (var codedQuestion in codedQuestions)
                {
                    var coderDto = Mapper.Map<UserReturnDto>(codedQuestion.CodedBy);
                    var codedQuestionReturnDto = Mapper.Map<CodedQuestionReturnDto>(codedQuestion);
                
                    if (!codersDictionary.ContainsKey(coderDto))
                    {
                        codersDictionary.Add(coderDto, new List<object>{codedQuestionReturnDto});
                    }
                    else
                    {
                        var values = codersDictionary[coderDto];
                        values.Add(codedQuestionReturnDto);
                    }
                }
            }
            else
            {

                var codedCategoryQuestions = await _unitOfWork.CodedCategoryQuestions.FindAsync(cq =>
                    cq.SchemeQuestion.Id == schemeQuestionId && cq.ProjectJurisdiction.Id == projectJurisdictionId);

                foreach (var codedQuestion in codedCategoryQuestions)
                {
                    var coderDto = Mapper.Map<UserReturnDto>(codedQuestion.CodedBy);
                    var codedQuestionReturnDto = Mapper.Map<CodedCategoryQuestionReturnDto>(codedQuestion);

                    if (!codersDictionary.ContainsKey(coderDto))
                    {
                        codersDictionary.Add(coderDto, new List<object> {codedQuestionReturnDto});
                    }
                    else
                    {
                        var values = codersDictionary[coderDto];
                        values.Add(codedQuestionReturnDto);
                    }
                }
            }

            var returnList = new List<CodedQuestionsListByCoderReturnDto>();

            foreach (var o in codersDictionary)
            {
                returnList.Add(new CodedQuestionsListByCoderReturnDto
                {
                    Coder = o.Key,
                    CodedQuestions = o.Value
                });
            }

            //if (returnList.Count == 0) NoContent();
   
            return Ok(returnList);
        }
        
        /// <summary>
        /// Get a coded category question for a user project jurisdiction
        /// </summary>
        /// <remarks>Get a coded category question for the specifed jurisdiction, user, and project.</remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the project jurisdiction.</param>
        /// <param name="schemeQuestionId">The ID of the scheme question.</param>
        /// <param name="categoryId">The ID of the category</param>
        [HttpGet("users/{userId}/projects/{projectId}/jurisdictions/{projectJurisdictionId}/codedquestions/{schemeQuestionId}/category/{categoryId}", Name = "GetCodedCategoryQuestion")]
        public async Task<IActionResult> GetCodedCategoryQuestion(long userId, long projectId, long projectJurisdictionId, long schemeQuestionId, long categoryId)
        {
            var codedCategoryQuestion =
                await _unitOfWork.CodedCategoryQuestions.SingleOrDefaultAsync(ccq => 
                    ccq.CodedBy.Id == userId && ccq.ProjectJurisdiction.Id == projectJurisdictionId && 
                    ccq.SchemeQuestion.Id == schemeQuestionId && ccq.Category.Id == categoryId);
            
            if (codedCategoryQuestion == null)
            {
                return NotFound();
            }
            
            var codedCategoryQuestionReturnDto = Mapper.Map<CodedCategoryQuestionReturnDto>(codedCategoryQuestion);
            
            return Ok(codedCategoryQuestionReturnDto);
        }
        
        
        
        /// <summary>
        /// Update a coded question for a user project jurisdiction
        /// </summary>
        /// <remarks>Create a coded question for the specifed jurisdiction, user, and project.</remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the project jurisdiction.</param>
        /// <param name="schemeQuestionId">The ID of the SchemeQuestion</param>
        /// <param name="codedQuestionUpdateDto">The CodedQuestion DTO.</param>
        [HttpPut("users/{userId}/projects/{projectId}/jurisdictions/{projectJurisdictionId}/codedquestions/{schemeQuestionId}", Name = "UpdateCodedQuestion")]
        public async Task<IActionResult> UpdateCodedQuestion(long userId, long projectId, long projectJurisdictionId, long schemeQuestionId, 
            [FromBody] CodedQuestionUpdateDto codedQuestionUpdateDto)
        {   
            if (codedQuestionUpdateDto == null)
            {
                Console.WriteLine("UpdateCodedQuestion: codedQuestionUpdateDto == null");
                return BadRequest();
            }
            
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var schemeQuestion = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            var projectJurisdiction = await _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId); 
            if (project.Status != (int) ProjectStatus.Active)
            {   // project locked no update allowed
                return StatusCode(403);  
            }
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userId);
            
            if (schemeQuestion == null)
            {
                Console.WriteLine("UpdateCodedQuestion: schemeQuestion == null");
                return NotFound();
            }

            if (projectJurisdiction == null)
            {
                Console.WriteLine("UpdateCodedQuestion: projectJurisdiction == null");
                return NotFound();
            }

            if (project == null)
            {
                Console.WriteLine("UpdateCodedQuestion: project == null");
                return NotFound();
            }

            if (user == null)
            {
                Console.WriteLine("UpdateCodedQuestion: user == null");
                return NotFound();
            }
            
            if (!schemeQuestion.IsCategoryQuestion)
            {
                var codedQuestion = await _unitOfWork.CodedQuestions.SingleOrDefaultAsync(cq => cq.Id == codedQuestionUpdateDto.Id);

                if (codedQuestion == null)
                {
                    Console.WriteLine("UpdateCodedQuestion: codedQuestion == null");
                    return NotFound();
                }
                
                if (codedQuestion.SchemeQuestionId != schemeQuestionId)
                {
                    Console.WriteLine("UpdateCodedQuestion: schemeQuestionId mismatch");
                    return BadRequest("SchemeQuestionId mismatch");
                }
                
                Mapper.Map(codedQuestionUpdateDto, codedQuestion);
                if (!await UpdateCodedAnswersForCodedQuestion(codedQuestion, codedQuestionUpdateDto.CodedAnswers, schemeQuestion,
                    projectJurisdictionId, userId, false))
                {
                    return StatusCode(500, "Something went wrong. Please try your request again.");
                }
                HandleFlags(codedQuestion, codedQuestionUpdateDto.Flag, schemeQuestion, user);


                project.UpdateLastEditedDetails(user);
                
                if (await _unitOfWork.Complete() <= 0)
                {
                    return StatusCode(500, "Something went wrong. Please try your request again.");
                }
                
                return Ok(Mapper.Map<CodedQuestionReturnDto>(codedQuestion));
            }

            if (codedQuestionUpdateDto.CategoryId == 0)
            {
                Console.WriteLine("UpdatedCodedQuestion: codedQuestionUpdateDto.CategoryId == 0 when schemeQuestion.isCategoryQuestion == true");
                return null;
            }
                        
            var codedCategoryQuestion = await _unitOfWork.CodedCategoryQuestions.SingleOrDefaultAsync(cq => cq.Id == codedQuestionUpdateDto.Id);

            if (codedCategoryQuestion == null)
            {
                Console.WriteLine("UpdatedCodedQuestion: codedCategoryQuestion == null");
                return null;
            }
            
            if (codedCategoryQuestion.SchemeQuestionId != schemeQuestionId)
            {
                Console.WriteLine("UpdateCodedQuestion: schemeQuestionId mismatch");
                return null;
            }

            if (codedQuestionUpdateDto.CategoryId != codedCategoryQuestion.Category.Id)
            {
                Console.WriteLine("UpdateCodedQuestion: categoryId mismatch");
                return null;
            }

            
            Mapper.Map(codedQuestionUpdateDto, codedCategoryQuestion);

            if (!await UpdateCodedAnswersForCodedQuestion(codedCategoryQuestion, codedQuestionUpdateDto.CodedAnswers,
                schemeQuestion, projectJurisdictionId, userId, false))
            {
                return null;
            }

            HandleFlags(codedCategoryQuestion, codedQuestionUpdateDto.Flag, schemeQuestion, user);
                
            project.UpdateLastEditedDetails(user);
            
            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(500, "Something went wrong. Please try your request again.");
            }
            
            return Ok(Mapper.Map<CodedCategoryQuestionReturnDto>(codedCategoryQuestion));

        }
        
        /// <summary>
        /// Create a coded question object for a user project jurisdiction
        /// </summary>
        /// <remarks>Create a coded question for the specifed jurisdiction, user, and project.</remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the project jurisdiction.</param>
        /// <param name="schemeQuestionId">The ID of the SchemeQuestion</param>
        /// <param name="codedQuestionCreationDto">The CodedQuestion DTO.</param>
        [HttpPost("users/{userId}/projects/{projectId}/jurisdictions/{projectJurisdictionId}/codedquestions/{schemeQuestionId}", Name = "CreateCodedQuestion")]
        public async Task<IActionResult> CreateCodedQuestion(long userId, long projectId, long projectJurisdictionId, long schemeQuestionId, 
            [FromBody] CodedQuestionCreationDto codedQuestionCreationDto)
        {   
            if (codedQuestionCreationDto == null)
            {
                Console.WriteLine("CreateCodedQuestion: codedQuestionCreationDto == null");
                return BadRequest();
            }
            
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var schemeQuestion = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            var projectJurisdiction = await _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project.Status != (int) ProjectStatus.Active)
            {
                return Forbid();
            }
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (schemeQuestion == null)
            {
                Console.WriteLine("CreateCodedQuestion: schemeQuestion == null");
                return NotFound();
            }

            if (projectJurisdiction == null)
            {
                Console.WriteLine("CreateCodedQuestion: projectJurisdiction == null");
                return NotFound();
            }

            if (user == null)
            {
                Console.WriteLine("CreateCodedQuestion: user == null");
                return NotFound();
            }

            if (project == null)
            {
                Console.WriteLine("CreateCodedQuestion: project == null");
                return NotFound();
            }
            
            if (schemeQuestion.IsCategoryQuestion)
            {
                if (codedQuestionCreationDto.CategoryId == 0)
                {
                    return BadRequest("CategoryId is missing or invalid");
                }

                CodedCategoryQuestion codedCategoryQuestion;
                /*var codedCategoryQuestion = await _unitOfWork.CodedCategoryQuestions.SingleOrDefaultAsync(ccq => 
                    ccq.CodedBy.Id == userId && ccq.ProjectJurisdiction.Id == projectJurisdictionId && 
                    ccq.SchemeQuestion.Id == schemeQuestionId && ccq.Category.Id == codedQuestionCreationDto.CategoryId);
                
                if (codedCategoryQuestion != null)
                {
                    return StatusCode(303, Mapper.Map<CodedCategoryQuestion>(codedCategoryQuestion));
                }*/

                var category =
                    await _unitOfWork.SchemeAnswers.SingleOrDefaultAsync(sa =>
                        sa.Id == codedQuestionCreationDto.CategoryId);
                if (category == null)
                {
                    Console.WriteLine("CreateCodedQuestionForCategories: category == null");
                    return NotFound("Category not found");
                }
            
                codedCategoryQuestion = Mapper.Map<CodedCategoryQuestion>(codedQuestionCreationDto);
                codedCategoryQuestion.SchemeQuestionId = schemeQuestionId;
                codedCategoryQuestion.Category = category;

                codedCategoryQuestion.ProjectJurisdiction = projectJurisdiction;
                codedCategoryQuestion.CodedBy = user;

                await UpdateCodedAnswersForCodedQuestion(codedCategoryQuestion, codedQuestionCreationDto.CodedAnswers,
                    schemeQuestion, projectJurisdictionId, userId, false);
                
                /*if (!await UpdateCodedAnswersForCodedQuestion(codedCategoryQuestion, codedQuestionCreationDto.CodedAnswers,
                    schemeQuestion, projectJurisdictionId, userId, false))
                {
                    return StatusCode(500, "Something went wrong. Please try your request again.");
                }*/

                HandleFlags(codedCategoryQuestion, codedQuestionCreationDto.Flag, schemeQuestion, user);
                
                _unitOfWork.CodedCategoryQuestions.Add(codedCategoryQuestion);

                project.UpdateLastEditedDetails(user);

                await _unitOfWork.Complete();
                
                /*if (await _unitOfWork.Complete() <= 0)
                {
                    return StatusCode(500, "Something went wrong. Please try your request again.");
                }*/
                
                return Ok(Mapper.Map<CodedCategoryQuestionReturnDto>(codedCategoryQuestion));
            }

            /*if (await _unitOfWork.CodedQuestions.Exists(cq => cq.CodedBy.Id == userId &&
                cq.ProjectJurisdiction.Id == projectJurisdictionId &&
                cq.SchemeQuestion.Id == schemeQuestionId)) 
            {
                var existingCodedQuestion = await _unitOfWork.CodedQuestions.SingleOrDefaultAsync(cq => 
                  cq.CodedBy.Id == userId &&
                  cq.ProjectJurisdiction.Id == projectJurisdictionId &
                  cq.SchemeQuestion.Id == schemeQuestionId);

                return StatusCode(303, Mapper.Map<CodedQuestionReturnDto>(existingCodedQuestion));

            }*/
               
            var codedQuestion = Mapper.Map<CodedQuestion>(codedQuestionCreationDto);
            codedQuestion.SchemeQuestionId = schemeQuestionId;
            codedQuestion.CodedBy = user;

            codedQuestion.ProjectJurisdiction = projectJurisdiction;

            await UpdateCodedAnswersForCodedQuestion(codedQuestion, codedQuestionCreationDto.CodedAnswers,
                schemeQuestion, projectJurisdictionId, userId, false);
            
/*            if (!await UpdateCodedAnswersForCodedQuestion(codedQuestion, codedQuestionCreationDto.CodedAnswers,
                schemeQuestion, projectJurisdictionId, userId, false))
            {
                return StatusCode(500, "Something went wrong. Please try your request again.");
            }*/

            HandleFlags(codedQuestion, codedQuestionCreationDto.Flag, schemeQuestion, user);

            _unitOfWork.CodedQuestions.Add(codedQuestion);

            project.UpdateLastEditedDetails(user);

            await _unitOfWork.Complete();
            
            /*if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(500, "Something went wrong. Please try your request again.");
            }*/
            
            return Ok(Mapper.Map<CodedQuestionReturnDto>(codedQuestion));
            
          
         }
        
        /// <summary>
        /// Get all validated questions for a project jurisdiction
        /// </summary>
        /// <remarks>Get all validated questions for the specifed jurisdiction and project.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the ProjectJurisdiction.</param>
        [HttpGet("projects/{projectId}/jurisdictions/{projectJurisdictionId}/validatedquestions", Name = "GetAllValidatedQuestions")]
        public async Task<IActionResult> GetAllValidatedQuestions(long projectId, long projectJurisdictionId)
        {
            var projectJurisdiction = await _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);

            if (projectJurisdiction?.ProjectId != projectId )
            {
                return NotFound();
            }

            var validatedQuestions = await _unitOfWork.ValidatedQuestions.FindAsync(cq =>
                cq.ProjectJurisdiction.Id == projectJurisdictionId);

            var validatedCategoryQuestions = await _unitOfWork.ValidatedCategoryQuestions.FindAsync(vcq =>
                vcq.ProjectJurisdiction.Id == projectJurisdictionId);

            var returnList = new List<object>();
            
            returnList.AddRange(Mapper.Map<IEnumerable<ValidatedQuestionReturnDto>>(validatedQuestions));
            returnList.AddRange(Mapper.Map<IEnumerable<ValidatedCategoryQuestionReturnDto>>(validatedCategoryQuestions));

            return Ok(returnList);
        }

        /// <summary>
        /// Get a validated question for a project jurisdiction
        /// </summary>
        /// <remarks>Get a validated question for the specifed ProjectJurisdiction and SchemeQuestion.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the project jurisdiction.</param>
        /// <param name="schemeQuestionId">The ID of the scheme question.</param>
        [HttpGet("projects/{projectId}/jurisdictions/{projectJurisdictionId}/validatedquestions/{schemeQuestionId}", Name = "GetValidatedQuestion")]
        public async Task<IActionResult> GetValidatedQuestion(long projectId, long projectJurisdictionId, long schemeQuestionId)
        {
            var projectExists = await _unitOfWork.Projects.Exists(p => p.Id == projectId);
            var projectJurisdictionExists = await _unitOfWork.ProjectJurisdictions.Exists(pj => pj.Id == projectJurisdictionId);
            var schemeQuestion = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            
            var validatedQuestion = await _unitOfWork.ValidatedQuestions.SingleOrDefaultAsync(cq => 
                cq.ProjectJurisdiction.Id == projectJurisdictionId && 
                cq.SchemeQuestion.Id == schemeQuestionId);
            
            var validatedCategoryQuestion = await _unitOfWork.ValidatedCategoryQuestions.FindAsync(cq => 
                cq.ProjectJurisdiction.Id == projectJurisdictionId && 
                cq.SchemeQuestion.Id == schemeQuestionId);

            if (!projectExists || !projectJurisdictionExists || schemeQuestion == null)
            {
                return NotFound();
            }

            return schemeQuestion.IsCategoryQuestion ? 
                Ok(Mapper.Map<List<ValidatedCategoryQuestionReturnDto>>(validatedCategoryQuestion)) :
                Ok(Mapper.Map<ValidatedQuestionReturnDto>(validatedQuestion));
        }
        
        /// <summary>
        /// Create a validated question for a project jurisdiction
        /// </summary>
        /// <remarks>Create a validated question for the specifed jurisdiction and project.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the ProjectJurisdiction.</param>
        /// <param name="schemeQuestionId">The ID of the SchemeQuestion</param>
        /// <param name="validatedQuestionCreationDto">The ValidatedQuestionCreationDto.</param>
        [HttpPost("projects/{projectId}/jurisdictions/{projectJurisdictionId}/validatedquestions/{schemeQuestionId}", Name = "CreateValidatedQuestion")]
        public async Task<IActionResult> CreateValidatedQuestion(long projectId, long projectJurisdictionId,
            long schemeQuestionId,
            [FromBody] ValidatedQuestionCreationDto validatedQuestionCreationDto)
        {
            if (validatedQuestionCreationDto == null)
            {
                return BadRequest();
            }
            
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await
                _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == validatedQuestionCreationDto.ValidatedBy);
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project.Status != (int) ProjectStatus.Active)
            {
                return Forbid();
            }
            var schemeQuestion = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            var projectJurisdiction = await 
                _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);
            

            if (user == null)
            {
                Console.WriteLine("CreateValidatedQuestion: user == null");
                return NotFound();
            }
            
            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                Console.WriteLine("CreateValidatedQuestion: user does not have edit rights");
                return Forbid();
            }
            

            if (schemeQuestion == null)
            {
                Console.WriteLine("CreateValidatedQuestion: schemeQuestion == null");
                return NotFound();
            }
            

            if (projectJurisdiction == null)
            {
                Console.WriteLine("CreateValidatedQuestion: projectJurisdiction does not exist");
                return NotFound();
            }

            if (project == null)
            {
                return NotFound();
            }
            
            if (schemeQuestion.IsCategoryQuestion)
            {
                if (validatedQuestionCreationDto.CategoryId == 0)
                {
                    Console.WriteLine("CreateValidatedQuestion: validatedQuestionCreationDto.CategoryId == 0 for a category child question.");
                    return BadRequest("CategoryId is missing or invalid.");
                }

                ValidatedCategoryQuestion validatedCategoryQuestion;
                
                /*var validatedCategoryQuestion = await 
                        _unitOfWork.ValidatedCategoryQuestions.SingleOrDefaultAsync(vcq => 
                            vcq.ProjectJurisdiction.Id == projectJurisdictionId &&
                            vcq.SchemeQuestion.Id == schemeQuestionId &&
                            vcq.Category.Id == validatedQuestionCreationDto.CategoryId);
                    
                    if (validatedCategoryQuestion != null)
                    {
                        return StatusCode(303, Mapper.Map<ValidatedCategoryQuestionReturnDto>(validatedCategoryQuestion));
                    }*/

                    var category = await _unitOfWork.SchemeAnswers.SingleOrDefaultAsync(sa => sa.Id == validatedQuestionCreationDto.CategoryId);
                    if (category == null)
                    {
                        Console.WriteLine("CreateValidatedQuestion: category == null");
                        return NotFound("Category not found.");
                    }
                
                    validatedCategoryQuestion = Mapper.Map<ValidatedCategoryQuestion>(validatedQuestionCreationDto);
                    validatedCategoryQuestion.SchemeQuestionId = schemeQuestionId;
                    validatedCategoryQuestion.Category = category;

                    validatedCategoryQuestion.ProjectJurisdiction = projectJurisdiction;
                    
                    validatedCategoryQuestion.ValidatedBy = user;

                await UpdateCodedAnswersForCodedQuestion(validatedCategoryQuestion,
                    validatedQuestionCreationDto.CodedAnswers,
                    schemeQuestion, projectJurisdictionId, (long) validatedQuestionCreationDto.ValidatedBy, false);
                
                /*if (!await UpdateCodedAnswersForCodedQuestion(validatedCategoryQuestion, validatedQuestionCreationDto.CodedAnswers,
                    schemeQuestion, projectJurisdictionId, validatedQuestionCreationDto.ValidatedBy, false))
                {
                    return StatusCode(500, "Something went wrong. Please try your request again.");
                }*/

                HandleFlags(validatedCategoryQuestion, validatedQuestionCreationDto.Flag, schemeQuestion, user);

                _unitOfWork.ValidatedCategoryQuestions.Add(validatedCategoryQuestion);

                project.UpdateLastEditedDetails(user);

                await _unitOfWork.Complete();
                
                /*if (await _unitOfWork.Complete() <= 0)
                    return StatusCode(500, "Something went wrong. Please try your request again.");*/

                
                return Ok(Mapper.Map<ValidatedCategoryQuestionReturnDto>(validatedCategoryQuestion));
            }

            ValidatedQuestion validatedQuestion;
            /*var validatedQuestion =
                await _unitOfWork.ValidatedQuestions.SingleOrDefaultAsync(vq => 
                    vq.ProjectJurisdiction.Id == projectJurisdictionId &&
                    vq.SchemeQuestion.Id == schemeQuestionId);

            if (validatedQuestion != null)
            {
                return StatusCode(303, Mapper.Map<ValidatedQuestionReturnDto>(validatedQuestion));
            }*/
               
            validatedQuestion = Mapper.Map<ValidatedQuestion>(validatedQuestionCreationDto);
            validatedQuestion.SchemeQuestionId = schemeQuestionId;
            
            validatedQuestion.ValidatedBy = user;

            validatedQuestion.ProjectJurisdiction = projectJurisdiction;

            await UpdateCodedAnswersForCodedQuestion(validatedQuestion, validatedQuestionCreationDto.CodedAnswers,
                schemeQuestion, projectJurisdictionId, (long) validatedQuestionCreationDto.ValidatedBy, false);
            
            /*if (!await UpdateCodedAnswersForCodedQuestion(validatedQuestion, validatedQuestionCreationDto.CodedAnswers,
                schemeQuestion, projectJurisdictionId, validatedQuestionCreationDto.ValidatedBy, false))
            {
                return StatusCode(500, "Something went wrong. Please try your request again.");
            }*/

            HandleFlags(validatedQuestion, validatedQuestionCreationDto.Flag, schemeQuestion, user);

            _unitOfWork.ValidatedQuestions.Add(validatedQuestion);

            project.UpdateLastEditedDetails(user);

            await _unitOfWork.Complete();
            
            /*if (await _unitOfWork.Complete() <= 0)
                return StatusCode(500, "Something went wrong. Please try your request again.");*/

            return Ok(Mapper.Map<ValidatedQuestionReturnDto>(validatedQuestion));
        }
        
        /// <summary>
        /// Update a validated question for a project jurisdiction
        /// </summary>
        /// <remarks>Update a validated question for the specifed jurisdiction and project.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the ProjectJurisdiction.</param>
        /// <param name="schemeQuestionId">The ID of the SchemeQuestion</param>
        /// <param name="validatedQuestionUpdateDto">The ValidatedQuestionUpdateDto.</param>
        [HttpPut("projects/{projectId}/jurisdictions/{projectJurisdictionId}/validatedquestions/{schemeQuestionId}", Name = "UpdateValidatedQuestion")]
        public async Task<IActionResult> UpdateValidatedQuestion(long projectId, long projectJurisdictionId,
            long schemeQuestionId,
            [FromBody] ValidatedQuestionUpdateDto validatedQuestionUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await 
                _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == validatedQuestionUpdateDto.ValidatedBy);
            var schemeQuestion = await  _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            var projectJurisdiction = await 
                _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project.Status != (int) ProjectStatus.Active)
            {   // project locked no update allowed
                return StatusCode(403);  
            }
            if (user == null)
            {
                Console.WriteLine("UpdateValidatedQuestion: user == null");
                return NotFound();
            }
            
            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                Console.WriteLine("UpdateValidatedQuestion: user does not have edit rights");
                return Forbid();
            }
            

            if (schemeQuestion == null)
            {
                Console.WriteLine("UpdateValidatedQuestion: schemeQuestion == null");
                return NotFound();
            }
            

            if (projectJurisdiction == null)
            {
                Console.WriteLine("UpdateValidatedQuestion: projectJurisdiction does not exist");
                return NotFound();
            }

            if (project == null)
            {
                return NotFound();
            }

            if (!schemeQuestion.IsCategoryQuestion)
            {
                var validatedQuestion = await _unitOfWork.ValidatedQuestions.SingleOrDefaultAsync(vq => vq.Id == validatedQuestionUpdateDto.Id);

                if (validatedQuestion == null)
                {
                    Console.WriteLine("UpdateValidatedQuestion: validedQuestion == null");
                    return NotFound("ValidatedQuestion not found.");
                }

                if (validatedQuestion.SchemeQuestionId != schemeQuestionId)
                {
                    Console.WriteLine("UpdateValidatedQuestion: schemeQuestionId mismatch");
                    return BadRequest("SchemeQuestionId mismatch");
                }
                    
                Mapper.Map(validatedQuestionUpdateDto, validatedQuestion);
                if (!await UpdateCodedAnswersForCodedQuestion(validatedQuestion, validatedQuestionUpdateDto.CodedAnswers, schemeQuestion,
                    projectJurisdictionId, (long) validatedQuestionUpdateDto.ValidatedBy, true))
                {
                    return StatusCode(500, "Something went wrong. Please try your request again.");
                }
                HandleFlags(validatedQuestion, validatedQuestionUpdateDto.Flag, schemeQuestion, user);

                validatedQuestion.ValidatedBy = user;

                project.UpdateLastEditedDetails(user);
                
                if (await _unitOfWork.Complete() <= 0)
                    return StatusCode(500, "Something went wrong. Please try your request again.");
                
                return Ok(Mapper.Map<ValidatedQuestionReturnDto>(validatedQuestion));
            }

            if (validatedQuestionUpdateDto.CategoryId == 0)
            {
                Console.WriteLine("UpdateValidatedQuestion: validatedQuestionUpdateDto.CategoryId == 0 for a category child question.");
                return BadRequest("CategoryId missing or invalid.");
            }
             
            var validatedCategoryQuestion = await _unitOfWork.ValidatedCategoryQuestions.SingleOrDefaultAsync(vcq => vcq.Id == validatedQuestionUpdateDto.Id);

            if (validatedCategoryQuestion == null)
            {
                Console.WriteLine("UpdatedValidatedQuestion: validatedCategoryQuestion == null");
                return NotFound("ValidatedCategoryQuestion not found.");
            }

            if (validatedCategoryQuestion.SchemeQuestionId != schemeQuestionId)
            {
                Console.WriteLine("UpdateValidatedQuestion: schemeQuestionId mismatch");
                return BadRequest("SchemeQuestionId mismatch");
            }
            
            if (validatedQuestionUpdateDto.CategoryId != validatedCategoryQuestion.Category.Id)
            {
                Console.WriteLine("UpdateValidatedQuestion: categoryId mismatch");
                return BadRequest("CategoryId mismatch");
            }
            
            Mapper.Map(validatedQuestionUpdateDto, validatedCategoryQuestion);

            if (!await UpdateCodedAnswersForCodedQuestion(validatedCategoryQuestion, validatedQuestionUpdateDto.CodedAnswers,
                schemeQuestion, projectJurisdictionId, (long) validatedQuestionUpdateDto.ValidatedBy, true))
            {
                return StatusCode(500, "Something went wrong. Please try your request again.");
            }

            HandleFlags(validatedCategoryQuestion, validatedQuestionUpdateDto.Flag, schemeQuestion, user);
            
            validatedCategoryQuestion.ValidatedBy = user;
            
            project.UpdateLastEditedDetails(user);
            
            if (await _unitOfWork.Complete() <= 0)
                return StatusCode(500, "Something went wrong. Please try your request again.");

            return Ok(Mapper.Map<ValidatedCategoryQuestionReturnDto>(validatedCategoryQuestion));

        }
        
        private async Task<bool> UpdateCodedAnswersForCodedQuestion(CodedQuestionBase codedQuestion,
            ICollection<CodedAnswerCreationDto> codedAnswers, SchemeQuestion schemeQuestion,
            long projectJurisdictionId, long userId, bool isValidated)
        {
            if (schemeQuestion == null)
            {
                Console.WriteLine("UpdateCodedAnswersForCodedQuestion: schemeQuestion == null");
                return false;
            }

            // Return false if any codedAnswer in the DTO has a schemeAnswerId that is not in the list of PossibleAnswers for this scheme question
            if (codedAnswers.Any(codedAnswer => schemeQuestion.PossibleAnswers.All(pa => pa.Id != codedAnswer.SchemeAnswerId)))
            {
                Console.WriteLine("UpdateCodedAnswersForCodedQuestion: schemeAnswerId is not valid");
                return false;
            }

            switch (schemeQuestion.QuestionType)
            {
                // Handle single answer question types - these question types should never have more than one coded answer.
                case QuestionType.Binary:
                case QuestionType.MultipleChoice:
                case QuestionType.TextField:
                    return UpdateCodedAnswersForSingleAnswerTypes(codedQuestion, codedAnswers,
                        schemeQuestion);
                // Handle multiple answer question types - these question types can have more than one coded answer.
                case QuestionType.Category:
                case QuestionType.Checkbox:
                    return await UpdateCodedAnswersForMultipleAnswerTypes(codedQuestion, codedAnswers, schemeQuestion, projectJurisdictionId, userId, isValidated);
                default:
                    return false;
            }
        }

        private bool UpdateCodedAnswersForSingleAnswerTypes(CodedQuestionBase codedQuestion,
            ICollection<CodedAnswerCreationDto> codedAnswers, SchemeQuestion schemeQuestion)
        {
            CodedAnswerCreationDto codedAnswerDtoForSingleAnswerTypes;

            try
            {
                codedAnswerDtoForSingleAnswerTypes =
                    codedAnswers.SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                Console.Write(
                    "CodedQuestionUpdateDto has an invalid number of CodedAnswers for this question type - CodedQuestionId: " +
                    codedQuestion.Id + " codedAnswers.Count: " + codedAnswers.Count);
                return false;
            }

            if (codedAnswerDtoForSingleAnswerTypes == null)
            {
                // We received a coded question without a coded answer
                // This means the user has cleared their answer.
                // Delete any answers from codedQuestion.CodedAnswers
                _unitOfWork.CodedAnswers.RemoveRange(codedQuestion.CodedAnswers);
                return true;
            }

            // Add or update the answer
            CodedAnswer codedAnswerMatch = null;
            try
            {
                codedAnswerMatch = codedQuestion.CodedAnswers.SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                // Duplicate coded answer detected.
                Console.Write(
                    "CodedQuestion has an invalid number of CodedAnswers for this question type - CodedQuestionId: " +
                    codedQuestion.Id + " CodedQuestion: " + codedQuestion);
            }

            if (codedAnswerMatch != null)
            {
                if (schemeQuestion.QuestionType == QuestionType.TextField)
                {
                    codedAnswerMatch.TextAnswer = codedAnswerDtoForSingleAnswerTypes.TextAnswer;
                }

                if (codedAnswerMatch.SchemeAnswerId != codedAnswerDtoForSingleAnswerTypes.SchemeAnswerId)
                {
                    codedAnswerMatch.SchemeAnswerId = (long) codedAnswerDtoForSingleAnswerTypes.SchemeAnswerId;
                }

                codedAnswerMatch.Pincite = codedAnswerDtoForSingleAnswerTypes.Pincite;
             
                codedAnswerMatch.Annotations = Mapper.Map<List<Annotation>,string>(codedAnswerDtoForSingleAnswerTypes.Annotations);
            }
            else
            {
                // There was no coded answer - add one.
                codedQuestion.CodedAnswers.Add(Mapper.Map<CodedAnswer>(codedAnswerDtoForSingleAnswerTypes));
            }

            return true;
        }

        private async Task<bool> UpdateCodedAnswersForMultipleAnswerTypes(CodedQuestionBase codedQuestion,
            ICollection<CodedAnswerCreationDto> codedAnswers, SchemeQuestion schemeQuestion, long projectJurisdictionId, long userId, bool isValidated)
        {
            foreach (var possibleAnswer in schemeQuestion.PossibleAnswers)
            {
                CodedAnswerCreationDto codedAnswerDto;
                try
                {
                    codedAnswerDto =
                        codedAnswers.SingleOrDefault(ca =>
                            ca.SchemeAnswerId == possibleAnswer.Id);

                }
                catch (InvalidOperationException)
                {
                    Console.Write(
                        "CodedQuestionUpdateDto has multiple CodedAnswers matching the id for this possible answer - CodedQuestionId: " +
                        codedQuestion.Id);
                    return false;
                }

                if (codedAnswerDto != null)
                {
                    // Add or update coded answer
                    CodedAnswer codedAnswer = null;
                    try
                    {
                        codedAnswer =
                            codedQuestion.CodedAnswers.SingleOrDefault(ca =>
                                ca.SchemeAnswerId == possibleAnswer.Id);
                    }
                    catch (InvalidOperationException)
                    {
                        // Duplicated coded answer detected.
                        Console.Write(
                            "CodedQuestion has more than one CodedAnswer for this possible answer - CodedQuestionId: " +
                            codedQuestion.Id + " CodedQuestion: " + codedQuestion);
                    }

                    if (codedAnswer == null)
                    {
                        // There was no coded answer - add one.
                        codedQuestion.CodedAnswers.Add(Mapper.Map<CodedAnswer>(codedAnswerDto));
                    }
                    else
                    {
                        codedAnswer.Pincite = codedAnswerDto.Pincite;
                        codedAnswer.Annotations = Mapper.Map<List<Annotation>,string>(codedAnswerDto.Annotations);
                    }
                }
                else
                {
                    // No codedAnswerDto was sent for this possible answer.
                    // If a coded answer exists that matches this possible answer, the user has cleared this answer.
                    // If it exists - remove coded answer. If this is a parent category question, delete coded category questions where categoryId == possibleAnswer.Id
                    CodedAnswer codedAnswerToDelete;
                    try
                    {
                        codedAnswerToDelete =
                            codedQuestion.CodedAnswers.SingleOrDefault(ca =>
                                ca.SchemeAnswerId == possibleAnswer.Id);
                    }
                    catch (InvalidOperationException)
                    {
                        codedAnswerToDelete = null;
                    }

                    if (codedAnswerToDelete == null) continue;

                    codedQuestion.CodedAnswers.Remove(codedAnswerToDelete);

                    if (schemeQuestion.QuestionType != QuestionType.Category) continue;

                    if (isValidated)
                    {

                        
                        var validatedCategoryQuestions = await _unitOfWork.ValidatedCategoryQuestions.FindAsync(vcq =>
                            vcq.ProjectJurisdiction.Id == projectJurisdictionId &&
                            vcq.Category.Id == possibleAnswer.Id);

                        var list = validatedCategoryQuestions.ToList();
                        
                        foreach (var cq in list)
                        {
                            _unitOfWork.CodedAnswers.RemoveRange(cq.CodedAnswers);
                        }

                        _unitOfWork.ValidatedCategoryQuestions.RemoveRange(list);

                    }
                    else
                    {
                        var codedCategoryQuestions = await _unitOfWork.CodedCategoryQuestions.FindAsync(ccq => 
                            ccq.CodedBy.Id == userId && ccq.ProjectJurisdiction.Id == projectJurisdictionId && 
                            ccq.SchemeQuestion.Id == schemeQuestion.Id && ccq.Category.Id == possibleAnswer.Id);

                        var list = codedCategoryQuestions.ToList();
                        
                        foreach (var cq in list)
                        {
                            _unitOfWork.CodedAnswers.RemoveRange(cq.CodedAnswers);
                        }

                        _unitOfWork.CodedCategoryQuestions.RemoveRange(list);
                    }
                }
            }
            return true;
        }
        
        private bool HandleFlags(CodedQuestionBase codedQuestion, FlagCreationDto flagCreationDto, SchemeQuestion schemeQuestion,
         User user)
        {
            if (flagCreationDto == null) return false;
            
            switch (flagCreationDto.Type)
            {
                case FlagType.Red:
                    return false;
                case FlagType.Green: case FlagType.Yellow:
                    var codedQuestionFlag = codedQuestion.Flag ?? new CodedQuestionFlag();

                    codedQuestionFlag.Notes = flagCreationDto.Notes;
                    codedQuestionFlag.Type = flagCreationDto.Type;
                    codedQuestionFlag.SetRaisedDetails(user);
                    
                    codedQuestion.Flag = codedQuestionFlag;
                    break;
                default:
                    return false;
            }

            return true;
        }
        
        /// <summary>
        /// Create validated questions for a project jurisdiction using existing coded questions
        /// </summary>
        /// <remarks>Create a validated question for the specifed jurisdiction and project using coded questions by a user</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="projectJurisdictionId">The ID of the ProjectJurisdiction.</param>
        /// <param name="fromUserId">The ID of the user whom answers will be used to create validated answers.</param>
        [HttpPost("projects/{projectId}/jurisdictions/{projectJurisdictionId}/bulkValidatedQuestions/{fromUserId}",
            Name = "BulkValidatedQuestion")]
        public async Task<IActionResult> BulkValidateQuestion(long projectId, long fromUserId,
            long projectJurisdictionId = -1)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);
            var requestedUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);
            if (requestedUser == null)
            {
                Console.WriteLine("BulkValidatedQuestion: user == null");
                return NotFound();
            }

            if (!_unitOfWork.Users.CanUserEdit(requestedUser))
            {
                Console.WriteLine("BulkValidatedQuestion: user does not have edit rights");
                return Forbid();
            }

            var project = await _unitOfWork.Projects.GetProjectWithScheme(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }

            if (project.Status != (int) ProjectStatus.Active)
            {
                return Forbid();
            }

            List<ProjectJurisdiction> projectJurisdictions = new List<ProjectJurisdiction>();
            if (projectJurisdictionId == -1)
            {
                projectJurisdictions =
                    await _unitOfWork.ProjectJurisdictions.FindAsync(p => p.Project.Id == projectId);
            }
            else
            {
                var projectJurisdiction = await _unitOfWork.ProjectJurisdictions
                    .FirstOrDefaultAsync(pj => pj.Id == projectJurisdictionId);
                if (projectJurisdiction != null)
                {
                    projectJurisdictions.Add(projectJurisdiction);
                }
            }

            List<object> validatedQuestions = new List<object>();
            // build a list of coded questions for each jurisdiction
            foreach (var projectJurisdiction in projectJurisdictions)
            {
                IActionResult questionsResult =
                    await GetAllCodedQuestions(fromUserId, projectId, projectJurisdiction.Id);
                if (questionsResult is OkObjectResult okResult)
                {
                    List<object> myQuestions = okResult.Value as List<object>;
                    foreach (var question in myQuestions)
                    {
                        IActionResult validatedResult;
                        if (question.GetType() == typeof(CodedCategoryQuestionReturnDto)) // handle category question
                        {
                            //var jsonQuestion = JsonConvert.SerializeObject(question); 
                            CodedCategoryQuestionReturnDto parentCategoryQuestion =
                                (CodedCategoryQuestionReturnDto) question;
                            // get all category child questions if exist
                            var childQuestions = await _unitOfWork.CodedCategoryQuestions.FindAsync(cq =>
                                cq.SchemeQuestionId == parentCategoryQuestion.SchemeQuestionId && cq.ProjectJurisdiction.Id == projectJurisdiction.Id);
                            foreach (var categoryQuestion in childQuestions)
                            {
                                // build a validation object from the model
                                // check if the question has been validated.  if yes, either override or skip
                                var existingValidatedQuestion =
                                    await _unitOfWork.ValidatedCategoryQuestions.FirstOrDefaultAsync(vq =>
                                        vq.SchemeQuestionId == categoryQuestion.SchemeQuestionId &&
                                        vq.ProjectJurisdiction.Id == projectJurisdiction.Id && vq.Category.Id == categoryQuestion.Category.Id);
                                if (existingValidatedQuestion != null)
                                {
                                    // question has been validated, override
                                    var validateDto = Mapper.Map<ValidatedQuestionUpdateDto>(categoryQuestion);
                                    validateDto.ValidatedBy = requestedUser.Id;
                                    validateDto.Id = existingValidatedQuestion.Id; // override the id using existing;
                                    validatedResult = await UpdateValidatedQuestion(projectId,
                                        projectJurisdiction.Id, categoryQuestion.SchemeQuestionId, validateDto);
                                }
                                else
                                {
                                    var validateDto = Mapper.Map<ValidatedQuestionCreationDto>(categoryQuestion);
                                    validateDto.ValidatedBy = requestedUser.Id;
                                    validatedResult = await CreateValidatedQuestion(projectId,
                                        projectJurisdiction.Id, categoryQuestion.SchemeQuestionId, validateDto);
                                }


                                if (validatedResult is OkObjectResult okValidateResult)
                                {
                                    validatedQuestions.Add(
                                        okValidateResult.Value as ValidatedCategoryQuestionReturnDto);
                                }
                            }

                        }
                        else  // handle regular question
                        {
                            var codedQuestion = (CodedQuestionReturnDto) question; 
                            // check if the question has been validated
                            var existingValidatedQuestion =
                                await _unitOfWork.ValidatedQuestions.SingleOrDefaultAsync(vq =>
                                    vq.SchemeQuestionId == codedQuestion.SchemeQuestionId && vq.ProjectJurisdiction.Id == projectJurisdiction.Id);
                            if (existingValidatedQuestion != null)
                            {
                                var validateDto = Mapper.Map<ValidatedQuestionUpdateDto>(codedQuestion);
                                validateDto.ValidatedBy = requestedUser.Id;
                                validateDto.Id = existingValidatedQuestion.Id;  // override the id using existing;
                                //Console.WriteLine("category update data");
                                //Console.WriteLine(JsonConvert.SerializeObject(validateDto));
                                validatedResult = await UpdateValidatedQuestion(projectId,
                                    projectJurisdiction.Id, codedQuestion.SchemeQuestionId, validateDto);
                            }
                            else
                            {
                                var validateDto = Mapper.Map<ValidatedQuestionCreationDto>(codedQuestion);
                                validateDto.ValidatedBy = requestedUser.Id;
                                //Console.WriteLine("regular question update data");
                                //Console.WriteLine(JsonConvert.SerializeObject(validateDto));
                                validatedResult = await CreateValidatedQuestion(projectId,
                                    projectJurisdiction.Id, codedQuestion.SchemeQuestionId, validateDto);
                                
                            }
                            if (validatedResult is OkObjectResult okValidateResult)
                            {
                                validatedQuestions.Add(okValidateResult.Value as ValidatedQuestionReturnDto);
                            }
                           
                        }
                       
                    }

                    ;
                }
            }

            return Ok(validatedQuestions);
        }

    }
}