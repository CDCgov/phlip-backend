using System;
using System.Threading.Tasks;
using esquire;
using Esquire.Data.Repositories;

namespace Esquire.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ICodedAnswerRepository CodedAnswers { get; }
        ICodedCategoryQuestionRepository CodedCategoryQuestions { get; }
        ICodedQuestionRepository CodedQuestions { get; }
        ICodedQuestionBaseRepository CodedQuestionBases { get; }
        ICodedQuestionFlagRepository CodedQuestionFlags { get; }
        IFlagBaseRepository FlagBases { get; }
        IJurisdictionRepository Jurisdictions { get; }
        IProjectRepository Projects { get; }
        IProjectJurisdictionRepository ProjectJurisdictions { get; }
        IProtocolRepository Protocols { get; }
        ISchemeRepository Schemes { get; }
        ISchemeAnswerRepository SchemeAnswers { get; }
        ISchemeQuestionRepository SchemeQuestions { get; }
        ISchemeQuestionFlagRepository SchemeQuestionFlags { get; }
        IUserRepository Users { get; }
        IValidatedCategoryQuestionRepository ValidatedCategoryQuestions { get; }
        IValidatedQuestionRepository ValidatedQuestions { get; }
        IValidatedQuestionBaseRepository ValidatedQuestionBases { get; }
        
        Task<int> Complete();
        string DbInfo();
    }
}