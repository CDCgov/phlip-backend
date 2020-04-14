namespace Esquire.Models
{
    /// <summary>
    /// ReturnDto for a ValidatedQuestion object
    /// Inherits from CodedQuestionReturnDto which contains common fields
    /// </summary>
    public class ValidatedQuestionReturnDto : CodedQuestionReturnDto
    {
        /// <summary>
        /// ValidatedBy - The User who validated this ValidatedQuestion object.
        /// </summary>
        public UserReturnDto ValidatedBy { get; set; }
    }
}