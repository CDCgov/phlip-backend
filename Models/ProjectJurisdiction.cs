using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esquire.Models
{
    /// <summary>
    /// ProjectJurisdiction is a join between a Project and a Jurisdiction.
    /// This is used for Coding/Validating - each question in the Scheme is answered for every ProjectJurisdiction.
    /// The ProjectJurisdiction has a Start and End date which tells the coder/coordinator the date range to use when coding/validating.
    /// </summary>
    public class ProjectJurisdiction
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Project - The Project associated with this ProjectJurisdiction
        /// </summary>
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public long ProjectId { get; set; }
        
        /// <summary>
        /// Jurisdiction - The Jurisdiction asscociated with this ProjectJurisdiction
        /// </summary>
        [ForeignKey("JurisdictionId")]
        public Jurisdiction Jurisdiction { get; set; }
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