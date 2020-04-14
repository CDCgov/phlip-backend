using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class ValidatedQuestionRepository : Repository<ValidatedQuestion>, IValidatedQuestionRepository
    {
        public ValidatedQuestionRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<ValidatedQuestion>> GetAllAsync()
        {
            return Context.ValidatedQuestions
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .ToListAsync();
        }

        public new Task<List<ValidatedQuestion>> FindAsync(Expression<Func<ValidatedQuestion, bool>> predicate)
        {
            return Context.ValidatedQuestions
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).ToListAsync();
        }

        public new Task<ValidatedQuestion> FirstOrDefaultAsync(Expression<Func<ValidatedQuestion, bool>> predicate)
        {
            return Context.ValidatedQuestions
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<ValidatedQuestion> FirstAsync(Expression<Func<ValidatedQuestion, bool>> predicate)
        {
            return Context.ValidatedQuestions
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).FirstAsync();
        }

        public new Task<ValidatedQuestion> SingleOrDefaultAsync(Expression<Func<ValidatedQuestion, bool>> predicate)
        {
            return Context.ValidatedQuestions
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<ValidatedQuestion> SingleAsync(Expression<Func<ValidatedQuestion, bool>> predicate)
        {
            return Context.ValidatedQuestions
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).SingleAsync();
        }
    }
}