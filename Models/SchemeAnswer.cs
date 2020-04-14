namespace Esquire.Models
{
    /// <summary>
    /// SchemeAnswer - an answer choice within a SchemeQuestion. This is used to display a possible answer for a question.
    /// </summary>
    public class SchemeAnswer
    {
        /// <summary>
        /// Id - Auto generated
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Text - the answer text
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Order - this sets the answer order within the question. This allows the answers to be reordered after creation.
        /// This is set via the UI.
        /// </summary>
        public int Order { get; set; }
    }
}