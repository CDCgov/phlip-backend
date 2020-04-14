using Esquire.Models;

namespace Esquire.Data.Repositories
{
    public class SchemeQuestionFlagRepository : Repository<SchemeQuestionFlag>, ISchemeQuestionFlagRepository
    {
        public SchemeQuestionFlagRepository(ProjectContext context) : base(context)
        {
        }
    }
}