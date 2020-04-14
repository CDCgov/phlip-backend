namespace Esquire.Models
{
    /// <summary>
    /// LoginDto for a User
    /// This is used in the request to Authenticate a user.
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// Email - The User's email address.
        /// </summary>
        public string Email { get; set; }
    }
}