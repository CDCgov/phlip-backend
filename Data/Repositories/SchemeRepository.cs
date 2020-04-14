using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Esquire.Data.Repositories
{
    public class SchemeRepository : Repository<Scheme>, ISchemeRepository
    {
        public SchemeRepository(ProjectContext context) : base(context)
        {
        }

        // not in use
        public new Task<List<Scheme>> GetAllAsync()
        {
            return Context.Schemes
                .Include(s => s.Questions)
                .ThenInclude(q => q.PossibleAnswers)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Flags)
                .ThenInclude(f => f.RaisedBy)
                .ToListAsync();
        }

        // not in use
        public new Task<List<Scheme>> FindAsync(Expression<Func<Scheme, bool>> predicate)
        {
            return Context.Schemes
                .Include(s => s.Questions)
                .ThenInclude(q => q.PossibleAnswers)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).ToListAsync();
        }

        // not in use
        public new Task<Scheme> FirstOrDefaultAsync(Expression<Func<Scheme, bool>> predicate)
        {
            return Context.Schemes
                .Include(s => s.Questions)
                .ThenInclude(q => q.PossibleAnswers)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        // not in use
        public new Task<Scheme> FirstAsync(Expression<Func<Scheme, bool>> predicate)
        {
            return Context.Schemes
                .Include(s => s.Questions)
                .ThenInclude(q => q.PossibleAnswers)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).FirstAsync();
        }

        // not in use
        public new Task<Scheme> SingleOrDefaultAsync(Expression<Func<Scheme, bool>> predicate)
        {
            return Context.Schemes
                .Include(s => s.Questions)
                .ThenInclude(q => q.PossibleAnswers)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        // not in use
        public new Task<Scheme> SingleAsync(Expression<Func<Scheme, bool>> predicate)
        {
            return Context.Schemes
                .Include(s => s.Questions)
                .ThenInclude(q => q.PossibleAnswers)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Flags)
                .ThenInclude(f => f.RaisedBy)
                .Where(predicate).SingleAsync();
        }

        public List<SchemeQuestionWithChildQuestionsDto> GetSchemeQuestionsAsTree(Scheme scheme)
        {
            if (scheme == null || scheme.Questions.Count == 0 || scheme.QuestionNumbering == null)
            {
                return null;
            }

            var schemeQuestions = Mapper.Map<List<SchemeQuestionWithChildQuestionsDto>>(scheme.Questions);
            var outline = JsonConvert.DeserializeObject<Dictionary<long, OrderInfo>>(scheme.QuestionNumbering);

            var sortedOutline = from entry in outline
                orderby entry.Value.parentId, entry.Value.positionInParent
                select entry;

            foreach (var kvp in sortedOutline)
            {
                if (kvp.Value.parentId == 0) continue;

                var parent = schemeQuestions.SingleOrDefault(q => q.Id == kvp.Value.parentId);
                parent?.ChildQuestions.Add(schemeQuestions.SingleOrDefault(q => q.Id == kvp.Key));
            }

            return (from kvp in sortedOutline where kvp.Value.parentId == 0 select schemeQuestions.SingleOrDefault(q => q.Id == kvp.Key)).ToList();
        }
    }
}