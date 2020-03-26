using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using TNDStudios.Azure.FunctionApp.Mocking.Extensions;
using TNDStudios.Azure.FunctionApp.Mocking.Mocks;
using TNDStudios.Azure.FunctionApp.Mocking.Policies;

namespace TNDStudios.Azure.FunctionApp.Mocking.Factories
{
    public static class TestCollectorFactory
    {
        /// <summary>
        /// Return a mocked Async collector for a given type which will collect
        /// the output for a given function of whatever type it may be
        /// </summary>
        /// <returns>A Mocked IAsyncCollector implementation</returns>
        public static TestAsyncCollector<T> CreateAsyncCollector<T>()
            => CreateAsyncCollector<T>(new List<T>() { });

        public static TestAsyncCollector<T> CreateAsyncCollector<T>(List<T> initialisingList)
            => CreateAsyncCollector<T>(initialisingList, new TestExceptionPolicy() { });

        public static TestAsyncCollector<T> CreateAsyncCollector<T>(List<T> initialisingList, TestExceptionPolicy policy)
            => new TestAsyncCollector<T>(initialisingList, GetMatchLogic<T>(), policy);

        /// <summary>
        /// Determine if the collector should perform merge / matching
        /// logic when the item is written and the item is identified 
        /// as one we have already got in the initialising array
        /// </summary>
        /// <typeparam name="T">The type of object the collector manages</typeparam>
        /// <returns>The logic to apply in the collector</returns>
        private static Func<T, T, Boolean> GetMatchLogic<T>()
        {
            switch (typeof(T).ShortName())
            {
                case "document":

                    return DocumentMatchLogic<T>;

                default:

                    return null;
            }
        }

        private static Boolean DocumentMatchLogic<T>(T input, T compareTo)
        {
            Document sourceDoc = (Document)Convert.ChangeType(input, typeof(Document));
            Document compareDoc = (Document)Convert.ChangeType(compareTo, typeof(Document));

            return (sourceDoc != null && compareDoc != null &&
                sourceDoc.Id == compareDoc.Id);
        }
    }
}
