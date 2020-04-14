namespace Esquire.Models
{
    /// <summary>
    /// CodedQuestion - the coded data entered by a User in response to a SchemeQuestion
    /// </summary>
    public class CodedQuestion : CodedQuestionBase
    {
        /// <summary>
        /// ProjectJurisdiction - the ProjectJurisdiction for which the CodedQuestion applies
        /// </summary>
        public ProjectJurisdiction ProjectJurisdiction { get; set; }
        
        /// <summary>
        /// CodedBy - The user that coded the question
        /// </summary>
        public User CodedBy { get; set; }
    }
}