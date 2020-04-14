using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Authorization;

namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/users")]
    [Authorize]
    public class BookmarksController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public BookmarksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Get bookmarked projects for a user
        /// </summary>
        /// <remarks>Get all the user bookmarked projects for the specified user.</remarks>
        /// <param name="userId">The ID of the user.</param>
        [HttpGet("{userId}/bookmarkedprojects", Name = "GetProjectBookmarks")]
        public async Task<IActionResult> GetProjectBookmarks(long userId)
        {
            var user = await _unitOfWork.Users.GetUserWithBookmarkedProjects(u => u.Id == userId);

            if (user == null) return NotFound();
            
            return Ok(Mapper.Map<IEnumerable<BookmarkedProjectReturnDto>>(user.BookmarkedProjects));
        }

        
        /// <summary>
        /// Add a bookmarked project for a user
        /// </summary>
        /// <remarks>Add a project bookmark for the specified user and specified project.</remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        [HttpPost("{userId}/bookmarkedprojects/{projectId}")]
        public async Task<IActionResult> BookmarkProject(long userId, long projectId)
        {
            var user = await _unitOfWork.Users.GetUserWithBookmarkedProjects(u => u.Id == userId);
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (user == null || project == null) return NotFound();

            user.BookmarkedProjects.Add(project);

            await _unitOfWork.Complete();

            var returnBookmark = new BookmarkedProjectReturnDto {ProjectId = projectId};

            return Ok(returnBookmark);

        }        
        
        /// <summary>
        /// Remove a project bookmark for a user
        /// </summary>
        /// <remarks>Remove the project bookmark for the specified user and specified project.</remarks>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="projectId">The ID of the project.</param>
        [HttpDelete("{userId}/bookmarkedprojects/{projectId}")]
        public async Task<IActionResult> DeleteProjectBookmark(int userId, int projectId)
        {
            var user = await _unitOfWork.Users.GetUserWithBookmarkedProjects(u => u.Id == userId);
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            
            if (user == null || project == null) return NotFound();

            user.BookmarkedProjects.Remove(project);
            
            await _unitOfWork.Complete();
            
            return NoContent();
        }
    }
}