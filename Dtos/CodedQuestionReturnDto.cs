using System.Collections.Generic;

namespace Esquire.Models
{
    /// <summary>
    /// The ReturnDto for a CodedQuestion
    /// </summary>
    public class CodedQuestionReturnDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// SchemeQuestionId - The SchemeQuestion which this CodedQuestion is associated with
        /// </summary>
        public long SchemeQuestionId { get; set; }
        
        /// <summary>
        /// Flag - a CodedQuestionFlag (Green or Yellow Flag)
        /// </summary>
        public CodedQuestionFlagReturnDto Flag { get; set; }
        
        /// <summary>
        /// Comment - a comment entered by the user
        /// </summary>
        public string Comment { get; set; }
        
        /// <summary>
        /// CodedAnswers - a collection of CodedAnswers the the user selected while coding.
        /// </summary>
        public ICollection<CodedAnswerReturnDto> CodedAnswers { get; set; }
        
        /// <summary>
        /// ProjectJurisdictionId - The project jurisdiction for this ValidatedQuestion object.
        /// </summary>
        public long ProjectJurisdictionId { get; set; }

    }
}