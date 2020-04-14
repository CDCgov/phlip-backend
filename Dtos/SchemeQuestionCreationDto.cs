using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using esquire;

namespace Esquire.Models
{
    /// <summary>
    /// CreationDto for a SchemeQuestion object
    /// </summary>
    public class SchemeQuestionCreationDto
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
        
        /// <summary>
        /// IncludeComment - a boolean value which allows the Coordinator to whether or not to include a comment field with the question.
        /// </summary>
        [Required]
        public bool? IncludeComment { get; set; }
        
        /// <summary>
        /// ParentId - The Id of the parent question or 0 if this question is at the root of the scheme.
        /// </summary>
        [Required]
        public long? ParentId { get; set; }
        
        /// <summary>
        /// PositionInParent - The position within the parent question or Scheme if the question is at the root of the scheme.
        /// </summary>
        [Required]
        public long? PositionInParent { get; set; }
        
        //TODO: Remove this and get the UserId from the JWT
        /// <summary>
        /// UserId - the User who created the question
        /// </summary>
        [Required]
        public long? UserId { get; set; }
        
        /// <summary>
        /// Outline - The collection of heirarchical data for the scheme.
        /// This is updated with the data for new question then stringified and stored in the database to
        /// allow the UI to display the SchemeQuestions in the correct order.
        /// </summary>
        [Required]
        public Dictionary<long, OrderInfo> Outline { get; set; }
        
        /// <summary>
        /// IsCategoryQuestion - this is true for child questions of a Category (Tabbed) type question. This allows child questions
        /// of a Category (Tabbed) type question to be handled differently than other questions.
        /// </summary>
        [Required]
        public bool? IsCategoryQuestion { get; set; }
    }
}