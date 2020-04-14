using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class ValidatedCategoryQuestionRepository : Repository<ValidatedCategoryQuestion>, IValidatedCategoryQuestionRepository
    {
        public ValidatedCategoryQuestionRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<ValidatedCategoryQuestion>> GetAllAsync()
        {
            return Context.ValidatedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .ToListAsync();
        }

        public new Task<List<ValidatedCategoryQuestion>> FindAsync(Expression<Func<ValidatedCategoryQuestion, bool>> predicate)
        {
            return Context.ValidatedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).ToListAsync();
        }

        public new Task<ValidatedCategoryQuestion> FirstOrDefaultAsync(Expression<Func<ValidatedCategoryQuestion, bool>> predicate)
        {
            return Context.ValidatedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<ValidatedCategoryQuestion> FirstAsync(Expression<Func<ValidatedCategoryQuestion, bool>> predicate)
        {
            return Context.ValidatedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).FirstAsync();
        }

        public new Task<ValidatedCategoryQuestion> SingleOrDefaultAsync(Expression<Func<ValidatedCategoryQuestion, bool>> predicate)
        {
            return Context.ValidatedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<ValidatedCategoryQuestion> SingleAsync(Expression<Func<ValidatedCategoryQuestion, bool>> predicate)
        {
            return Context.ValidatedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.ValidatedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).SingleAsync();
        }
    }
}