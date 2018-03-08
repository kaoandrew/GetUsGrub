﻿namespace GitGrub.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// The <c>ErrorHandler</c> class.
    /// Contains methods for performing error handling functions.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/07/2018
    /// </para>
    /// </summary>
    public class ErrorHandler
    {
        /// <summary>
        /// A GetGeneralError method.
        /// Returns a string for general errors that occur.
        /// <para>
        /// @author: Jennifer Nguyen
        /// @updated: 03/05/2018
        /// </para>
        /// </summary>
        public static string GetGeneralError()
        {
            return "Something went wrong. Please try again later.";
        }

        /// <summary>
        /// A GetGeneralError method.
        /// Returns a string for general errors that occur.
        /// <para>
        /// @author: Jennifer Nguyen
        /// @updated: 03/07/2018
        /// </para>
        /// </summary>
        public static string SetCustomError(string error)
        {
            return error;
        }
    }
}