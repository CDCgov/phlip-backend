using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class CodedQuestionBaseRepository : Repository<CodedQuestionBase>, ICodedQuestionBaseRepository
    {
        public CodedQuestionBaseRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<CodedQuestionBase>> GetAllAsync()
        {
            return Context.CodedQuestionBases
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .ToListAsync();
        }

        public new Task<List<CodedQuestionBase>> FindAsync(Expression<Func<CodedQuestionBase, bool>> predicate)
        {
            return Context.CodedQuestionBases
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).ToListAsync();
        }

        public new Task<CodedQuestionBase> FirstOrDefaultAsync(Expression<Func<CodedQuestionBase, bool>> predicate)
        {
            return Context.CodedQuestionBases
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<CodedQuestionBase> FirstAsync(Expression<Func<CodedQuestionBase, bool>> predicate)
        {
            return Context.CodedQuestionBases
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).FirstAsync();
        }

        public new Task<CodedQuestionBase> SingleOrDefaultAsync(Expression<Func<CodedQuestionBase, bool>> predicate)
        {
            return Context.CodedQuestionBases
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<CodedQuestionBase> SingleAsync(Expression<Func<CodedQuestionBase, bool>> predicate)
        {
            return Context.CodedQuestionBases
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).SingleAsync();
        }
    }
}