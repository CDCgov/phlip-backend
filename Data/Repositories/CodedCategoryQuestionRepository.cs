using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class CodedCategoryQuestionRepository : Repository<CodedCategoryQuestion>, ICodedCategoryQuestionRepository
    {
        public CodedCategoryQuestionRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<CodedCategoryQuestion>> GetAllAsync()
        {
            return Context.CodedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .ToListAsync();
        }

        public new Task<List<CodedCategoryQuestion>> FindAsync(Expression<Func<CodedCategoryQuestion, bool>> predicate)
        {
            return Context.CodedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).ToListAsync();
        }

        public new Task<CodedCategoryQuestion> FirstOrDefaultAsync(Expression<Func<CodedCategoryQuestion, bool>> predicate)
        {
            return Context.CodedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<CodedCategoryQuestion> FirstAsync(Expression<Func<CodedCategoryQuestion, bool>> predicate)
        {
            return Context.CodedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).FirstAsync();
        }

        public new Task<CodedCategoryQuestion> SingleOrDefaultAsync(Expression<Func<CodedCategoryQuestion, bool>> predicate)
        {
            return Context.CodedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<CodedCategoryQuestion> SingleAsync(Expression<Func<CodedCategoryQuestion, bool>> predicate)
        {
            return Context.CodedCategoryQuestions
                .Include(cq => cq.Category)
                .Include(cq => cq.CodedBy)
                .Include(cq => cq.CodedAnswers)
                .Include(cq => cq.Flag)
                .ThenInclude(cq => cq.RaisedBy)
                .Where(predicate).SingleAsync();
        }
    }
}