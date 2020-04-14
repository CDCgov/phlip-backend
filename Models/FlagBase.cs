using System;
using esquire;

namespace Esquire.Models
{
    /// <summary>
    /// FlagBase contains fields common to all FlagTypes.
    /// </summary>
    public abstract class FlagBase
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Type - FlagType - Green, Yellow, Red. Green and Yellow are visible to the User and Coordinator only. Red is visible to all Users.
        /// </summary>
        public FlagType Type { get; set; }
        
        /// <summary>
        /// Notes - The notes entered by the User associated with this Flag
        /// </summary>
        public string Notes { get; set; }
        
        /// <summary>
        /// RaisedBy - The User who created the Flag
        /// </summary>
        public User RaisedBy { get; set; }
        
        /// <summary>
        /// RaisedAt - The Date when the Flag was created
        /// </summary>
        public DateTime RaisedAt { get; set; }

        /// <summary>
        /// Helper method to set RaisedAt and RaisedBy
        /// </summary>
        /// <param name="user">The User who created the Flag</param>
        public void SetRaisedDetails(User user)
        {
            RaisedAt = DateTime.UtcNow;
            RaisedBy = user;
        }
    }
}