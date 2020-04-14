using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class ProjectJurisdictionRepository : Repository<ProjectJurisdiction>, IProjectJurisdictionRepository
    {
        public ProjectJurisdictionRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<ProjectJurisdiction>> GetAllAsync()
        {
            return Context.ProjectJurisdictions
                .Include(pj => pj.Jurisdiction)
                .OrderBy(pj => pj.Jurisdiction.Name).ThenBy(pj => pj.StartDate).ThenBy(pj => pj.EndDate)
                .ToListAsync();
        }

        public new Task<List<ProjectJurisdiction>> FindAsync(Expression<Func<ProjectJurisdiction, bool>> predicate)
        {
            return Context.ProjectJurisdictions
                .Include(pj => pj.Jurisdiction)
                .OrderBy(pj => pj.Jurisdiction.Name).ThenBy(pj => pj.StartDate).ThenBy(pj => pj.EndDate)
                .Where(predicate).ToListAsync();
        }

        public new Task<ProjectJurisdiction> FirstOrDefaultAsync(Expression<Func<ProjectJurisdiction, bool>> predicate)
        {
            return Context.ProjectJurisdictions
                .Include(pj => pj.Jurisdiction)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public new Task<ProjectJurisdiction> FirstAsync(Expression<Func<ProjectJurisdiction, bool>> predicate)
        {
            return Context.ProjectJurisdictions
                .Include(pj => pj.Jurisdiction)
                .Where(predicate).FirstAsync();
        }

        public new Task<ProjectJurisdiction> SingleOrDefaultAsync(Expression<Func<ProjectJurisdiction, bool>> predicate)
        {
            return Context.ProjectJurisdictions
                .Include(pj => pj.Jurisdiction)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public new Task<ProjectJurisdiction> SingleAsync(Expression<Func<ProjectJurisdiction, bool>> predicate)
        {
            return Context.ProjectJurisdictions
                .Include(pj => pj.Jurisdiction)
                .Where(predicate).SingleAsync();
        }
    }
}