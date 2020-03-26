using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace TNDStudios.Azure.FunctionApp.Mocking.Mocks
{
    /// <summary>
    /// Mocked up writer type to collect what was created by the function
    /// </summary>
    public class TestTextWriter : TextWriter
    {
        // Bound attributes from the Binder constructor
        public IList<Attribute> BoundAttributes { get; internal set; }
        public CancellationToken BoundCancellationToken { get; internal set; }

        public override Encoding Encoding => Encoding.UTF8; // Hardcode the encoding type (needed by the base)

        // Items that are written to by the mocked process and can be checked later
        // not done with NSubstitute for now as want to add more complex state based checks
        private List<String> writtenItems = new List<String>();
        public IReadOnlyList<String> WrittenItems { get => writtenItems; }

        // Override the collection of the writing of the text data
        public override void Write(String value) => writtenItems.Add(value);

        /// <summary>
        /// Constructors
        /// </summary>
        public TestTextWriter() : base() => Initialise(null, default);
        public TestTextWriter(IFormatProvider formatProvider) : base(formatProvider) => Initialise(null, default);
        public TestTextWriter(IFormatProvider formatProvider, Attribute[] attributes, CancellationToken cancellationToken = default) : base(formatProvider) => Initialise(attributes, cancellationToken);
        public TestTextWriter(Attribute[] attributes, CancellationToken cancellationToken = default) : base() => Initialise(attributes, cancellationToken);
        private void Initialise(Attribute[] attributes, CancellationToken cancellationToken = default)
        {
            BoundAttributes = (attributes != null) ? new List<Attribute>(attributes) : new List<Attribute>();
            BoundCancellationToken = cancellationToken;
        }
    }
}
