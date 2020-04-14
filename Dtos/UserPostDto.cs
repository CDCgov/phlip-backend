using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// PostDto for creating a User object
    /// </summary>
    public class UserPostDto
    {
        /// <summary>
        /// FirstName - the User's first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        
        /// <summary>
        /// LastName - the User's last name.
        /// </summary>
        [Required]
        public string LastName { get; set; }
        
        /// <summary>
        /// Email - the User's email address. Required field. Must be unique
        /// </summary>
        [Required]
        public string Email { get; set; }
        
        /// <summary>
        /// Avatar - the User's avatar which is displayed in various places in the UI.
        /// </summary>
        public string Avatar { get; set; }
        
        //TODO: use enum
        /// <summary>
        /// Role - The Users role
        /// Coder - can only view Project details and Coding screen
        /// Coordinator - all rights of coder and can edit Project details and has access to Validation screen
        /// Admin - has all rights of Coordinator and can access User management
        /// </summary>
        [Required]
        public string Role { get;set; }

        /// <summary>
        /// IsActive - allows users to be deactivated
        /// </summary>
        [Required]
        public bool? IsActive { get; set; }
    }
}