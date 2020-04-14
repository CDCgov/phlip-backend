using Esquire.Models;

namespace Esquire.Data.Repositories
{
    public class CodedAnswerRepository : Repository<CodedAnswer>, ICodedAnswerRepository
    {
        public CodedAnswerRepository(ProjectContext context) : base(context)
        {
        }
    }
}