using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class JurisdictionRepository : Repository<Jurisdiction>, IJurisdictionRepository
    {
        public JurisdictionRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<Jurisdiction>> GetAllAsync()
        {
            return Context.Jurisdictions.OrderBy(j => j.Name).ToListAsync();
        }

        public new Task<List<Jurisdiction>> FindAsync(Expression<Func<Jurisdiction, bool>> predicate)
        {
            return Context.Jurisdictions.OrderBy(j => j.Name).Where(predicate).ToListAsync();
        }
    }
}