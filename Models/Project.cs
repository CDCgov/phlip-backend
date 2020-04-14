﻿using System;
using System.Collections.Generic;
using System.Linq;
using esquire;
using AutoMapper;

namespace Esquire.Models
{
    public class Project
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Name - must be unique
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Type - Allows coordinator to specify the type of Project. Currenltly there are no functional differences between the types.
        /// </summary>
        public ProjectType Type { get; set; }
                
        /// <summary>
        /// List of ProjectJurisdictions associated with this Project.
        /// </summary>
        public ICollection<ProjectJurisdiction> ProjectJurisdictions { get; set; } = new List<ProjectJurisdiction>();
        
        /// <summary>
        /// Scheme (CodingScheme) for the project.
        /// </summary>
        public Scheme Scheme { get; set; } = new Scheme();

        /// <summary>
        /// DateLastEdited - This is updated any time anything related to the Project is updated.
        /// </summary>
        public DateTime DateLastEdited { get; set; }
        
        /// <summary>
        /// DateCreated - set when the project is created.
        /// </summary>
        public DateTime DateCreated { get; set; }
        
        /// <summary>
        /// LastEditedBy - This is updated any time anything related to the Project is updated.
        /// </summary>
        public User LastEditedBy { get; set; }
        
        /// <summary>
        /// CreatedBy - set when the project is created.
        /// </summary>
        public User CreatedBy { get; set; }
        
        /// <summary>
        /// Protocol for the project
        /// </summary>
        public Protocol Protocol { get; set; }

        /// <summary>
        /// Helper method to update the LastEditedBy and DateLastEdited fields.
        /// </summary>
        /// <param name="user">The User who made the edit</param>
        public void UpdateLastEditedDetails(User user)
        {
            LastEditedBy = user;
            DateLastEdited = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Helper method to initialize DateCreated, CreatedBy, LastEditedBy, and DateLastEdited properties
        /// </summary>
        /// <param name="user">The User who created the project.</param>
        public void SetCreatedDetails(User user)
        {
            var date = DateTime.UtcNow;
            
            CreatedBy = user;
            DateCreated = date;
            LastEditedBy = user;
            DateLastEdited = date;
            if (ProjectUsers.Any(aUser => aUser.UserId == user.Id)) return;
            var associatedUser = Mapper.Map<ProjectUser>(user);
            ProjectUsers.Add(associatedUser);
        }
        
        /// <summary>
        /// Helper method to add a user to project users list
        /// </summary>
        /// <param name="user">The User who will be assigned to the project.</param>
        public void AssignUser(User user)
        {
            if (ProjectUsers.Any(aUser => aUser.UserId == user.Id)) return;
            var associatedUser = Mapper.Map<ProjectUser>(user);
            ProjectUsers.Add(associatedUser);
        }
        
        /// <summary>
        /// Helper method to remove a user from project users list
        /// </summary>
        /// <param name="user">The User who will be removed from the project.</param>
        public void RemoveUser(User user)
        {
            try
            {
                var associatedUser = Mapper.Map<ProjectUser>(user);
                ProjectUsers.Remove(associatedUser);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        /// <summary>
        /// List of users that touched the project
        /// </summary>
        public ICollection<ProjectUser> ProjectUsers{ get; set; } = new List<ProjectUser>();
        
        /// <summary>
        /// Status flag  :  0 - deleted ; 1 - unlocked / active ; 2 - locked
        /// </summary>
        public byte Status { get; set; }
    }
}
