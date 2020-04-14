using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// The UpdateDto for a CodedQuestion
    /// </summary>
    public class CodedQuestionUpdateDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        [Required]
        public long? Id { get; set; }
        
        /// <summary>
        /// Flag - a CodedQuestionFlag created by a User.
        /// </summary>
        public FlagCreationDto Flag { get; set; }
        
        /// <summary>
        /// Comment - a comment entered by a User
        /// </summary>
        public string Comment { get; set; }
        
        /// <summary>
        /// CodedAnswers - a collection of CodedAnswers selected by the User
        /// </summary>
        public ICollection<CodedAnswerCreationDto> CodedAnswers { get; set; } = new List<CodedAnswerCreationDto>();
        
        /// <summary>
        /// CategoryId - Used for child questions of a Category (Tabbed) type question. The CategoryId is the SchemeAnswerId of the tab from the parent question
        /// </summary>
        public long? CategoryId { get; set; }
    }
}