using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace esquire_backend.Controllers
{

    [Produces("application/json")]
    [Route("api/jurisdictions")]
    [Authorize]
    public class JurisdictionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public JurisdictionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get list of Jurisdictions
        /// </summary>
        /// <remarks>Get all Jurisdictions or all Jurisdictions that start with a given string</remarks>
        /// <param name="name">The name used to filter the list of Jurisdictions.</param>
        /// <param name="tag">The tag used to filter the list of Jurisdictions. </param>
        [HttpGet]
        public async Task<IActionResult> GetAll(string name, string tag)
        {

            IEnumerable<Jurisdiction> jurisdictionsFromDb;

            if (name?.Length > 0 && tag?.Length > 0)
                jurisdictionsFromDb = await _unitOfWork.Jurisdictions.FindAsync(j => j.Name.StartsWith(name) && j.Tag.Equals(tag));
            
            else if (name?.Length > 0)
            {
                jurisdictionsFromDb = 
                    await _unitOfWork.Jurisdictions.FindAsync(j => EF.Functions.Like(j.Name, "%" + name.Trim() + "%"));
            }
            else if (tag?.Length > 0)
                jurisdictionsFromDb = await _unitOfWork.Jurisdictions.FindAsync(j => j.Tag.Equals(tag));

            else jurisdictionsFromDb = await _unitOfWork.Jurisdictions.GetAllAsync();
            
            Request.HttpContext.Response.Headers.Add("X-Total-Count", jurisdictionsFromDb.Count().ToString());

            return Ok(Mapper.Map<ICollection<JurisdictionReturnDto>>(jurisdictionsFromDb.OrderByDescending(j=>j.Tag)));
        }

        /// <summary>
        /// Get Jurisdictions by id
        /// </summary>
        /// <remarks>Get Jurisdiction by id</remarks>
        /// <param name="id">The id of the jurisdiction.</param>
        [HttpGet("{id}", Name = "GetJurisdictionById")]
        public async Task<IActionResult> GetById(long id)
        {
            var jurisdiction = await _unitOfWork.Jurisdictions.SingleOrDefaultAsync(j => j.Id == id);
            if (jurisdiction == null)
            {
                return NotFound();
            }
            
            return Ok(Mapper.Map<JurisdictionReturnDto>(jurisdiction));
        }
        
        /// <summary>
        /// Create Jurisdiction
        /// </summary>
        /// <remarks>Create Jurisdiction</remarks>
        /// <param name="jurisdictionToCreate">The jurisdiction object to create.</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JurisdictionCreationDto jurisdictionToCreate)
        {
            if (jurisdictionToCreate == null)
            {
                return BadRequest();
            }

            var jurisdiction = Mapper.Map<Jurisdiction>(jurisdictionToCreate);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var gnisFipsConcatenation = jurisdictionToCreate.GnisCode + jurisdictionToCreate.FipsCode;

            var jurisdictionMatch = await _unitOfWork.Jurisdictions.SingleOrDefaultAsync(j =>
                j.GnisFipsConcatenation == gnisFipsConcatenation);

            if (jurisdictionMatch != null) return Ok(Mapper.Map<JurisdictionReturnDto>(jurisdictionMatch));

            jurisdiction.GnisFipsConcatenation = gnisFipsConcatenation;
            
            _unitOfWork.Jurisdictions.Add(jurisdiction);
            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            return CreatedAtRoute("GetJurisdictionById", new { id = jurisdiction.Id }, Mapper.Map<JurisdictionReturnDto>(jurisdiction));
        }

        /// <summary>
        /// Delete Jurisdiction
        /// </summary>
        /// <remarks>Delete Jurisdiction</remarks>
        /// <param name="id">The id of the jurisdiction.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);
            if (!_unitOfWork.Users.IsAdmin(user)) return Forbid();

            var jurisdiction = await _unitOfWork.Jurisdictions.SingleOrDefaultAsync(j => j.Id == id);
            if (jurisdiction == null) return NotFound();
            
            _unitOfWork.Jurisdictions.Remove(jurisdiction);

            await _unitOfWork.Complete();
                
            return NoContent();
        }

        /// <summary>
        /// Update Jurisdiction
        /// </summary>
        /// <remarks>Update Jurisdiction</remarks>
        /// <param name="id">The id of the jurisdiction.</param>
        /// <param name="jurisdictionDto">The updated jurisdiction object.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(long id, [FromBody] JurisdictionCreationDto jurisdictionDto)
        {
            if (jurisdictionDto == null) return BadRequest();
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var gnisFipsConcatenation = jurisdictionDto.GnisCode + jurisdictionDto.FipsCode;


            var jurisdiction = await _unitOfWork.Jurisdictions.SingleOrDefaultAsync(j => j.Id == id);
            if (jurisdiction == null) return NotFound();
           
            Mapper.Map(jurisdictionDto, jurisdiction);            

            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            jurisdiction.GnisFipsConcatenation = gnisFipsConcatenation;
            
            await _unitOfWork.Complete();
                
            return NoContent();
        }
    }
}