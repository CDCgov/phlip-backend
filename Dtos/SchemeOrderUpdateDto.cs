using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// UpdateDto for SchemeOrder (Outline)
    /// The SchemeOrd
    /// </summary>
    public class SchemeOrderUpdateDto
    {
        //TODO: Remove this and get the UserId from the JWT
        /// <summary>
        /// UserId - the User who edited the outline
        /// </summary>
        [Required]
        public long? UserId { get; set; }
        
        /// <summary>
        /// Outline - The collection of heirarchical data for the scheme.
        /// This is stringified and stored in the database to allow the UI to display the SchemeQuestions in the correct order.
        /// </summary>
        [Required]
        public Dictionary<long, OrderInfo> Outline { get; set; } 
    }
}