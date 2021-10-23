using System;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Exceptions
{
    public static class ExceptionHandler
    {
        public static SafeCallResult SafeCall(Action action)
        {
            return SafeCall(action, (IEnumerable<Type>)null);
        }

        public static SafeCallResult SafeCall(Action action, params Type[] exceptions)
        {
            return SafeCall(action, (IEnumerable<Type>)exceptions);
        }

        public static SafeCallResult SafeCall(Action action, IEnumerable<Type> exceptions)
        {
            try
            {
                action?.Invoke();
                return SafeCallResult.CreateSuccess();
            }
            catch (Exception e)
            {
                if (exceptions == null || exceptions.Any(t => t.IsAssignableFrom(e.GetType())))
                {
                    return SafeCallResult.CreateError(e);
                }
                throw;
            }
        }

        public static SafeCallResult<T> SafeCall<T>(Func<T> action)
        {
            return SafeCall(action, (IEnumerable<Type>)null);
        }

        public static SafeCallResult<T> SafeCall<T>(Func<T> action, params Type[] exceptions)
        {
            return SafeCall(action, (IEnumerable<Type>)exceptions);
        }

        public static SafeCallResult<T> SafeCall<T>(Func<T> action, IEnumerable<Type> exceptions)
        {
            try
            {
                T result = action == null ? default(T) : action.Invoke();
                return SafeCallResult<T>.CreateSuccess(result);
            }
            catch (Exception e)
            {
                if (exceptions == null || exceptions.Any(t => t.IsAssignableFrom(e.GetType())))
                {
                    return SafeCallResult<T>.CreateError(e);
                }
                throw;
            }
        }
    }
}
