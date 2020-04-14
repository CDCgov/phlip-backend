using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ProjectContext context) : base(context)
        {
        }

        public new Task<List<Project>> GetAllAsync()
        {
            var projects = Context.Projects;
            projects.Include(p => p.ProjectUsers).Load();
            projects.Include(p => p.LastEditedBy).Include(p => p.CreatedBy).Load();
            projects.Include(p => p.ProjectJurisdictions).ThenInclude(pj => pj.Jurisdiction).Select(p => p.ProjectJurisdictions).Load();

            return projects.ToListAsync();
        }

        public new Task<List<Project>> FindAsync(Expression<Func<Project, bool>> predicate)
        {
            var projects = Context.Projects.Where(predicate);
            projects.Include(p => p.ProjectUsers).Load();
            projects.Include(p => p.LastEditedBy).Include(p => p.CreatedBy).Load();
            projects.Include(p => p.ProjectJurisdictions).ThenInclude(pj => pj.Jurisdiction).Select(p => p.ProjectJurisdictions).Load();
            
            return projects.ToListAsync();
        }

        // not currently in use
        public new Task<Project> FirstOrDefaultAsync(Expression<Func<Project, bool>> predicate)
        {
            var project = Context.Projects.Where(predicate);
            project.Include(p => p.ProjectUsers).Load();
            project.Include(p => p.LastEditedBy).Include(p => p.CreatedBy).Load();
            project.Include(p => p.ProjectJurisdictions).ThenInclude(pj => pj.Jurisdiction).Select(p => p.ProjectJurisdictions).Load();

            return project.FirstOrDefaultAsync();
        }

        // not currently in use
        public new Task<Project> FirstAsync(Expression<Func<Project, bool>> predicate)
        {
            var project = Context.Projects.Where(predicate);
            project.Include(p => p.ProjectUsers).Load();
            project.Include(p => p.LastEditedBy).Include(p => p.CreatedBy).Load();
            project.Include(p => p.ProjectJurisdictions).ThenInclude(pj => pj.Jurisdiction).Select(p => p.ProjectJurisdictions).Load();

            return project.FirstAsync();
        }

        // not currently in use
        public new Task<Project> SingleAsync(Expression<Func<Project, bool>> predicate)
        {
            var project = Context.Projects.Where(predicate);
            project.Include(p => p.ProjectUsers).Load();
            project.Include(p => p.LastEditedBy).Include(p => p.CreatedBy).Load();
            project.Include(p => p.ProjectJurisdictions).ThenInclude(pj => pj.Jurisdiction).Select(p => p.ProjectJurisdictions).Load();

            return project.SingleAsync();
        }

        public new Task<Project> SingleOrDefaultAsync(Expression<Func<Project, bool>> predicate)
        {
            var project = Context.Projects.Where(predicate);
            project.Include(p => p.ProjectUsers).Load();
            project.Include(p => p.LastEditedBy).Include(p => p.CreatedBy).Load();
            project.Include(p => p.ProjectJurisdictions).ThenInclude(pj => pj.Jurisdiction).Select(p => p.ProjectJurisdictions).Load();
            
            return project.SingleOrDefaultAsync();
        }

        public Task<Project> GetProjectWithScheme(Expression<Func<Project, bool>> predicate)
        {
            var project = Context.Projects.Where(predicate);
            project.Include(p => p.ProjectUsers).Load();
            project.Include(p => p.LastEditedBy).Include(p => p.CreatedBy).Load();
            project.Include(p => p.ProjectJurisdictions).ThenInclude(pj => pj.Jurisdiction).Select(p => p.ProjectJurisdictions).Load();
            project
                .Include(p => p.Scheme).ThenInclude(s => s.Questions).ThenInclude(q => q.PossibleAnswers)
                .Include(p => p.Scheme).ThenInclude(s => s.Questions).ThenInclude(q => q.Flags).ThenInclude(f => f.RaisedBy)
                .Load();

            return project.SingleOrDefaultAsync();
        }

        public Task<Project> GetProjectScheme(Expression<Func<Project, bool>> predicate)
        {
            var project = Context.Projects.Where(predicate);
            project
                .Include(p => p.Scheme).ThenInclude(s => s.Questions).ThenInclude(q => q.PossibleAnswers)
                .Include(p => p.Scheme).ThenInclude(s => s.Questions).ThenInclude(q => q.Flags).ThenInclude(f => f.RaisedBy)
                .Load();

            return project.SingleOrDefaultAsync();
        }

        public Task<Project> GetProjectSchemeWithoutFlags(Expression<Func<Project, bool>> predicate)
        {
            var project = Context.Projects.Where(predicate);
            project.Include(p => p.Scheme).ThenInclude(s => s.Questions).ThenInclude(q => q.PossibleAnswers).Load();
            
            return project.SingleOrDefaultAsync();
        }

        public Task<Project> GetProjectWithProtocol(Expression<Func<Project, bool>> predicate)
        {
            return Context.Projects
                .Include(p => p.Protocol)
                .Include(p => p.LastEditedBy)
                .Include(p => p.ProjectUsers)
                .Where(predicate).SingleOrDefaultAsync();
        }

        public Task<List<Project>> GetTopXProjectByUser(Expression<Func<Project, bool>> predicate, int count)
        {
            var projects = Context.Projects;
            projects.Include(p => p.ProjectUsers).Load();
            projects.Include(p => p.LastEditedBy).Include(p => p.CreatedBy).Load();
            projects.Include(p => p.ProjectJurisdictions).ThenInclude(pj => pj.Jurisdiction).Select(p => p.ProjectJurisdictions).Load();

            return projects
                .OrderByDescending(predicate)
                .ThenByDescending(p => p.DateCreated)
                .ThenBy(p => p.Name)
                .Take(count).ToListAsync();
        }
    }
}