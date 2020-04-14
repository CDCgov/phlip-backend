using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// This is the value in a key value pair which represents a SchemeQuestion and it's heirarchical position.
    /// e.g. 1: {parentId: 0, positionInParent: 0} would signify that SchemeQuestion 1 has no parent, and is at position 0 in the ouline.
    /// </summary>
    public class OrderInfo
    {
        /// <summary>
        /// parentId - the Id of a the parent question. If this question is at the root level, this will be 0.
        /// </summary>
        [Required]
        public long? parentId { get; set; }
        
        /// <summary>
        /// positionInParent - This signifies the heirarcy within a set of child questions for the specified parentId.
        /// If parentId is 0, this is the position within the set of root level questions.
        /// </summary>
        [Required]
        public long? positionInParent { get; set; }
    }
}