using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    public class CleanupCodedQuestionDto
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public long ProjectJurisdictionId { get; set; }
        [Required]
        public long SchemeQuestionId { get; set; }
        
        public List<long> Categories { get; set; } = new List<long>();
    }
}