using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using esquire;

namespace Esquire.Models
{
    /// <summary>
    /// UpdateDto for a SchemeQuestion object
    /// </summary>
    public class SchemeQuestionUpdateDto
    {
        /// <summary>
        /// Text - the question text.
        /// </summary>
        [Required]
        public string Text { get; set; }
        
        /// <summary>
        /// PossibleAnswers - a collection of SchemeAnswers
        /// </summary>
        public ICollection<SchemeAnswerCreationDto> PossibleAnswers { get; set; } = new List<SchemeAnswerCreationDto>();
        
        /// <summary>
        /// QuestionType - the type of the question. This allows the Coordinator to set the format of the answers.
        /// </summary>
        [Required]
        public QuestionType QuestionType { get; set; }
        
        /// <summary>
        /// Hint - extra information to provide to a User to help them answer the question.
        /// </summary>
        public string Hint { get; set; }
        
        //TODO: Remove this and get the UserId from the JWT
        /// <summary>
        /// UserId - the User who edited the question
        /// </summary>
        [Required]
        public long? UserId { get; set; }
        
        /// <summary>
        /// IncludeComment - a boolean value which allows the Coordinator to whether or not to include a comment field with the question.
        /// </summary>
        [Required]
        public bool? IncludeComment { get; set; }
        
        /// <summary>
        /// IsCategoryQuestion - this is true for child questions of a Category (Tabbed) type question. This allows child questions
        /// of a Category (Tabbed) type question to be handled differently than other questions.
        /// </summary>
        [Required]
        public bool? IsCategoryQuestion { get; set; }
    }
}