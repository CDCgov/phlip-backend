namespace Esquire.Models
{
    /// <summary>
    /// The ReturnDto for a Jurisdiction object
    /// </summary>
    public class JurisdictionReturnDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Name - The name of the Jurisdiction
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// GnisCode - Geographic Names Information System  - uniquely identifies geographic areas.
        /// </summary>
        public string GnisCode { get; set; }
        
        /// <summary>
        /// FipsCode - Federal Information Processing Standard - uniquely identifies geographic areas.
        /// </summary>
        public string FipsCode { get; set; }
        
        /// <summary>
        /// Tag - used to create a predefinied list of Jurisdictions to allow for adding multiple ProjectJurisdictions with one action.
        /// Currently the only tag in use is "US States"
        /// </summary>
        public string Tag { get; set; }
    }
}