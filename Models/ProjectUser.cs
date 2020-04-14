﻿using System.ComponentModel.DataAnnotations.Schema;

 namespace Esquire.Models
{
    /// <summary>
    /// Associated User object
    /// This is used to return the UserId, FirstName, and LastName in most places where a User is associated with some other object.
    /// </summary>
    public class ProjectUser
    {   
        /// <summary>
        /// Id - id of the row
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long RowId { get; set; }
        /// <summary>
        /// userId - id of user
        /// </summary>
        public long UserId { get; set; }
        
        /// <summary>
        /// FirstName - the User's first name.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// LastName - the User's last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Email - The user's email address
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Role - The Users role
        /// Coder - can only view Project details and Coding screen
        /// Coordinator - all rights of coder and can edit Project details and has access to Validation screen
        /// Admin - has all rights of Coordinator and can access User management
        /// </summary>
        public string Role { get;set; }
    }
}