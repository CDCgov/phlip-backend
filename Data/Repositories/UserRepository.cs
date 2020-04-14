using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Esquire.Models;
using Microsoft.EntityFrameworkCore;

namespace Esquire.Data.Repositories
{
    class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ProjectContext context) : base(context)
        {
        }

        public Task<User> GetUserWithBookmarkedProjects(Expression<Func<User, bool>> predicate)
        {
            return Context.Users.Include(u => u.BookmarkedProjects).Where(predicate).SingleOrDefaultAsync();
        }

        public bool CanUserEdit(User user)
        {
            if (user == null)
            {
                return false;
            }
           
            return user.Role == "Coordinator" || user.Role == "Admin";
        }

        public bool IsAdmin(User user)
        {
            if (user == null) return false;
            
            return user.Role == "Admin";
        }
    }
}