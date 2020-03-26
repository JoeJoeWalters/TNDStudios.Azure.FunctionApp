using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TNDStudios.Azure.FunctionApp.Mocking.Policies;

namespace TNDStudios.Azure.FunctionApp.Mocking.Mocks
{
    public class TestAsyncCollector<T> : IAsyncCollector<T>
    {
        /// <summary>
        /// The policy to use to raise errors etc. at certain points in the
        /// collection process
        /// </summary>
        private TestExceptionPolicy policy;

        /// <summary>
        /// List of pre-defined objects in the collector or a new array
        /// to show what was collected
        /// </summary>
        private IList<T> writtenItems;
        public IList<T> WrittenItems { get => writtenItems; }

        /// <summary>
        /// Matching logic to define whether we should apply merge logic
        /// to writes to the collector
        /// </summary>
        private Func<T, T, Boolean> matchLogic;
        private Boolean DefaultMatchLogic(T input, T compareTo) => false;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="initialisingList">The initialising list of items</param>
        /// <param name="matchLogic">Logic to tell if the collector should merge</param>
        public TestAsyncCollector(
                List<T> initialisingList, 
                Func<T, T, Boolean> matchLogic,
                TestExceptionPolicy policy
                )
        {
            // Assign the logic for item matching (if we should do merges on writing)
            this.matchLogic = (matchLogic == null) ? DefaultMatchLogic : matchLogic;

            // Assign the exception policy
            this.policy = policy ?? new TestExceptionPolicy() { };

            // Assign the initialised list to the incoming list (so we can share the object reference)
            this.writtenItems = initialisingList ?? new List<T>();
        }

        public Task AddAsync(T item, CancellationToken cancellationToken = default)
        {
            // Throw an error when writing according to the policy?
            if (policy.WriteException != null)
                throw policy.WriteException;

            // Check the match logic first to see if we need to apply merges etc.
            // Can't have a nullable T so added a second found flag instead of just
            // comparing foundItem == null
            T foundItem = default(T);
            Boolean found = false;

            // Loop the items in the stored list
            foreach (T compareTo in writtenItems)
            {
                // Did the match logic say these two items were the same?
                if (matchLogic(item, compareTo))
                {
                    foundItem = compareTo;
                    found = true;
                }
            }

            // Found a match? Set the pointer to the new object
            if (found)
                foundItem = item;
            else
                writtenItems.Add(item); // Not a match, add the item

            return Task.FromResult<Boolean>(true);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            // Throw an error when flushing according to the policy?
            if (policy.FlushException != null)
                throw policy.FlushException;

            writtenItems.Clear();
            return Task.FromResult<Boolean>(true);

        }
    }
}
