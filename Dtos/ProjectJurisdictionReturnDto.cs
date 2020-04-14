using System;

namespace Esquire.Models
{
    /// <summary>
    /// ReturnDto for a ProjectJurisdiction object
    /// </summary>
    public class ProjectJurisdictionReturnDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// The Jurisdiction name for this ProjectJurisdiction object
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// ProjectId - The Id of the Project associated with this ProjectJurisdiction
        /// </summary>
        public long ProjectId { get; set; }
        
        /// <summary>
        /// JurisdictionId - The Id of the Jurisdiction associated with this ProjectJurisdiction
        /// </summary>
        public long JurisdictionId { get; set; }
        
        /// <summary>
        /// StartDate (SegmentStartDate) - the lower bound of the date range to consider when coding/validating
        /// No earlier than 1850 and no later than 2050
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// EndDate (SegmentEndDate) - the upper bound of the date range to consider when coding/validating
        /// No earlier than 1850 and no later than 2050
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}