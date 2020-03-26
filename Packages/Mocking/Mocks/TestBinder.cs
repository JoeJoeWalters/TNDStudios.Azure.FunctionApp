using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TNDStudios.Azure.FunctionApp.Mocking.Extensions;

namespace TNDStudios.Azure.FunctionApp.Mocking.Mocks
{
    public class TestBinder : Binder
    {
        private IList<TestTextWriter> textWriters;
        public IList<TestTextWriter> TextWriters { get => textWriters; }

        public TestBinder()
        {
            textWriters = new List<TestTextWriter>();
        }

        public override Task<TValue> BindAsync<TValue>(Attribute[] attributes, CancellationToken cancellationToken = default)
        {
            Object result = default(TValue);

            switch (typeof(TValue).ShortName())
            {
                case "textwriter":
                    TestTextWriter textWriter = new TestTextWriter(attributes, cancellationToken);
                    textWriters.Add(textWriter);
                    result = textWriter;
                    break;
            }

            return Task.FromResult((TValue)result);
        }
    }

}
