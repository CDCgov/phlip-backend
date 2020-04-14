using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// The CreationDto for a Jurisdiction
    /// This is not currently used by the UI
    /// </summary>
    public class JurisdictionCreationDto
    {
        /// <summary>
        /// Name - The name of the Jurisdiction
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// GnisCode - Geographic Names Information System  - uniquely identifies geographic areas.
        /// </summary>
        [Required]
        public string GnisCode { get; set; }
        
        /// <summary>
        /// FipsCode - Federal Information Processing Standard - uniquely identifies geographic areas.
        /// </summary>
        [Required]
        public string FipsCode { get; set; }
        
        //TODO: Remove this and retrieve this information from the JWT
        /// <summary>
        /// UserId - the User who is creating this object
        /// </summary>
        [Required]
        public long? UserId { get; set; }
        
        /// <summary>
        /// Tag - used to assign a Jurisdiction to a list which can be added in one action.
        /// Currently the only tag in use is "US States"
        /// </summary>
        public string Tag { get; set; }
    }
}