using System.Collections.Generic;

namespace Esquire.Models
{
    /// <summary>
    /// CodedQuestionCreationDto - used to create a coded question
    /// </summary>
    public class CodedQuestionCreationDto
    {
        /// <summary>
        /// CodedQuestionFlag - this type of flag is only seen by the User and Coordinator and is only applicable to the jurisdiction and/or Tab for which it was created.
        /// </summary>
        public FlagCreationDto Flag { get; set; }
        
        /// <summary>
        /// Comment entered by the User.
        /// </summary>
        public string Comment { get; set; }
        
        /// <summary>
        /// Answers selected by the User.
        /// </summary>
        public ICollection<CodedAnswerCreationDto> CodedAnswers { get; set; } = new List<CodedAnswerCreationDto>();
        
        /// <summary>
        /// CategoryId - this is used for child questions of Tab type questions. The CategoryId is a SchemeAnswerId from the parent Tab question.
        /// </summary>
        public long? CategoryId { get; set; }
    }
}