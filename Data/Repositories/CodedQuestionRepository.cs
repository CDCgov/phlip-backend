using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class CodedQuestionRepository : Repository<CodedQuestion>, ICodedQuestionRepository
    {
        public CodedQuestionRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<CodedQuestion>> GetAllAsync()
        {
            return Context.CodedQuestions
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .ToListAsync();
        }

        public new Task<CodedQuestion> SingleOrDefaultAsync(Expression<Func<CodedQuestion, bool>> predicate)
        {
            return Context.CodedQuestions
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<List<CodedQuestion>> FindAsync(Expression<Func<CodedQuestion, bool>> predicate)
        {
            return Context.CodedQuestions
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).ToListAsync();
        }

        public new Task<CodedQuestion> FirstOrDefaultAsync(Expression<Func<CodedQuestion, bool>> predicate)
        {
            return Context.CodedQuestions
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<CodedQuestion> FirstAsync(Expression<Func<CodedQuestion, bool>> predicate)
        {
            return Context.CodedQuestions
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).FirstAsync();
        }

        public new Task<CodedQuestion> SingleAsync(Expression<Func<CodedQuestion, bool>> predicate)
        {
            return Context.CodedQuestions
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).SingleAsync();
        }
    }
}