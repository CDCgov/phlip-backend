using System;
using System.Collections.Generic;
using esquire;

namespace Esquire.Models
{
    /// <summary>
    /// The ReturnDto for a Project
    /// </summary>
    public class ProjectReturnDto
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Name - the Name of the project
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// DateLastEdited - time when the project was last edited
        /// </summary>
        public DateTime DateLastEdited { get; set; }
        
        /// <summary>
        /// DateCreated - time when the project was created
        /// </summary>
        public DateTime DateCreated { get; set; }
        
        /// <summary>
        /// LastEditedBy - User who last edited the project
        /// </summary>
        public string LastEditedBy { get; set; }
        
        /// <summary>
        /// CreatedBy - User who created the project
        /// </summary>
        public string CreatedBy { get; set; }
        
        /// <summary>
        /// CreatedById - Id of User who created the project
        /// </summary>
        public long CreatedById { get; set; }
        
        /// <summary>
        /// CreatedById - email of User who created the project
        /// </summary>
        public string CreatedByEmail { get; set; }
        
        /// <summary>
        /// Type - the ProjectType associated with this project.
        /// </summary>
        public ProjectType Type { get; set; }     
        
        /// <summary>
        /// ProjectJurisdictions - A collection of ProjectJurisdictions associated with this project.
        /// </summary>
        public ICollection<ProjectJurisdictionReturnDto> ProjectJurisdictions { get; set; }
        
        /// <summary>
        /// ProjectUsers - A collection of users associated with this project.
        /// </summary>
        public ICollection<ProjectUser> ProjectUsers { get; set; }
        
        /// <summary>
        /// project status - status of the project:  0=deleted;1=active;2=locked.
        /// </summary>
        public byte Status { get; set; }
    }
}