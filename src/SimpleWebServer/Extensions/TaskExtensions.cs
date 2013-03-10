using System;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWebServer.Extensions
{
    public static class TaskExtensions
    {
        public static Task<T> Catch<T>(this Task<T> task, T valueOnError)
        {
            return Catch<T, Exception>(task, valueOnError, e => { });
        } 
        public static Task<T> Catch<T, TEx>(this Task<T> task, T valueOnError, Action<TEx> action) where TEx : Exception
        {
            return Catch(task, valueOnError, action, TaskScheduler.Default);
        }

        public static Task<T> Catch<T, TEx>(this Task<T> task, T valueOnError, Action<TEx> action, TaskScheduler scheduler) where TEx : Exception
        {
            return Catch<T, TEx>(task, tex =>
                {
                    action(tex);
                    return valueOnError;
                }, scheduler);
        }

        public static Task<T> Catch<T, TEx>(this Task<T> task, Func<TEx, T> func, TaskScheduler scheduler) where TEx : Exception
        {
            return task.ContinueWith(t => DoOnException(t.Exception, func, () => t.Result), scheduler);
        }

        private static T DoOnException<T, TEx>(AggregateException ex, Func<TEx, T> func, Func<T> otherwize) where TEx : Exception
        {
            var exception =
                ex == null
                    ? null
                    : ex.Flatten().InnerExceptions.OfType<TEx>().FirstOrDefault();
            return
                exception == null
                    ? otherwize()
                    : func(exception);

        }
    }
}