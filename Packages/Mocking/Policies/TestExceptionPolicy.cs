using System;

namespace TNDStudios.Azure.FunctionApp.Mocking.Policies
{
    /// <summary>
    /// Definition of how a test document client should behave
    /// </summary>
    public class TestExceptionPolicy
    {
        /// <summary>
        /// Exceptions to raise when actions happen
        /// </summary>
        public Exception ReadException { get; set; }
        public Exception WriteException { get; set; }
        public Exception FlushException { get; set; }
    }
}
