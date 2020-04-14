using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esquire.Models
{
    /// <summary>
    /// CodedQuestionBase - contains fields common to all types of CodedQuestions
    /// A CodedQuestion is the object that contains the User's response to a SchemeQuestion
    /// </summary>
    public abstract class CodedQuestionBase
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// SchemeQuestionId - The SchemeQuestion associated with this CodedQuestion
        /// </summary>
        [ForeignKey("SchemeQuestionId")]
        public SchemeQuestion SchemeQuestion { get; set; }
        public long SchemeQuestionId { get; set; }
       
        /// <summary>
        /// Flag - The flag object - only green and yellow flags are stored in the CodedQuestion
        /// </summary>
        public CodedQuestionFlag Flag { get; set; }
        
        /// <summary>
        /// CodedAnswers - a list of CodedAnswer objects which represent the answers chosen by the User
        /// </summary>
        public ICollection<CodedAnswer> CodedAnswers { get; set; } = new List<CodedAnswer>();
        
        /// <summary>
        /// Comment - The text entered by a User in the comment field.
        /// </summary>
        public string Comment { get; set; }

    }
}