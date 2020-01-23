using System;

namespace XOutput.Core.Exceptions
{
    public class SafeCallResult
    {
        public Exception Error { get; protected set; }

        public bool HasError => Error != null;

        protected SafeCallResult(Exception exception)
        {
            Error = exception;
        }

        public static SafeCallResult CreateSuccess()
        {
            return new SafeCallResult(null);
        }

        public static SafeCallResult CreateError(Exception exception)
        {
            return new SafeCallResult(exception);
        }
    }
    public sealed class SafeCallResult<T> : SafeCallResult
    {
        public T Result { get; private set; }

        private SafeCallResult(T result, Exception exception) : base(exception)
        {
            Result = result;
        }

        public static SafeCallResult<T> CreateSuccess(T result)
        {
            return new SafeCallResult<T>(result, null);
        }

        public new static SafeCallResult<T> CreateError(Exception exception)
        {
            return new SafeCallResult<T>(default(T), exception);
        }
    }
}
