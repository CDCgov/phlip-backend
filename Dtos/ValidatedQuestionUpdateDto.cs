using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// UpdateDto for a ValidatedQuestion object
    /// Inherits from CodedQuestionUpdateDto which contains common fields.
    /// </summary>
    public class ValidatedQuestionUpdateDto : CodedQuestionUpdateDto
    {
        /// <summary>
        /// ValdiatedBy - The user who updated this ValidatedQuestion object.
        /// </summary>
        [Required]
        public long? ValidatedBy { get; set; }
    }
}