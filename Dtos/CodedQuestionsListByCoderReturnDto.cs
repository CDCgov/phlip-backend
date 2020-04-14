using System.Collections.Generic;

namespace Esquire.Models
{
    /// <summary>
    /// The ReturnDto for an object which contains a User and a list of CodedQuestions associated with that user.
    /// This is used to populate each card in the Validation screen.
    /// </summary>
    public class CodedQuestionsListByCoderReturnDto
    {
        /// <summary>
        /// Coder - The User who coded the set of CodedQuestions
        /// </summary>
        public UserReturnDto Coder { get; set; }
        
        /// <summary>
        /// CodedQuestions - the list of CodedQuestions (or CodedCategoryQuestions) which the User coded.
        /// </summary>
        public ICollection<object> CodedQuestions { get; set; } = new List<object>();
    }
}