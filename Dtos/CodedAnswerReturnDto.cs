using System.Collections.Generic;
namespace Esquire.Models
{
    /// <summary>
    /// The ReturnDto for a CodedAnswer
    /// </summary>
    public class CodedAnswerReturnDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// SchemeAnswerId - SchemeAnswer which this CodedAnswer is associated with
        /// </summary>
        public long SchemeAnswerId { get; set; }
        
        /// <summary>
        /// Pincite - The reason why the user selected this answer
        /// </summary>
        public string Pincite { get; set; }
        
        /// <summary>
        /// TextAnswer - FreeForm text answer used for TextType questions only.
        /// </summary>
        public string TextAnswer { get; set; }
        
        /// <summary>
        /// Annotations - Highlight of the sections
        /// </summary>
        public List<Annotation> Annotations { get; set; }
    }
}