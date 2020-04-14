using System.ComponentModel.DataAnnotations;
using esquire;

namespace Esquire.Models
{
    public class FlagCreationDto
    {
        /// <summary>
        /// Type - Green, Yellow, Red map to
        /// </summary>
        [Required]
        [Range(1, 3)]
        public FlagType Type { get; set; }
        [Required]
        public string Notes { get; set; }
    }
}