namespace Esquire.Models
{
    /// <summary>
    /// CodedCategoryQuestion - The coded data entered by a User in response to the child of a Category (Tabbed) type question.
    /// </summary>
    public class CodedCategoryQuestion : CodedQuestionBase
    {
        /// <summary>
        /// Category - the Category (Tab) for which this CodedQuestion applies
        /// </summary>
        public SchemeAnswer Category { get; set; }
        
        /// <summary>
        /// ProjectJurisdiction - the ProjectJurisdiction for which the CodedQuestion applies
        /// </summary>
        public ProjectJurisdiction ProjectJurisdiction { get; set; }
        
        /// <summary>
        /// CodedBy - The User who created this CodedQuestion
        /// </summary>
        public User CodedBy { get; set; }
    }
}