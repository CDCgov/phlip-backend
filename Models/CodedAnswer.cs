using System.ComponentModel.DataAnnotations.Schema;

namespace Esquire.Models
{
    /// <summary>
    /// The coded data associated with a user answer
    /// </summary>
    public class CodedAnswer
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// SchemeAnswer - the SchemeAnswer that this CodedAnswer applies to
        /// </summary>
        [ForeignKey("SchemeAnswerId")]
        public SchemeAnswer SchemeAnswer { get; set; }
        public long SchemeAnswerId { get; set; }

        /// <summary>
        /// Pincite - the reason why the User selected this answer
        /// </summary>
        public string Pincite { get; set; }
        
        /// <summary>
        /// The User's answer in a free form text box - this is for TextType questions only.
        /// </summary>
        public string TextAnswer { get; set; }
        
        /// <summary>
        /// Annotations for the question
        /// </summary>
        public string Annotations { get; set; }
    }
}