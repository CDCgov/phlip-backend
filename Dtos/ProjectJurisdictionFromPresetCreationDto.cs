using System;
using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// CreationDto for a list of ProjectJurisdiction objects created from a preset list of Jurisdictions
    /// </summary>
    public class ProjectJurisdictionFromPresetCreationDto
    {
        /// <summary>
        /// Tag - The Tag of a preset list of Jurisdictions
        /// </summary>
        [Required]
        public string Tag { get; set; }
        
        /// <summary>
        /// StartDate (SegmentStartDate) - the lower bound of the date range to consider when coding/validating
        /// No earlier than 1850 and no later than 2050
        /// </summary>
        [Required]
        [Range(typeof(DateTime), "01/01/1850", "12/31/2050", ErrorMessage = "Date is out of range")]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// EndDate (SegmentEndDate) - the upper bound of the date range to consider when coding/validating
        /// No earlier than 1850 and no later than 2050
        /// </summary>
        [Required]
        [Range(typeof(DateTime), "01/01/1850", "12/31/2050", ErrorMessage = "Date is out of range")]
        public DateTime EndDate { get; set; }
        
        //TODO: Remove this and get the UserId from the JWT
        /// <summary>
        /// UserId - the User who created the ProjectJurisdiction
        /// </summary>
        [Required]
        public long? UserId { get; set; }
    }
}