namespace MediatorLibrary
{
    /// <summary>
    /// Represents an error with a specific code and message.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class with the specified code and message.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; private set; }
    }

}
