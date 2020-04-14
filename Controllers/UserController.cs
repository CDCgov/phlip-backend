using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Esquire.Models;
using Esquire.Data;
using System.Diagnostics.Contracts;
using AutoMapper;
using esquire_backend.security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace esquire_backend.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public UserController(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="userPostDto">The User object to create.</param>
        /// <response code="200">User created</response>
        /// <response code="303">User with this email address already exists. Payload includes object for existing User.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="403">Requestor is not authorized to perform this operation.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserGetDto), 200)]
        [ProducesResponseType(typeof(UserGetDto), 303)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public async Task<IActionResult> AddUser([FromBody] UserPostDto userPostDto)
        {
            if (!await IsRequestorAdmin())
            {
                return Forbid();
            }

            if (userPostDto == null) return BadRequest();

            if (string.IsNullOrEmpty(userPostDto.Email))
            {
                ModelState.AddModelError("Email", "Email is required");
            }

            if (string.IsNullOrEmpty(userPostDto.FirstName))
            {
                ModelState.AddModelError("FirstName", "First name is required");
            }

            if (string.IsNullOrEmpty(userPostDto.LastName))
            {
                ModelState.AddModelError("LastName", "Last name is required");
            }

            if (string.IsNullOrEmpty(userPostDto.Role))
            {
                ModelState.AddModelError("Role", "Role is required");
            }


            var existingUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == userPostDto.Email);

            if (existingUser != null) return StatusCode(303, Mapper.Map<UserGetDto>(existingUser));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newUser = Mapper.Map<User>(userPostDto);

            try
            {
                _unitOfWork.Users.Add(newUser);
                await _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "add new user failed: '" + ex.Message + "-" + ex.InnerException
                }
                );

            }
            return Ok(Mapper.Map<UserGetDto>(newUser));



        }


        /// <summary>
        /// Authenticate User (Get User object with JWT)
        /// </summary>
        /// <remarks>In production, a valid JWT is required to use this route.
        /// In development mode, AllowAnonymous enables basic auth using an email address.</remarks>
        /// <param name="userLoginDto">The User to authenticate.</param>
        /// <response code="200">User authenticated.</response>
        /// <response code="401">Unauthorized. User either does not exist or has been deactivated.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserWithTokenGetDto), 200)]
        [ProducesResponseType(401)]
#if DEBUG
        /* AllowAnonymous in DEBUG mode only - this supports using basic auth only in non-production instances.
           Production instances will use an external auth provider.
           Following successful login with the external provider, a request is sent to this route (must contain valid JWT) 
           to retrieve the user object.
        */
        [AllowAnonymous]
