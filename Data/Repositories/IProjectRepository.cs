using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;

namespace Esquire.Data.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetProjectWithScheme(Expression<Func<Project, bool>> predicate);
        Task<Project> GetProjectWithProtocol(Expression<Func<Project, bool>> predicate);
        Task<List<Project>> GetTopXProjectByUser(Expression<Func<Project, bool>> predicate, int x);
        Task<Project> GetProjectScheme(Expression<Func<Project, bool>> predicate);
        Task<Project> GetProjectSchemeWithoutFlags(Expression<Func<Project, bool>> predicate);
    }
}