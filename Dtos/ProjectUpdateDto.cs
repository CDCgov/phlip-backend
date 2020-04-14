﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using esquire;

namespace Esquire.Models
{
    public class ProjectUpdateDto
    {
        /// <summary>
        /// Name - the Name of the project - must be unique
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        //TODO: Remove this and get the UserId from the JWT
        /// <summary>
        /// UserId - the User who edited the Project
        /// </summary>
        [Required]
        public long? UserId { get; set; }
        
        /// <summary>
        /// Type - the ProjectType associated with this project.
        /// </summary>
        [Required]
        public ProjectType Type { get; set; }    
        /// <summary>
        /// user List - the list of user ids associated with this project.
        /// </summary>
        public List<int>Users { get; set; } = new List<int>();
        
        /// <summary>
        /// project status - status of the project:  0=deleted;1=active;2=locked.
        /// </summary>
        public byte Status { get; set; }
    }
}