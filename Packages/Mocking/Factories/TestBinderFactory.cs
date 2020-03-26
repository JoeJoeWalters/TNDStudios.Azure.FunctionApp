using TNDStudios.Azure.FunctionApp.Mocking.Mocks;

namespace TNDStudios.Azure.FunctionApp.Mocking.Factories
{
    public static class TestBinderFactory
    {
        /// <summary>
        /// Create a basic mock for the actual class and not the interface
        /// due to Binder extending IBinder and not implementing it's own interface
        /// </summary>
        /// <returns>A overloaded Binder implementation</returns>
        public static TestBinder CreateBinder()
            => new TestBinder();
    }
}
