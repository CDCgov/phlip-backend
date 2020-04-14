using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Esquire.Models
{
    /// <summary>
    /// Used to create a CodedAnswer which is part of a set of CodedAnswers in a CodedQuestion object.
    /// </summary>
    public class CodedAnswerCreationDto
    {
        /// <summary>
        /// SchemeAnswerId - SchemeAnswer which this CodedAnswer is associated with
        /// </summary>
        [Required]
        public long? SchemeAnswerId { get; set; }
        
        /// <summary>
        /// Pincite - The reason why the user selected this answer
        /// </summary>
        public string Pincite { get; set; }
        
        /// <summary>
        /// TextAnswer - FreeForm text answer used for TextType questions only.
        /// </summary>
        public string TextAnswer { get; set; }
        
        /// <summary>
        /// Annotations for the question
        /// </summary>
        public List<Annotation>Annotations { get; set; }
    }
}