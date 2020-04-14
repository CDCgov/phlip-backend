using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;

namespace Esquire.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserWithBookmarkedProjects(Expression<Func<User, bool>> predicate);
        bool CanUserEdit(User user);
        bool IsAdmin(User user);
    }
}