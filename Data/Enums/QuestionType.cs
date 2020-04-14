namespace esquire
{
    public enum QuestionType
    {
        /// <summary>
        /// Binary - Yes or No / True or False
        /// </summary>
        Binary = 1,
        
        /// <summary>
        /// Tabbed
        /// This type of question creates tabs/categories which are used for answering child questions.
        /// When a User encounters a Tabbed type question while coding or validating, they are presented with the list of possible
        /// answers in checkbox form. The answers selected represent tabs which are then displayed for each child question.
        /// Each child question must be answered for every tab.
        /// Tabbed type questions can only have non-tabbed type child questions.
        /// </summary>
        Category = 2,
        
        /// <summary>
        /// Checkbox (multiselect)
        /// </summary>
        Checkbox = 3,
        
        /// <summary>
        /// MultipleChoice - (RadioButton - single select)
        /// </summary>
        MultipleChoice = 4,
        
        /// <summary>
        /// TextField - Freeform text
        /// </summary>
        TextField = 5
    }
}