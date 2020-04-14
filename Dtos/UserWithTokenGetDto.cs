namespace Esquire.Models
{
    /// <summary>
    /// GetDto for User object with JWT
    /// Inherits from UserGetDto
    /// This is used for responses to Authentication requests.
    /// </summary>
    public class UserWithTokenGetDto : UserGetDto
    {
        /// <summary>
        /// Token - The JWT that is generated for this user.
        /// </summary>
        public JwtToken Token { get; set; }
    }
}