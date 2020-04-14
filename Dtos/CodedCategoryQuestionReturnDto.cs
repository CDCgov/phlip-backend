namespace Esquire.Models
{
    /// <summary>
    /// The ReturnDto for a CodedCategoryQuestion
    /// Inherits from CodedQuestionReturnDto which contains fields common to all types of CodedQuestions
    /// </summary>
    public class CodedCategoryQuestionReturnDto : CodedQuestionReturnDto
    {
        /// <summary>
        /// CategoryId - the Id of the SchemeAnswer (Category/Tab) which this CodedCategoryQuestion is associated with.
        /// </summary>
        public long CategoryId { get; set; }
    }
}