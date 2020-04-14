using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class SchemeQuestionRepository : Repository<SchemeQuestion>, ISchemeQuestionRepository
    {
        public SchemeQuestionRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<SchemeQuestion>> GetAllAsync()
        {
            return Context.SchemeQuestions
                .Include(s => s.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Include(s => s.PossibleAnswers)
                .ToListAsync();
        }

        public new Task<List<SchemeQuestion>> FindAsync(Expression<Func<SchemeQuestion, bool>> predicate)
        {
            return Context.SchemeQuestions
                .Include(s => s.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Include(s => s.PossibleAnswers)
                .Where(predicate).ToListAsync();
        }

        public new Task<SchemeQuestion> FirstOrDefaultAsync(Expression<Func<SchemeQuestion, bool>> predicate)
        {
            return Context.SchemeQuestions
                .Include(s => s.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Include(s => s.PossibleAnswers)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<SchemeQuestion> FirstAsync(Expression<Func<SchemeQuestion, bool>> predicate)
        {
            return Context.SchemeQuestions
                .Include(s => s.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Include(s => s.PossibleAnswers)
                .Where(predicate).FirstAsync();
        }

        public new Task<SchemeQuestion> SingleOrDefaultAsync(Expression<Func<SchemeQuestion, bool>> predicate)
        {
            return Context.SchemeQuestions
                .Include(s => s.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Include(s => s.PossibleAnswers)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<SchemeQuestion> SingleAsync(Expression<Func<SchemeQuestion, bool>> predicate)
        {
            return Context.SchemeQuestions
                .Include(s => s.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Include(s => s.PossibleAnswers)
                .Where(predicate).SingleAsync();
        }

        public SchemeQuestionWithChildQuestionsDto FindSchemeQuestionWithChildQuestionsDto(long schemeQuestionId,
            List<SchemeQuestionWithChildQuestionsDto> questionsToCheck)
        {
            if (questionsToCheck == null) return null;
            
            var questionMatch = questionsToCheck.SingleOrDefault(sq => sq.Id == schemeQuestionId);
            if (questionMatch != null) return questionMatch;

            foreach (var questionToCheck in questionsToCheck)
            {
                questionMatch = FindSchemeQuestionWithChildQuestionsDto(schemeQuestionId, questionToCheck.ChildQuestions);
                if (questionMatch != null) return questionMatch;
            }

            return null;
        }
    }
}