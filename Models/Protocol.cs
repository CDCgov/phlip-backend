using System;

namespace Esquire.Models
{
    /// <summary>
    /// The Protocol allows Coders/Coordinators to view/edit contextual information about the Project.
    /// </summary>
    public class Protocol
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// ProjectId - The Id of the Project which this Protocol is associated with 
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// Text - the Protocol text - default template is hard coded in this field.
        /// </summary>
        public string Text { get; set; } =
            "<p>Research &amp; Coding Protocol for [Enter Project Title Here]</p>\n<p>Prepared by [enter your name or organization here]</p>\n<ol>\n<li>Research Protocol<br /><br />\n<ol>\n<li>Date protocol last updated: <br /><br /></li>\n<li>Statement on the scope of the research &amp; coding project: <br /><br /></li>\n<li>Project team members:<br /><br /></li>\n<li>Primary data collection<br /><br />\n<ol>\n<li>Dates research was conducted: Began XX/XX/XXXX, completed XX/XX/XXXX.<br /><br /></li>\n<li>Description of data collection methods<br /><br />\n<ol>\n<li>Databases used: <br /><br /></li>\n<li>Secondary sources used: <br /><br /></li>\n<li>Search terms and strings used: <br /><br /></li>\n<li>Jurisdictions included and/or excluded in project scope:<br /><br /></li>\n<li>Inclusion/exclusion criteria for laws:<br /><br /></li>\n<li>Process for ensuring laws valid, not superseded: <br /><br /></li>\n<li>Preserving relevant statutes and regulations: <br /><br /></li>\n</ol>\n</li>\n<li>Coding Protocol<br /><br />\n<ol>\n<li>Process for determining research questions and indicators: <br /><br /></li>\n<li>Coding rules<br /><br />\n<ol>\n<li>General rules applicable to all coding questions: <br /><br />\n<ol>\n<li>[Enter and describe coding rule(s) and any supporting rationale]: <br /><br /></li>\n</ol>\n</li>\n<li>Limited rules applicable to certain coding questions, responses, or jurisdictions: <br /><br />\n<ol>\n<li>[Enter and describe coding rule(s) and any supporting rationale]: <br /><br /></li>\n</ol>\n</li>\n</ol>\n</li>\n</ol>\n</li>\n</ol>\n</li>\n</ol>\n</li>\n</ol>";
        
        /// <summary>
        /// LastEditedBy - Updated whenever the Protocol is edited.
        /// </summary>
        public User LastEditedBy { get; set; }
        
        /// <summary>
        /// DateLastEdited - Updated whenever the Protocol is edited.
        /// </summary>
        public DateTime DateLastEdited { get; set; }

        /// <summary>
        /// Helper method to updated the LastEditedBy and DateLastEdited fields.
        /// </summary>
        /// <param name="user">The User who made the edit.</param>
        public void UpdateLastEditedDetails(User user)
        {
            LastEditedBy = user;
            DateLastEdited = DateTime.UtcNow;
        }
    }
}