using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Data;
using Esquire.Data.Repositories;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace esquire
{
    class ValidatedQuestionBaseRepository : Repository<ValidatedQuestionBase>, IValidatedQuestionBaseRepository
    {
        public ValidatedQuestionBaseRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<ValidatedQuestionBase>> GetAllAsync()
        {
            return Context.ValidatedQuestionBases
                .Include(vq => vq.ValidatedBy)
                .Include(vq => vq.CodedAnswers)
                .Include(vq => vq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .ToListAsync();
        }

        public new Task<List<ValidatedQuestionBase>> FindAsync(Expression<Func<ValidatedQuestionBase, bool>> predicate)
        {
            return Context.ValidatedQuestionBases
                .Include(vq => vq.ValidatedBy)
                .Include(vq => vq.CodedAnswers)
                .Include(vq => vq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).ToListAsync();
        }

        public new Task<ValidatedQuestionBase> FirstOrDefaultAsync(Expression<Func<ValidatedQuestionBase, bool>> predicate)
        {
            return Context.ValidatedQuestionBases
                .Include(vq => vq.ValidatedBy)
                .Include(vq => vq.CodedAnswers)
                .Include(vq => vq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<ValidatedQuestionBase> FirstAsync(Expression<Func<ValidatedQuestionBase, bool>> predicate)
        {
            return Context.ValidatedQuestionBases
                .Include(vq => vq.ValidatedBy)
                .Include(vq => vq.CodedAnswers)
                .Include(vq => vq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).FirstAsync();
        }

        public new Task<ValidatedQuestionBase> SingleOrDefaultAsync(Expression<Func<ValidatedQuestionBase, bool>> predicate)
        {
            return Context.ValidatedQuestionBases
                .Include(vq => vq.ValidatedBy)
                .Include(vq => vq.CodedAnswers)
                .Include(vq => vq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<ValidatedQuestionBase> SingleAsync(Expression<Func<ValidatedQuestionBase, bool>> predicate)
        {
            return Context.ValidatedQuestionBases
                .Include(vq => vq.ValidatedBy)
                .Include(vq => vq.CodedAnswers)
                .Include(vq => vq.Flag)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).SingleAsync();
        }
    }
}