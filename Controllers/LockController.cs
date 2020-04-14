using System.Threading.Tasks;
using AutoMapper;
using Esquire.Data;
using Esquire.Models;
using Esquire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/locks")]
    [Authorize]
    public class LockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILockingService _lockingService;
        private ILogger<ExportController> _logger;

        public LockController(IUnitOfWork unitOfWork, ILockingService lockingService, ILogger<ExportController> logger)
        {
            _unitOfWork = unitOfWork;
            _lockingService = lockingService;
            _logger = logger;
        }


        /// <summary>
        /// Get user that locked the project protocol.
        /// </summary>
        /// <remarks>Get user that locked the project protocol.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        [HttpGet("protocol/projects/{projectId}", Name = "GetProjectProtocolLock")]
        public async Task<IActionResult> GetProtocolLock(long projectId)
        {

            // see if lock exists
            if (_lockingService.TryGetProtocolLock(projectId, out var userIdWithLock) == false)
                return NoContent();

            // lock does exist get user with lock 
            var userWithLock = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userIdWithLock);
            if (userWithLock == null)
            {
                // lock exists but can't load user, then delete lock
                _logger.LogError("Get Protocol Lock error: lock exists with unknown user id {Id}.", userIdWithLock);
                if (_lockingService.TryDeleteProtocolLock(projectId, userIdWithLock) == false)
                    return NotFound();
                return NoContent();

            }

            // got here if user has it locked and valid user, return user
            return Ok(Mapper.Map<UserReturnDto>(userWithLock));
              
        }

        /// <summary>
        /// Create a lock for the project protocol.
        /// </summary>
        /// <remarks>Create a lock for the project protocol for the specified user.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="userId">The ID of the user.</param>
        [HttpPost("protocol/projects/{projectId}/users/{userId}", Name = "CreateProjectProtocolLock")]
        public async Task<IActionResult> CreateProtocolLock(long projectId, long userId)
        {
            // try to create lock for requesting user
            var userIdWithLock = _lockingService.CreateProtocolLock(projectId, userId);
            var userWithLock = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userIdWithLock);
            if (userWithLock == null)
            {
                _logger.LogError("Create Protocol Lock error: lock exists with unknown user id {Id}.", userIdWithLock);
                return NotFound();
            }

            return Ok(Mapper.Map<UserReturnDto>(userWithLock));
        }

        /// <summary>
        /// Delete a lock for the project protocol.
        /// </summary>
        /// <remarks>Delete a lock for the project protocol for the specified user.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="userId">The ID of the user.</param>
        [HttpDelete("protocol/projects/{projectId}/users/{userId}", Name = "DeleteProjectProtocolLock")]
        public IActionResult DeleteProtocolLock(long projectId, long userId)
        {

            if (_lockingService.TryDeleteProtocolLock(projectId, userId) == false)
            {
                _logger.LogError("Delete Protocol Lock error: lock does not exist for user id {Id}.", userId);
               return NotFound();
            }
            return Ok();
        }

        /// <summary>
        /// Get user that locked the project scheme.
        /// </summary>
        /// <remarks>Get user that locked the project scheme.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        [HttpGet("scheme/projects/{projectId}", Name = "GetSchemeLock")]
        public async Task<IActionResult> GetSchemeLock(long projectId)
        {
            // see if lock exists
            if (_lockingService.TryGetSchemeLock(projectId, out var userIdWithLock) == false)
                return NoContent();

            // lock does exist get user with lock 
            var userWithLock = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userIdWithLock);
            if (userWithLock == null)
            {
                // lock exists but can't load user, then delete lock
                _logger.LogError("Get Scheme Lock error: lock exists with unknown user id {Id}.", userIdWithLock);
                if (_lockingService.TryDeleteSchemeLock(projectId, userIdWithLock) == false)
                    return NotFound();
                return NoContent();

            }
            
            // got here if user has it locked and valid user, return user
            return Ok(Mapper.Map<UserReturnDto>(userWithLock));
        }

        /// <summary>
        /// Create a lock for the project scheme.
        /// </summary>
        /// <remarks>Create a lock for the project protocol for the specified user.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="userId">The ID of the user.</param>
        [HttpPost("scheme/projects/{projectId}/users/{userId}", Name = "CreateSchemeLock")]
        public async Task<IActionResult> CreateSchemeLock(long projectId, long userId)
        {
            // try to create lock for requesting user
            var userIdWithLock = _lockingService.CreateSchemeLock(projectId, userId);
            var userWithLock = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == userIdWithLock);
            if (userWithLock == null)
            {
                _logger.LogError("Create Scheme Lock error: lock exists with unknown user id {Id}.", userIdWithLock);
                return NotFound();

            }
            return Ok(Mapper.Map<UserReturnDto>(userWithLock));
        }

        /// <summary>
        /// Delete a lock for the project scheme.
        /// </summary>
        /// <remarks>Delete a lock for the project scheme for the specified user.</remarks>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="userId">The ID of the user.</param>        
        [HttpDelete("scheme/projects/{projectId}/users/{userId}", Name = "DeleteSchemeLock")]
        public IActionResult DeleteSchemeLock(long projectId, long userId)
        {
            if (_lockingService.TryDeleteSchemeLock(projectId, userId) == false)
            {
                _logger.LogError("Delete Scheme Lock error: lock does not exist for user id {Id}.", userId);
                return NotFound();
            }
            return Ok();
        }
        
        
    }
}