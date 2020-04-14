using System.Collections.Generic;
using esquire;

namespace Esquire.Models
{
    /// <summary>
    /// SchemeQuestion - contains all data needed to display a question.
    /// </summary>
    public class SchemeQuestion
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Text - the question text.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Hint - extra information to provide to a User to help them answer the question.
        /// </summary>
        public string Hint { get; set; }
        
        /// <summary>
        /// IncludeComment - a boolean value which allows the Coordinator to whether or not to include a comment field with the question.
        /// </summary>
        public bool IncludeComment { get; set; }
        
        /// <summary>
        /// PossibleAnswers - a collection of SchemeAnswers
        /// </summary>
        public ICollection<SchemeAnswer> PossibleAnswers { get; set; } = new List<SchemeAnswer>();
        
        /// <summary>
        /// QuestionType - the type of the question. This allows the Coordinator to set the format of the answers.
        /// </summary>
        public QuestionType QuestionType { get; set; }
        
        /// <summary>
        /// IsCategoryQuestion - this is true for child questions of a Category (Tabbed) type question. This allows child questions
        /// of a Category (Tabbed) type question to be handled differently than other questions.
        /// </summary>
        public bool IsCategoryQuestion { get; set; }
        
        /// <summary>
        /// SchemeQuestionFlag (Red flag/StopCodingFlag) - This type of flag needs to be visible to all Coders who encounter this question.
        /// </summary>
        public ICollection<SchemeQuestionFlag> Flags { get; set; } = new List<SchemeQuestionFlag>();
    }
}
