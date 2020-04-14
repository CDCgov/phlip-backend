namespace Esquire.Models
{
    /// <summary>
    /// SchemeQuestionFlag (Red flag/StopCodingFlag) - This type of flag needs to be visible to all Coders who encounter this question.
    /// Inherits from FlagBase which has fields common to all FlagTypes.
    /// </summary>
    public class SchemeQuestionFlag : FlagBase
    {
        /// <summary>
        /// SchemeQuestionId - the Id of the SchemeQuestion which this Flag is associated with.
        /// </summary>
        public long SchemeQuestionId { get; set; }
    }
}