namespace Esquire.Models
{
    /// <summary>
    /// ReturnDto for a ValidatedCategoryQuestion object
    /// Inherits from ValidatedQuestionReturnDto which contains fields common to all ValidatedQuestion types
    /// </summary>
    public class ValidatedCategoryQuestionReturnDto : ValidatedQuestionReturnDto
    {
        /// <summary>
        /// CategoryId - The Id of the SchemeAnswer (tab) which applies to this ValidatedQuestion object.
        /// </summary>
        public long CategoryId { get; set; }
    }
}