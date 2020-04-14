using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// CreationDto for a SchemeQuestionFlag (Red/Stop coding flag) object
    /// Inherits from FlagCreationDto which contains fields common to all flag types
    /// </summary>
    public class SchemeQuestionFlagCreationDto : FlagCreationDto
    {
        /// <summary>
        /// RaisedBy - The user who created the flag.
        /// </summary>
        [Required]
        public long RaisedBy { get; set; }
    }
}