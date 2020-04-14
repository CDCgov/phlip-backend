using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class FlagBaseRepository : Repository<FlagBase>, IFlagBaseRepository
    {
        public FlagBaseRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<FlagBase>> GetAllAsync()
        {
            return Context.FlagBases
                .Include(f => f.RaisedBy)
                .ToListAsync();
        }

        public new Task<List<FlagBase>> FindAsync(Expression<Func<FlagBase, bool>> predicate)
        {
            return Context.FlagBases
                .Include(f => f.RaisedBy)
                .Where(predicate).ToListAsync();
        }

        public new Task<FlagBase> FirstOrDefaultAsync(Expression<Func<FlagBase, bool>> predicate)
        {
            return Context.FlagBases
                .Include(f => f.RaisedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<FlagBase> FirstAsync(Expression<Func<FlagBase, bool>> predicate)
        {
            return Context.FlagBases
                .Include(f => f.RaisedBy)
                .Where(predicate).FirstAsync();
        }

        public new Task<FlagBase> SingleOrDefaultAsync(Expression<Func<FlagBase, bool>> predicate)
        {
            return Context.FlagBases
                .Include(f => f.RaisedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<FlagBase> SingleAsync(Expression<Func<FlagBase, bool>> predicate)
        {
            return Context.FlagBases
                .Include(f => f.RaisedBy)
                .Where(predicate).SingleAsync();
        }
    }
}