using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSytem.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a business rule is violated
    /// This exception is used to return structured error responses to the client
    /// </summary>
    public class BusinessException : Exception
    {
        /// <summary>
        /// Error code to identify the type of business rule violation
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Creates a new BusinessException with the specified error code and message
        /// </summary>
        /// <param name="errorCode">Error code to identify the type of violation</param>
        /// <param name="message">Human-readable error message</param>
        public BusinessException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Creates a new BusinessException with the specified error code, message, and inner exception
        /// </summary>
        /// <param name="errorCode">Error code to identify the type of violation</param>
        /// <param name="message">Human-readable error message</param>
        /// <param name="innerException">The inner exception that caused this exception</param>
        public BusinessException(string errorCode, string message, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}