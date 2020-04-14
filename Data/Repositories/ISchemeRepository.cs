using System.Collections.Generic;
using Esquire.Models;

namespace Esquire.Data.Repositories
{
    public interface ISchemeRepository : IRepository<Scheme>
    {
        List<SchemeQuestionWithChildQuestionsDto> GetSchemeQuestionsAsTree(Scheme scheme);
    }
}