using System.Collections.Generic;
using Esquire.Models;

namespace Esquire.Data.Repositories
{
    public interface ISchemeQuestionRepository : IRepository<SchemeQuestion>
    {
        SchemeQuestionWithChildQuestionsDto FindSchemeQuestionWithChildQuestionsDto(long schemeQuestionId,
            List<SchemeQuestionWithChildQuestionsDto> questionsToCheck);
    }
}