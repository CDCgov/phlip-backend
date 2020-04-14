namespace Esquire.Models
{
    /// <summary>
    /// ReturnDto for User object with Avatar
    /// Inherits from UserReturnDto.
    /// </summary>
    public class UserWithAvatarReturnDto : UserReturnDto
    {
        /// <summary>
        /// Avatar - the User's avatar which is displayed in various places in the UI.
        /// </summary>
        public string Avatar { get; set; }
    }
}