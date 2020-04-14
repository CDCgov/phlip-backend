using Esquire.Models;

namespace Esquire.Data.Repositories
{
    public class SchemeAnswerRepository : Repository<SchemeAnswer>, ISchemeAnswerRepository
    {
        public SchemeAnswerRepository(ProjectContext context) : base(context)
        {
        }
    }
}