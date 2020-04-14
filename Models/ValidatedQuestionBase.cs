namespace Esquire.Models
{
    /// <summary>
    /// ValidatedQuestionBase - contains fields common to all ValidatedQuestion types
    /// Inherits from CodedQuestionBase which contains fields common to both Coded and Validated questions
    /// </summary>
    public abstract class ValidatedQuestionBase : CodedQuestionBase
    {
        /// <summary>
        /// ValidatedBy - the User who validated this question.
        /// </summary>
        public User ValidatedBy { get; set; }        
    }
}