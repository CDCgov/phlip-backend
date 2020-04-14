namespace Esquire.Models
{
    /// <summary>
    /// ValidatedQuestion - Validated Coded data.
    /// After coding, a Coordinator will validate the coded data. Validated data is used in the export
    /// and will make up the final data set
    /// </summary>
    public class ValidatedQuestion : ValidatedQuestionBase
    {
        /// <summary>
        /// ProjectJurisdiction - The ProjectJurisdiction to which the ValidatedQuestion applies
        /// </summary>
        public ProjectJurisdiction ProjectJurisdiction { get; set; }
    }
}