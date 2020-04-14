using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using esquire;
using Esquire.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Esquire.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private ProjectContext _context;

        public UnitOfWork(ProjectContext context)
        {
            _context = context;
            CodedAnswers = new CodedAnswerRepository(_context);
            CodedCategoryQuestions = new CodedCategoryQuestionRepository(_context);
            CodedQuestions = new CodedQuestionRepository(_context);
            CodedQuestionBases = new CodedQuestionBaseRepository(_context);
            CodedQuestionFlags = new CodedQuestionFlagRepository(_context);
            FlagBases = new FlagBaseRepository(_context);
            Jurisdictions = new JurisdictionRepository(_context);
            Projects = new ProjectRepository(_context);
            ProjectJurisdictions = new ProjectJurisdictionRepository(_context);
            Protocols = new ProtocolRepository(_context);
            Schemes = new SchemeRepository(_context);
            SchemeAnswers = new SchemeAnswerRepository(_context);
            SchemeQuestions = new SchemeQuestionRepository(_context);
            SchemeQuestionFlags = new SchemeQuestionFlagRepository(_context);
            Users = new UserRepository(_context);
            ValidatedQuestions = new ValidatedQuestionRepository(_context);
            ValidatedCategoryQuestions = new ValidatedCategoryQuestionRepository(_context);
            ValidatedQuestionBases = new ValidatedQuestionBaseRepository(_context);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_context == null) return;
            _context.Dispose();
            _context = null;
        }


        public ICodedAnswerRepository CodedAnswers { get; }
        public ICodedCategoryQuestionRepository CodedCategoryQuestions { get; }
        public ICodedQuestionRepository CodedQuestions { get; }
        public ICodedQuestionBaseRepository CodedQuestionBases { get; }
        public ICodedQuestionFlagRepository CodedQuestionFlags { get; }
        public IFlagBaseRepository FlagBases { get; }
        public IJurisdictionRepository Jurisdictions { get; }
        public IProjectRepository Projects { get; }
        public IProjectJurisdictionRepository ProjectJurisdictions { get; }
        public IProtocolRepository Protocols { get; }
        public ISchemeRepository Schemes { get; }
        public ISchemeAnswerRepository SchemeAnswers { get; }
        public ISchemeQuestionRepository SchemeQuestions { get; }
        public ISchemeQuestionFlagRepository SchemeQuestionFlags { get; }
        public IUserRepository Users { get; }
        public IValidatedCategoryQuestionRepository ValidatedCategoryQuestions { get; }
        public IValidatedQuestionRepository ValidatedQuestions { get; }
        public IValidatedQuestionBaseRepository ValidatedQuestionBases { get; }
        public Task<int> Complete()
        {
            return _context.SaveChangesAsync();        
        }

        public string DbInfo()
        {
            var currentCommit = string.Empty;
            var builtTime = string.Empty;
            var pipelineId = string.Empty;
            try
            {
                using (var sr = new StreamReader("CurrentCommit.txt"))
                {
                    currentCommit = sr.ReadLine();
                    var timestamp = double.Parse(sr.ReadLine().Split("=")[1].Trim());
                    var dateTime = new DateTime(1970,1,1,0,0,0,0);
                    builtTime = dateTime.AddSeconds(timestamp).ToString("MM/dd/yyyy HH:mm:ss");
                    pipelineId = sr.ReadLine().Split("=")[1];
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            var dbInfo = new
            {
                pipelineId,
                currentCommit,
                builtTime,
                databaseName = _context.Database.GetDbConnection().Database,
                appVersion =
                    $"{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}"
            };
            return JsonConvert.SerializeObject(dbInfo);
        }
    }
}