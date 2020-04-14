using System.ComponentModel.DataAnnotations.Schema;

namespace Esquire.Models
{
    /// <summary>
    /// CodedQuestionFlag (Green and Yellow flags) This type of flag is visible to only the User who created it and the Coordinator.
    /// Inherits from FlagBase which has fields common to all FlagTypes.
    /// </summary>
    public class CodedQuestionFlag : FlagBase
    {
        /// <summary>
        /// CodedQuestion - The CodedQuestion which this flag is associated with.
        /// </summary>
        [ForeignKey("Id")]
        public CodedQuestionBase CodedQuestion { get; set; }
    }
}