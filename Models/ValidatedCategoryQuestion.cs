namespace Esquire.Models
{
    /// <summary>
    /// ValidatedCategoryQuestion - Validated data for the child of a Category (Tabbed) type question.
    /// Inherits from ValidatedQuestionBase which contains fields common to all types of ValidatedQuestions
    /// </summary>
    public class ValidatedCategoryQuestion : ValidatedQuestionBase
    {
        /// <summary>
        /// Category - The SchemeAnswer (tab) which applies to this ValidatedQuestion object.
        /// </summary>
        public SchemeAnswer Category { get; set; }
        
        /// <summary>
        /// ProjectJurisdiction - The ProjectJurisdiction to which the ValidatedQuestion applies
        /// </summary>
        public ProjectJurisdiction ProjectJurisdiction { get; set; }
    }
}