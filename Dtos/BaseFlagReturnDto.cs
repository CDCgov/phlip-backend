using System;
using esquire;

namespace Esquire.Models
{
    /// <summary>
    /// Contains fields common to all types of Flags
    /// </summary>
    public class BaseFlagReturnDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Type - The type of Flag
        /// </summary>
        public FlagType Type { get; set; }
        
        /// <summary>
        /// Notes - the notes associated with this Flag
        /// </summary>
        public string Notes { get; set; }
        
        /// <summary>
        /// RaisedBy - the User who created this flag.
        /// </summary>
        public UserReturnDto RaisedBy { get; set; }
        
        /// <summary>
        /// RaisedAt - the Date when the flag was created.
        /// </summary>
        public DateTime RaisedAt { get; set; }
    }
}