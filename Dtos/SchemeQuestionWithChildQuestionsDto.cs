using System.Collections.Generic;

namespace Esquire.Models
{
    /// <summary>
    /// ReturnDto for child questions of a SchemeQuestion object
    /// This is not used by the UI
    /// </summary>
    public class SchemeQuestionWithChildQuestionsDto : SchemeQuestionReturnDto
    {
        /// <summary>
        /// ChildQuestions - a collection of child questions associated with a SchemeQuestion
        /// </summary>
        public List<SchemeQuestionWithChildQuestionsDto> ChildQuestions { get; set; } = new List<SchemeQuestionWithChildQuestionsDto>();
    }
}