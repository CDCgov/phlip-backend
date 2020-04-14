using System;
using System.Collections.Generic;

namespace Esquire.Models
{
    /// <summary>
    /// ReturnDto for a Scheme (CodingScheme) object
    /// </summary>
    public class SchemeReturnDto
    {
        /// <summary>
        /// SchemeQuestions - the list of questions in the Scheme
        /// </summary>
        public ICollection<SchemeQuestionReturnDto> SchemeQuestions { get; set; }
        
        /// <summary>
        /// Outline - the heirarchical data associated with the list of SchemeQuestions.
        /// This allows the UI to display the questions in the correct order.
        /// </summary>
        public Object Outline { get; set; }
    }
}