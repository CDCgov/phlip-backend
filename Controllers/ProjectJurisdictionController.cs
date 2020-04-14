using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using esquire;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/projects/{projectId}/jurisdictions/")]
    [Authorize]
    public class ProjectJurisdictionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectJurisdictionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Get a list of jurisdictions in a project
        /// </summary>
        /// <remarks>Get list of ProjectJurisdcitions.</remarks>
        /// <param name="projectId">The id of the project whose project jurisdictions should be returned.</param>
        [HttpGet]
        public async Task<IActionResult> GetAll(long projectId)
        {
            if (!await _unitOfWork.Projects.Exists(p => p.Id == projectId))
            {
                return NotFound();
            }

            var projectJurisdictionsFromDb = await _unitOfWork.ProjectJurisdictions.FindAsync(pj => pj.ProjectId == projectId);

            var projectJurisdictionsToReturn =
                Mapper.Map<IEnumerable<ProjectJurisdictionReturnDto>>(projectJurisdictionsFromDb);

            var count = projectJurisdictionsToReturn.Count();
            
            Request.HttpContext.Response.Headers.Add("X-Total-Count", count.ToString());
            
            return Ok(projectJurisdictionsToReturn);
        }

        /// <summary>
        /// Get a jurisdiction in a project by project jurisdiction id
        /// </summary>
        /// <remarks>Get ProjectJurisdiction by Id.</remarks>
        /// <param name="projectId">The id of the project.</param>
        /// <param name="projectJurisdictionId">The id of the ProjectJurisdiction.</param>
        [HttpGet("{projectJurisdictionId}")]
        public async Task<IActionResult> GetById(long projectId, long projectJurisdictionId)
        {
            var projectJurisdictionMatch = await _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);
            
            if (projectJurisdictionMatch == null)
            {
                return NotFound();
            }

            var projectJurisdictionToReturn = Mapper.Map<ProjectJurisdictionReturnDto>(projectJurisdictionMatch);

            return Ok(projectJurisdictionToReturn);

        }

        /// <summary>
        /// Add a preset list of jurisdictions to a project
        /// </summary>
        /// <remarks>Create a ProjectJurisdictions for every jurisdiction in a preset list.</remarks>
        /// <param name="projectId">The id of the project.</param>
        /// <param name="projectJurisdictionFromPresetCreationDto">The projectJurisdiction FromPresetCreationDto.</param>
        [HttpPost("preset/")]
        public async Task<IActionResult> Create(long projectId,
            [FromBody] ProjectJurisdictionFromPresetCreationDto projectJurisdictionFromPresetCreationDto)
        {
            if (projectJurisdictionFromPresetCreationDto == null)
            {
                return BadRequest();
            }
            
            var tag = projectJurisdictionFromPresetCreationDto.Tag;

            if (tag == null || tag.Equals(""))
            {
                ModelState.AddModelError("Tag", "Tag is required");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == projectJurisdictionFromPresetCreationDto.UserId);
            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                return Forbid();
            }
                        
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }
            if (project.Status != (int) ProjectStatus.Active)
            {
                return Forbid();
            }

            var jurisdictionsWithTag = await _unitOfWork.Jurisdictions.FindAsync(j => j.Tag == tag);
            if (jurisdictionsWithTag.Count == 0)
            {
                return BadRequest("No Jurisdictions match the specified tag.");
            }

            var projectJurisdictionsToAdd = new List<ProjectJurisdiction>();

            foreach (var taggedJurisdiction in jurisdictionsWithTag)
            {
                var pj = Mapper.Map<ProjectJurisdiction>(projectJurisdictionFromPresetCreationDto);
                pj.Project = project;
                pj.Jurisdiction = taggedJurisdiction;
                projectJurisdictionsToAdd.Add(pj);
            }

            foreach (var pj in projectJurisdictionsToAdd)
            {
                project.ProjectJurisdictions.Add(pj);
            }

            project.UpdateLastEditedDetails(user);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }
            
            Request.HttpContext.Response.Headers.Add("X-Total-Count", projectJurisdictionsToAdd.Count.ToString());

            return Ok(Mapper.Map<ICollection<ProjectJurisdictionReturnDto>>(projectJurisdictionsToAdd
                .OrderBy(pj => pj.Jurisdiction.Name).ThenBy(pj => pj.StartDate).ThenBy(pj => pj.EndDate)));
        }

        /// <summary>
        /// Add a jurisdiction to a project
        /// </summary>
        /// <remarks>Create a ProjectJurisdiction.</remarks>
        /// <param name="projectId">The id of the project.</param>
        /// <param name="projectJurisdictionCreationDto">The projectJurisdictionCreationDto.</param>
        [HttpPost]
        public async Task<IActionResult> Create(long projectId, 
            [FromBody] ProjectJurisdictionCreationDto projectJurisdictionCreationDto)
        {
            if (projectJurisdictionCreationDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                        
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == projectJurisdictionCreationDto.UserId);
            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                return Forbid();
            }
            
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project.Status != (int) ProjectStatus.Active)
            {
                return Forbid();
            }
            var jurisdiction = await _unitOfWork.Jurisdictions.SingleOrDefaultAsync(j => j.Id == projectJurisdictionCreationDto.JurisdictionId);

            if (project == null || jurisdiction == null)
            {
                return NotFound();
            }

            var projectJurisdiction = Mapper.Map<ProjectJurisdiction>(projectJurisdictionCreationDto);
            projectJurisdiction.Project = project;
            projectJurisdiction.Jurisdiction = jurisdiction;
            
            project.ProjectJurisdictions.Add(projectJurisdiction);

            project.UpdateLastEditedDetails(user);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            var projectJurisdictionToReturn = Mapper.Map<ProjectJurisdictionReturnDto>(projectJurisdiction);

            return Ok(projectJurisdictionToReturn);
        }    
        
        /// <summary>
        /// Update a jurisdiction in a project
        /// </summary>
        /// <remarks>Update a ProjectJurisdiction.</remarks>
        /// <param name="projectId">The id of the project.</param>
        /// <param name="projectJurisdictionId">The id of the ProjectJurisdiction.</param>
        /// <param name="projectJurisdictionUpdateDto">The projectJurisdictionCreationDto.</param>
        [HttpPut("{projectJurisdictionId}")]
        public async Task<IActionResult> Update(long projectId, [FromBody] ProjectJurisdictionUpdateDto projectJurisdictionUpdateDto, long projectJurisdictionId)
        {
            if (projectJurisdictionUpdateDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == projectJurisdictionUpdateDto.UserId);
            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                return Unauthorized();
            }
            
            var projectJurisdictionMatch = await _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project.Status != (int) ProjectStatus.Active)
            {   // project locked no update allowed
                return StatusCode(403);  
            }
            if (projectJurisdictionMatch == null || project == null)
            {
                return NotFound();
            }
          
            Mapper.Map(projectJurisdictionUpdateDto, projectJurisdictionMatch);

            project.UpdateLastEditedDetails(user);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }
            
            var projectJurisdictionToReturn = Mapper.Map<ProjectJurisdictionReturnDto>(projectJurisdictionMatch);

            return Ok(projectJurisdictionToReturn);
        }

        [HttpDelete("{projectJurisdictionId}")]
        public async Task<IActionResult> Delete(long projectId, long projectJurisdictionId)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);
            if (!_unitOfWork.Users.CanUserEdit(user)) return Forbid();

            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project.Status != (int) ProjectStatus.Active)
            {   
                // project locked no update allowed
                return StatusCode(403);  
            }
            var projectJurisdiction = await _unitOfWork.ProjectJurisdictions.SingleOrDefaultAsync(pj => pj.Id == projectJurisdictionId);
            if (projectJurisdiction == null || project == null) return NotFound();
            
            var codedQuestions = await 
                _unitOfWork.CodedQuestions.FindAsync(cq => cq.ProjectJurisdiction.Id == projectJurisdictionId);
            var codedCategoryQuestions = await 
                _unitOfWork.CodedCategoryQuestions.FindAsync(cq => cq.ProjectJurisdiction.Id == projectJurisdictionId);
            var validatedQuestions = await 
                _unitOfWork.ValidatedQuestions.FindAsync(cq => cq.ProjectJurisdiction.Id == projectJurisdictionId);
            var validatedCategoryQuestions = await 
                _unitOfWork.ValidatedCategoryQuestions.FindAsync(cq => cq.ProjectJurisdiction.Id == projectJurisdictionId);

            var bases = new List<CodedQuestionBase>();
            
            bases.AddRange(codedQuestions);
            bases.AddRange(codedCategoryQuestions);
            bases.AddRange(validatedQuestions);
            bases.AddRange(validatedCategoryQuestions);

            foreach (var codedQuestionBase in bases)
            {
                _unitOfWork.CodedAnswers.RemoveRange(codedQuestionBase.CodedAnswers);
                if(codedQuestionBase.Flag != null) _unitOfWork.CodedQuestionFlags.Remove(codedQuestionBase.Flag);
            }
            
            _unitOfWork.ProjectJurisdictions.Remove(projectJurisdiction);
            project.UpdateLastEditedDetails(user);
            
            await _unitOfWork.Complete();
                
            return NoContent();

        }

    }
}