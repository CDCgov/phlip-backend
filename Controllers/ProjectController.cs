using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using esquire;
using Microsoft.AspNetCore.Mvc;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Authorization;

namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/projects")]
    [Authorize]
    public class ProjectController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public ProjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<User> GetRequestor()
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);
            return user;
        }

        /// <summary>
        /// Search for projects based on name
        /// </summary>
        /// <remarks>
        /// Get list of projects. If name= is sent as a query param, returns
        /// any project that has name partially matched.
        /// </remarks>
        /// <param name="name">The name of the project to return.</param>
        [HttpGet("search")]
        public async Task<IActionResult> SearchProjects(string name)
        {
            IEnumerable<Project> projectsFromDb;
            if (name?.Length > 0)
            {
                projectsFromDb = await _unitOfWork.Projects.FindAsync(p => p.Name.Contains(name));
            }
            else
            {
                projectsFromDb = Enumerable.Empty<Project>();
            }
            projectsFromDb = await FilterProject(projectsFromDb);
            Request.HttpContext.Response.Headers.Add("X-Total-Count", projectsFromDb.Count().ToString());

            return Ok(Mapper.Map<ICollection<ProjectReturnDto>>(projectsFromDb));
        }
        
        /// <summary>
        /// Get projects sorted by user requestor 
        /// </summary>
        /// <remarks>
        /// Gets a list of projects that have projects created by the user who
        /// sent the request listed first, followed by the rest.
        /// </remarks>
        /// <param name="userId">The id of the current user</param>
        /// <param name="count">The number of projects to return</param>
        [HttpGet("searchRecent")]
        public async Task<IActionResult> GetRecentProjects(long userId, int count)
        {
            IEnumerable<Project> projectsFromDb = await _unitOfWork.Projects.GetTopXProjectByUser(p => p.CreatedBy.Id == userId, count);
            projectsFromDb = await FilterProject(projectsFromDb);
            Request.HttpContext.Response.Headers.Add("X-Total-Count", projectsFromDb.Count().ToString());
            return Ok(Mapper.Map<ICollection<ProjectReturnDto>>(projectsFromDb));
        }

        /// <summary>
        /// Get list of projects
        /// </summary>
        /// <remarks>
        /// Get a list of projects. If name= is sent as a query param, returns
        /// a project matching that name if one is found. Default returns all
        /// projects.
        /// </remarks>
        /// <param name="name">The name of the project to return.</param>
        [HttpGet]
        public async Task<IActionResult> GetProjects(string name)
        {
            IEnumerable<Project> projectsFromDb = new List<Project>();
            if (name?.Length > 0)
            {
                var projectFromDb = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Name == name);
                if (projectFromDb == null)
                {
                    return NotFound();
                }

                projectsFromDb.Append(projectFromDb);
                projectsFromDb = await FilterProject(projectsFromDb);
                var projectToReturn = Mapper.Map<ProjectReturnDto>(projectsFromDb);
                
                return Ok(projectToReturn);
            }

            projectsFromDb = await _unitOfWork.Projects.GetAllAsync();
            projectsFromDb = await FilterProject(projectsFromDb);
            var projectsToReturn = Mapper.Map<IEnumerable<ProjectReturnDto>>(projectsFromDb);

            return Ok(projectsToReturn);
        }

        /// <summary>
        /// Get project using project ID
        /// </summary>
        /// <remarks>Get project whose ID matches ID sent in query.</remarks>
        /// <param name="id">The ID of the project to return.</param>
        [HttpGet("{id}", Name = "GetProjectById")]
        public async Task<IActionResult> GetById(long id)
        {
            var projectFromDb = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == id);
            if (projectFromDb == null)
            {
                return NotFound();
            }

            IEnumerable<Project> projectsFromDb = new List<Project>().Append(projectFromDb);
            projectsFromDb = await FilterProject(projectsFromDb);
            if (projectsFromDb.Count() == 0)
                return Forbid();
            var projectsToReturn = Mapper.Map<IEnumerable<ProjectReturnDto>>(projectsFromDb);
                     
            return Ok(projectsToReturn);
        }

        /// <summary>
        /// Create a project
        /// </summary>
        /// <remarks>Create a project.</remarks>
        /// <param name="projectToCreate">The project object to create.</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectCreationDto projectToCreate)
        {
            if (projectToCreate == null)
            {
                return BadRequest();
            }
            
            if (await _unitOfWork.Projects.Exists(p => p.Name == projectToCreate.Name))
            {
                ModelState.AddModelError("Name", "The provided name already exists");
            }

            if(!Enum.IsDefined(typeof(ProjectType), Enum.ToObject(typeof(ProjectType), projectToCreate.Type)))
            {
                ModelState.AddModelError("Type", "The provided type is not a valid project type.");
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == projectToCreate.UserId);

            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                return Forbid();
            }
            
            var project = Mapper.Map<Project>(projectToCreate);
            
            project.SetCreatedDetails(user);
            foreach (var userId in projectToCreate.Users)
            {
                try
                {
                    var newUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userId);
                    if (newUser != null)
                    {
                        project.AssignUser(newUser);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
               
            }

            project.Protocol = new Protocol
            {
                DateLastEdited = DateTime.UtcNow,
                LastEditedBy = user
            };
            
            _unitOfWork.Projects.Add(project);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            var projectToReturn = Mapper.Map<ProjectReturnDto>(project);

            return CreatedAtRoute("GetProjectById", new { id = project.Id }, projectToReturn);
        }

        /// <summary>
        /// Update a project
        /// </summary>
        /// <remarks> Update a project.</remarks>
        /// <param name="updatedProject">The updated project object.</param>
        /// <param name="id">The id of the project to update.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] ProjectUpdateDto updatedProject)
        {
            if (updatedProject == null)
            {
                return BadRequest();
            }

            var oldProject = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == id);
            if (oldProject == null)
            {
                return NotFound();
            }
            
            if (!oldProject.Name.Equals(updatedProject.Name))
            {   
                if(await _unitOfWork.Projects.Exists(p => p.Name == updatedProject.Name && p.Id != oldProject.Id))
                    ModelState.AddModelError("Name", "The provided name already exists");
            }

            if(!Enum.IsDefined(typeof(ProjectType), Enum.ToObject(typeof(ProjectType), updatedProject.Type)))
            {
                ModelState.AddModelError("Type", "The provided type is not a valid project type.");
            }
                       
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == updatedProject.UserId);
            
            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                return Forbid();
            }

            Mapper.Map(updatedProject, oldProject);
            oldProject.UpdateLastEditedDetails(user);
            try
            {
                var existingUser = oldProject.ProjectUsers;
                oldProject.ProjectUsers.Clear();
                foreach (var userId in updatedProject.Users)
                {
                        var newUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userId);
                        if (newUser != null) // new user
                        {
                            oldProject.AssignUser(newUser);
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }
            var projectToReturn = Mapper.Map<ProjectReturnDto>(oldProject); 
            return Ok(projectToReturn);
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        /// <remarks>
        /// Delete a project and all associated data. This can only be
        /// called by a user who is an 'Admin.'
        /// </remarks>
        /// <param name="id">The id of the project to delete.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);
            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                return Forbid();
            }
            
            var project = await _unitOfWork.Projects.GetProjectWithScheme(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            if (project.Status != (int) ProjectStatus.Active)
            {
                return Forbid();
            }
            var projectJurisdictions = await _unitOfWork.ProjectJurisdictions.FindAsync(p => p.Project.Id == id);

            var schemeQuestionTree = _unitOfWork.Schemes.GetSchemeQuestionsAsTree(project.Scheme);
            
            if (schemeQuestionTree != null && schemeQuestionTree.Count > 0)
            {
                var helperMethods = new HelperMethods(_unitOfWork);
                foreach (var questionWithChildQuestionsDto in schemeQuestionTree)
                {
                    await helperMethods.DeleteSchemeQuestionAndAllNestedQuestions(questionWithChildQuestionsDto);
                }
            }

            var protocol = await _unitOfWork.Protocols.SingleOrDefaultAsync(p => p.ProjectId == id);
            
            if (protocol != null) _unitOfWork.Protocols.Remove(protocol);

            _unitOfWork.ProjectJurisdictions.RemoveRange(projectJurisdictions);
            
            if(project.Scheme != null) _unitOfWork.Schemes.Remove(project.Scheme);
  
            _unitOfWork.Projects.Remove(project);

            return await _unitOfWork.Complete() <= 0 ? StatusCode(304) : new NoContentResult();
        }

        /// <summary>
        /// Assign a user to project
        /// </summary>
        /// <remarks>Assign the provided user to the provided project</remarks>
        /// <param name="id">The id of the project to update.</param>
        /// <param name="users">The id of the project to update.</param>
        [HttpPut("assignUser")]
        public async Task<IActionResult> AssignProjectUser(long id,[FromBody] List<int> users)
        {

            var errorUsers = new List<int>();
            var requestor = await GetRequestor();
            
            if (!_unitOfWork.Users.CanUserEdit(requestor))
            {
                return Forbid();
            }
            
            var existingProject = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == id);
            if (existingProject == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var userId in users)
            {
                try
                {
                    var newUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userId);
                    if (newUser != null)
                    {
                        existingProject.AssignUser(newUser);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    errorUsers.Add(userId);
                }
               
            }
            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            if (!errorUsers.Any())
            {
                return StatusCode(206, errorUsers);
            }
            else
            {
                return Ok(new
                {    
                    existingProject.ProjectUsers 
                }); 
            }
           
        }

        /// <summary>
        /// Remove a user from project
        /// </summary>
        /// <remarks>Remove the provided user from the provided project</remarks>
        /// <param name="id">The id of the project to update.</param>
        /// <param name="userId">The id of the user to be removed from the project.</param>
        [HttpPut("removeUser/{id}/{userId}")]
        public async Task<IActionResult> RemoveProjectUser(long id, long userId)
        {
            var errors = false;
            var requestor = await GetRequestor();
            
            if (!_unitOfWork.Users.CanUserEdit(requestor))
            {
                return Forbid();
            }
            var existingProject = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == id);
            if (existingProject == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var removeUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (removeUser != null)
            {
                try
                {
                    existingProject.RemoveUser(removeUser);
                    if (await _unitOfWork.Complete() <= 0)
                    {
                        return StatusCode(304);
                    }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    errors = true;
                }
                
            }

            if (!errors)
            {
                return Ok(new
                {
                    existingProject.ProjectUsers,
                    requestor.LastName,
                    removeUser
                });
            }
            else
            {
                return StatusCode(500);
            }
        }
        
        /// <summary>
        /// Filter projects based on role or associated user
        /// </summary>
        private async Task<IEnumerable<Project>> FilterProject(IEnumerable<Project> projects)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);
            var projectsFromDb = _unitOfWork.Users.IsAdmin(user) ? projects : projects.Where(p => p.ProjectUsers.Any(aUser => aUser.UserId == user.Id));
            return projectsFromDb;
        }
    }

}
