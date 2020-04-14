using System.Collections.Generic;

namespace Esquire.Models
{
    /// <summary>
    /// The Scheme (CodingScheme) is the general set of questions for a Project. This set of questions is answered for each ProjectJurisdiction.
    /// </summary>
    public class Scheme
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Questions - a collection of SchemeQuestions.
        /// </summary>
        public ICollection<SchemeQuestion> Questions { get; set; } = new List<SchemeQuestion>();
        
        /// <summary>
        /// QuestionNumbering - this is a Stringified JSON object that is used to store the heirarchy of the Scheme.
        /// This is used by the UI to display the questions in the correct order/heirarchy.
        /// This is updated whenever a new question is added or when questions are reordered.
        /// </summary>
        public string QuestionNumbering { get; set; }
    }
}