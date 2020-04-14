namespace Esquire.Models
{
    /// <summary>
    /// ReturnDto for a User object
    /// This is used to return the Id, FirstName, and LastName in most places where a User is associated with some other object.
    /// </summary>
    public class UserReturnDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long UserId { get; set; }
        
        /// <summary>
        /// FirstName - the User's first name.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// LastName - the User's last name.
        /// </summary>
        public string LastName { get; set; }
    }
}