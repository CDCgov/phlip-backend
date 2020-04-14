using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// CreationDto for a ValidatedQuestion object
    /// Inherits from CodedQuestionCreationDto which contains common fields.
    /// </summary>
    public class ValidatedQuestionCreationDto : CodedQuestionCreationDto
    {
        /// <summary>
        /// ValidatedBy - The User who created the ValidatedQuestion object.
        /// </summary>
        [Required]
        public long? ValidatedBy { get; set; }
    }
}