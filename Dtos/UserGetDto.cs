namespace Esquire.Models
{
    /// <summary>
    /// GetDto for a User object
    /// This is used by the UI in the UserManagement screen.
    /// </summary>
    public class UserGetDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// FirstName - The user's first name
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// LastName - The user's last name
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Email - The user's email address
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Avatar - the user's avatar image which is dislayed in various places in the UI
        /// </summary>
        public string Avatar { get; set; }
        
        //TODO: use enum
        /// <summary>
        /// Role - The Users role
        /// Coder - can only view Project details and Coding screen
        /// Coordinator - all rights of coder and can edit Project details and has access to Validation screen
        /// Admin - has all rights of Coordinator and can access User management
        /// </summary>
        public string Role { get;set; }

        /// <summary>
        /// IsActive - allows users to be deactivated
        /// </summary>
        public bool IsActive { get; set; }
    }
}