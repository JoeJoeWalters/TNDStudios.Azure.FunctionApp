using System;
using System.IO;
using System.Text;

namespace TNDStudios.Azure.FunctionApp.Mocking.Mocks
{
    public class TestStream : MemoryStream
    {
        public Encoding Encoding { get; set; }
        public String Content { get => Encoding.UTF8.GetString(base.ToArray()); }

        public TestStream() : base() => Initialise(Encoding.UTF8, String.Empty);
        public TestStream(Encoding encoding) : base() => Initialise(encoding, String.Empty);
        public TestStream(Encoding encoding, String content) : base() => Initialise(encoding, content);
        public void Initialise(Encoding encoding, String content)
        {
            this.Encoding = encoding;
            byte[] contentBytes = this.Encoding.GetBytes(content);
            base.Write(contentBytes, 0, contentBytes.Length);
        }
    }
}
