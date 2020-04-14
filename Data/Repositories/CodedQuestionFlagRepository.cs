using Esquire.Models;

namespace Esquire.Data.Repositories
{
    public class CodedQuestionFlagRepository : Repository<CodedQuestionFlag>, ICodedQuestionFlagRepository
    {
        public CodedQuestionFlagRepository(ProjectContext context) : base(context)
        {
        }
    }
}