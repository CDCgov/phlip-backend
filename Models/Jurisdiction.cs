using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// Jurisdiction - a geographic area used for coding/validating.
    /// A Jurisdiction is used to code/validate multiple Projects via the ProjectJurisdiction join.
    /// </summary>
    public class Jurisdiction
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Concatenation of the GnisCode and FipsCode fields. This field is indexed and used to allow for updating
        /// the Name of a jurisdiction.
        /// </summary>
        public string GnisFipsConcatenation { get; set; }
        
        /// <summary>
        /// Name of the Jurisdiction
        /// </summary>
        [MaxLength(450)]
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
        
        /// <summary>
        /// List of ProjectJurisdictions which this Jurisdiction is associated with.
        /// </summary>
        public ICollection<ProjectJurisdiction> ProjectJurisdictions { get; set; } = new List<ProjectJurisdiction>();
    }
}