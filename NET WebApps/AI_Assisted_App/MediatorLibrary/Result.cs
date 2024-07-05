namespace MediatorLibrary
{
    /// <summary>
    /// Represents the result of an operation that can either be successful or contain an error.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response value.</typeparam>
    public sealed class Result<TResponse>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Gets the response value.
        /// </summary>
        public TResponse Value { get; private set; }

        /// <summary>
        /// Gets the error if the operation was unsuccessful.
        /// </summary>
        public Error Error { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TResponse}"/> class with an error.
        /// </summary>
        /// <param name="error">The error.</param>
        public Result(Error error)
        {
            Error = error;
            IsSuccess = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TResponse}"/> class with a response value.
        /// </summary>
        /// <param name="value">The response value.</param>
        public Result(TResponse value)
        {
            Value = value;
            IsSuccess = true;
        }
    }

}
