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
    [Route("api/projects/{projectId}/protocol")]
    [Authorize]
    public class ProtocolController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProtocolController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Get the protocol for the specified project
        /// </summary>
        /// <remarks>Get the protocol for a specific project.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        [HttpGet(Name = "GetProtocol")]
        public async Task<IActionResult> GetProtocol(long projectId)
        {
            var protocol = await _unitOfWork.Protocols.SingleOrDefaultAsync(p => p.ProjectId == projectId);

            if (protocol == null) return NotFound();
            
            return Ok(Mapper.Map<ProtocolReturnDto>(protocol));
        }
        
        /// <summary>
        /// Update the protocol for the specified project
        /// </summary>
        /// <remarks>Update the protocol content for a specific project. Request is denied if project is locked.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="protocolUpdateDto">Updated protocol content, and user id of the user sending the request</param>
        [HttpPut(Name = "UpdateProtocol")]
        public async Task<IActionResult> UpdateProtocol(long projectId, [FromBody] ProtocolUpdateDto protocolUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == protocolUpdateDto.UserId);
            var project = await _unitOfWork.Projects.GetProjectWithProtocol(p => p.Id == projectId);
            if (project.Status != (int) ProjectStatus.Active)
            {   
                // project locked no update allowed
                return Forbid();  
            }

            project.Protocol.Text = protocolUpdateDto.Text;
            project.Protocol.UpdateLastEditedDetails(user);
            project.UpdateLastEditedDetails(user);
            
            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }
            
            return Ok();
        }
        
        
    }
}