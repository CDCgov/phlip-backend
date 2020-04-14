using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esquire.Models
{
    public class User
    {   
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// FirstName - the User's first name.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// LastName - the User's last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Email - the User's email address. Required field.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Avatar - the User's avatar which is displayed in various places in the UI.
        /// </summary>
        public string Avatar { get; set; }
        
        //TODO: use enum
        /// <summary>
        /// Role - The Users role
        /// Coder - can only view Project details and Coding screen
        /// Coordinator - all rights of coder and can edit Project details and has access to Validation screen
        /// Admin - has all rights of Coordinator and can access User management
        /// </summary>
        public string Role { get;set; }

        /// <summary>
        /// IsActive - allows users to be deactivated
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// BookmarkedProjects - Projects the User has bookmarked
        /// </summary>
        public ICollection<Project> BookmarkedProjects { get; set; } = new List<Project>();
        
        /// <summary>
        /// LastEdited - Collection of Projects the User has edited
        /// This is a relationship required by Entity Framework as there are more than one Users mapped to a Project (CreatedBy, LastEditedBy)
        /// </summary>
        [InverseProperty("LastEditedBy")]
        public ICollection<Project> LastEdited { get; set; }
        
        /// <summary>
        /// Created - Collection of Projects the User has created
        /// This is a relationship required by Entity Framework as there are more than one Users mapped to a Project (CreatedBy, LastEditedBy)
        /// </summary>
        [InverseProperty("CreatedBy")]
        public ICollection<Project> Created { get; set; }

        /// <summary>
        /// Helper method to return the Users full name
        /// </summary>
        /// <returns>Full name</returns>
        public string GetFullName()
        {
            var fullName = FirstName;
            if (!string.IsNullOrEmpty(LastName)) fullName += " " + LastName;
            return fullName;
        }
    }
}
