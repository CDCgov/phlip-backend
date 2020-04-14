using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class ProtocolRepository : Repository<Protocol>, IProtocolRepository
    {
        public ProtocolRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<Protocol>> GetAllAsync()
        {
            return Context.Protocols
                .Include(p => p.LastEditedBy)
                .ToListAsync();
        }

        public new Task<List<Protocol>> FindAsync(Expression<Func<Protocol, bool>> predicate)
        {
            return Context.Protocols
                .Include(p => p.LastEditedBy)
                .Where(predicate).ToListAsync();
        }

        public new Task<Protocol> FirstOrDefaultAsync(Expression<Func<Protocol, bool>> predicate)
        {
            return Context.Protocols
                .Include(p => p.LastEditedBy)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<Protocol> FirstAsync(Expression<Func<Protocol, bool>> predicate)
        {
            return Context.Protocols
                .Include(p => p.LastEditedBy)
                .Where(predicate).FirstAsync();
        }

        public new Task<Protocol> SingleOrDefaultAsync(Expression<Func<Protocol, bool>> predicate)
        {
            return Context.Protocols
                .Include(p => p.LastEditedBy)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<Protocol> SingleAsync(Expression<Func<Protocol, bool>> predicate)
        {
            return Context.Protocols
                .Include(p => p.LastEditedBy)
                .Where(predicate).SingleAsync();
        }
    }
}