using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// CreationDto for a SchemeAnswer object.
    /// This is an object used in a collection of PossibleAnswers in a SchemeQuestion object.
    /// </summary>
    public class SchemeAnswerCreationDto
    {
        /// <summary>
        /// Id - The Id of the answer;
        /// </summary>
        public long? Id { get; set; }
        
        /// <summary>
        /// Text - The text of the answer.
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string Text { get; set; }
        
        /// <summary>
        /// Order - The position of the answer in a collection of answers
        /// </summary>
        [Required]
        public int? Order { get; set; }
    }
}