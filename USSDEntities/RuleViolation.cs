using System;


namespace USSD.Entities
{
    /// <summary>
    /// Provide violated business rules and corresponding error message
    /// </summary>
   public class RuleViolation
    {
        /// <summary>
        /// Error Message field
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// The name of the property that violate the business rule
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Constructor that expect the error message
        /// </summary>
        /// <param name="errorMessage">Error message that will be displayed</param>
        public RuleViolation(string errorMessage)
        {
            ErrorMessage = errorMessage;
            PropertyName = string.Empty;
        }
        /// <summary>
        /// Constructor that expect the error message and the affected property name
        /// </summary>
        /// <param name="errorMessage">Error message that will be displayed</param>
        /// <param name="propertyName">The name of the property that violate the business rule</param>

        public RuleViolation(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }

    }
}
