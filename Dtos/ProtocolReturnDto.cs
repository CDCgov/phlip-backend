using System;

namespace Esquire.Models
{
    /// <summary>
    /// The ReturnDto for the Protocol
    /// </summary>
    public class ProtocolReturnDto
    {
        /// <summary>
        /// Text - The Protocol text
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// LastEditedBy - User who last edited the Protocol
        /// </summary>
        public UserReturnDto LastEditedBy { get; set; }
        
        /// <summary>
        /// DateLastEdited - time the protocol was last edited
        /// </summary>
        public DateTime DateLastEdited { get; set; }
    }
}