#endif
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginDto userLoginDto)
        {
            Contract.Ensures(Contract.Result<OkObjectResult>() != null);
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == userLoginDto.Email);

            if (user == null) return Unauthorized();

            if (!user.IsActive)
                return StatusCode(401, new
                {
                    Message = "User has been deactivated",
                    user.IsActive
                });


            ////compare against database
            //// if match return a token
            //// if not, return invalid message
            //return JsonConvert.SerializeObject(user);

            var token = GenerateToken(user);

            var returnDto = Mapper.Map<UserWithTokenGetDto>(user);
            returnDto.Token = token;

            return Ok(returnDto);

        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <remarks>Default is a list of users.
        /// If a query string ?email='email address' is provided then this will search for that email address and
        /// return the result of the search.</remarks>
        /// <param name="email">The email address for the query</param>
        /// <param name="name">The name of the user for the query</param>
        /// <response code="200">A User object or List of User objects, depending on the state of the query string.</response>
        /// <response code="404">A query string was provided but no user matched the email address.</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserGetDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUsers(string email, string name)
        {
            //if (!await IsRequestorAdmin())
            if (!await AllowedUser())

            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email.Equals(email));

                    if (user == null) return NotFound();

                    var returnDto = Mapper.Map<UserGetDto>(user);

                    return Ok(returnDto);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e);
                    return StatusCode(500, "Multiple users matched the provided email address.");
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var matchedUsers = await _unitOfWork.Users.FindAsync(u => (u.LastName.Contains(name) || u.FirstName.Contains(name) || (u.FirstName + " " + u.LastName).Contains(name)));
                    Console.WriteLine(matchedUsers);
                    if (matchedUsers == null) return NotFound();
                    return Ok(Mapper.Map<IEnumerable<UserGetDto>>(matchedUsers));

                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e);
                    return StatusCode(500, e);
                }
            }

            var users = await _unitOfWork.Users.GetAllAsync();

            if (!users.Any())
            {
                return NoContent();
            }

            return Ok(Mapper.Map<IEnumerable<UserGetDto>>(users));
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="id">The id of the User to update</param>
        /// <param name="userPutDto">The updated User object</param>
        /// <response code="200">User updated.</response>
        /// <response code="304">No changes were required.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="403">Requestor is not authorized. Must be an admin.</response>
        /// <response code="404">User not found.</response>        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserGetDto), 200)]
        [ProducesResponseType(304)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserPutDto userPutDto)
        {
            if (!await IsRequestorAdmin())
            {
                return Forbid();
            }

            if (userPutDto == null) return BadRequest();

            if (string.IsNullOrEmpty(userPutDto.Email))
            {
                ModelState.AddModelError("Email", "Email is required");
            }

            if (string.IsNullOrEmpty(userPutDto.FirstName))
            {
                ModelState.AddModelError("FirstName", "First name is required");
            }

            if (string.IsNullOrEmpty(userPutDto.Role))
            {
                ModelState.AddModelError("Role", "Role is required");
            }


            var existingUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (existingUser == null) return NotFound();

            if (existingUser.Email != userPutDto.Email && await _unitOfWork.Users.Exists(u => u.Email == userPutDto.Email))
            {
                ModelState.AddModelError("Email", "The provided email address is already in use by another user.");
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            Mapper.Map(userPutDto, existingUser);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            return Ok(Mapper.Map<UserGetDto>(existingUser));
        }

        /// <summary>
        /// Patch User
        /// </summary>
        /// <remarks>Currently, this is only used by the UI for updating the Avatar field.</remarks>
        /// <param name="id">The id of the User to patch</param>
        /// <param name="userPatchDocument">The patch document</param>
        /// <response code="200">User updated.</response>
        /// <response code="304">No changes were required.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="403">Requestor not authorized. Must be admin user.</response>
        /// <response code="404">User not found.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(UserGetDto), 200)]
        [ProducesResponseType(304)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PatchUser(long id, [FromBody] JsonPatchDocument<UserPatchDto> userPatchDocument)
        {
            if (!await IsRequestorAdmin())
            {
                return Forbid();
            }

            if (userPatchDocument == null || userPatchDocument.Operations.Count < 1) return BadRequest();

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var userDto = Mapper.Map<UserPatchDto>(user);


            try
            {
                userPatchDocument.ApplyTo(userDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            Mapper.Map(userDto, user);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            return Ok(Mapper.Map<UserGetDto>(user));
        }

        /// <summary>
        /// Patch User
        /// </summary>
        /// <remarks>Currently, this is only used by the UI for updating the Avatar, first name, last name.</remarks>
        /// <param name="id">The id of the User to patch</param>
        /// <param name="userPatchDocument">The patch document</param>
        /// <response code="200">User updated.</response>
        /// <response code="304">No changes were required.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">User not found.</response>
        [HttpPatch("{id}/selfUpdate")]
        [ProducesResponseType(typeof(UserGetDto), 200)]
        [ProducesResponseType(304)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SelfPatchUser(long id, [FromBody] JsonPatchDocument<UserPatchDto> userPatchDocument)
        {
            //Console.WriteLine(JsonConvert.SerializeObject(userPatchDocument));

            if (userPatchDocument == null || userPatchDocument.Operations.Count < 1) return BadRequest();

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var userDto = Mapper.Map<UserPatchDto>(user);

            /*foreach (var oneOperation in userPatchDocument.Operations.ToList())
            {
                if (!new string[] {"email", "role", "isactive"}.Any(s =>
                    oneOperation.path.Contains(s, StringComparison.CurrentCultureIgnoreCase))) continue;
                Console.WriteLine("found invalid path" + oneOperation.path);
                userPatchDocument.Operations.Remove(oneOperation);
            }
            Console.WriteLine(JsonConvert.SerializeObject(userPatchDocument));*/
            try
            {

                userPatchDocument.ApplyTo(userDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            Mapper.Map(userDto, user);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            return Ok(Mapper.Map<UserGetDto>(user));
        }

        /// <summary>
        /// Get Avatar for User
        /// </summary>
        /// <param name="id">The id of the User</param>
        /// <response code="201">No avatar has been uploaded for this user.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{id}/avatar")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserAvatar(long id)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            if (user.Avatar == null) return NoContent();

            return Ok(user.Avatar);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <remarks>This is not used in the UI.
        /// This only works if the User does not have any foreign key relationships.</remarks>
        /// <param name="id">The id of the User</param>
        /// <response code="201">User deleted.</response>
        /// <response code="403">Requestor not authorized. Must be admin user.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Delete operation could not be performed.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);

            var requestor = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);

            if (!_unitOfWork.Users.IsAdmin(requestor)) return Forbid();

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            _unitOfWork.Users.Remove(user);
            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }
            return NoContent();
        }

        /// <summary>
        /// Helper method to determine if the requestor (user Id from JWT) is an admin user.
        /// </summary>
        /// <returns>bool</returns>
        private async Task<bool> IsRequestorAdmin()
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);

            return _unitOfWork.Users.IsAdmin(user);
        }

        private async Task<bool> AllowedUser()
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);

            return _unitOfWork.Users.CanUserEdit(user);
        }

        /// <summary>
        /// Helper method to generate a JWT
        /// </summary>
        /// <param name="user">The User being authenticated.</param>
        /// <returns>JWT</returns>
        private JwtToken GenerateToken(User user)
        {
            return new JwtTokenBuilder()
                .AddSecurityKey(JwtTokenOptions.Key)
                .AddSubject(JwtTokenOptions.Subject)
                .AddIssuer(JwtTokenOptions.Issuer)
                .AddAudience(JwtTokenOptions.Audience)
                .AddClaim("Email", user.Email)
                .AddClaim("Id", "" + user.Id)
                .AddExpiry(60)
                .Build();
        }
    }
}
