using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// The UpdateDto for a Protocol
    /// </summary>
    public class ProtocolUpdateDto
    {
        //TODO: Remove this and get the UserId from the JWT
        /// <summary>
        /// UserId - the User who edited the Protocol
        /// </summary>
        [Required]
        public long? UserId { get; set; }
        
        /// <summary>
        /// Text - the Protocol text
        /// </summary>
        [Required]
        public string Text { get; set; }
    }
}