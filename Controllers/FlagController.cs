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
    [Route("api/flags")]
    [Authorize]
    public class FlagController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FlagController(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Delete a Flag
        /// </summary>
        /// <remarks>
        /// Delete a Flag by Id.
        ///This route is used in the validation screen when a coordinator needs to clear (delete) a flag.
        /// </remarks>
        /// <param name="flagId">The ID of the flag.</param>
        [HttpDelete("{flagId}")]
        public async Task<IActionResult> DeleteFlagById(long flagId)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);
            if (!_unitOfWork.Users.CanUserEdit(user)) return Forbid();
            
            var flag = await _unitOfWork.FlagBases.SingleOrDefaultAsync(f => f.Id == flagId);
            if (flag == null) return NotFound();

            _unitOfWork.FlagBases.Remove(flag);

            return await _unitOfWork.Complete() <= 0 ? StatusCode(500) : NoContent();
        }

        /// <summary>
        /// Create a SchemeQuestionFlag
        /// </summary>
        /// <remarks>
        /// Create a SchemeQuestionFlag (Stop coding flag).
        /// This type of flag is visible to all users across all jurisdiction.
        /// </remarks>
        /// <param name="schemeQuestionId">The ID of the SchemeQuestion.</param>
        /// <param name="schemeQuestionFlagCreationDto">SchemeQuestionFlag object.</param>
        [HttpPost("schemequestionflag/{schemequestionid}")]
        public async Task<IActionResult> CreateSchemeQuestionFlag(long schemeQuestionId, [FromBody] SchemeQuestionFlagCreationDto schemeQuestionFlagCreationDto)
        {
            if (schemeQuestionFlagCreationDto == null)
            {
                return BadRequest();
            }

            if (schemeQuestionFlagCreationDto.Type != FlagType.Red)
            {
                ModelState.AddModelError("Invalid Flag type.", "Must be type red.");
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u =>
                u.Id == schemeQuestionFlagCreationDto.RaisedBy);
            if (user == null)
            {
                return BadRequest("No user matches the provided Id");
            }
            
            var schemeQuestion = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            if (schemeQuestion == null)
            {
                return NotFound();
            }


            var schemeQuestionFlag =
                schemeQuestion.Flags.FirstOrDefault(flag => flag.RaisedBy.Id == schemeQuestionFlagCreationDto.RaisedBy);

            if (schemeQuestionFlag == null)
            {
                schemeQuestionFlag = new SchemeQuestionFlag
                {
                    Type = FlagType.Red
                };
                
                schemeQuestion.Flags.Add(schemeQuestionFlag);
            }

            schemeQuestionFlag.Notes = schemeQuestionFlagCreationDto.Notes;
            schemeQuestionFlag.SetRaisedDetails(user);
            
            

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            return Ok(Mapper.Map<SchemeQuestionFlagReturnDto>(schemeQuestionFlag));
        }
    }
}