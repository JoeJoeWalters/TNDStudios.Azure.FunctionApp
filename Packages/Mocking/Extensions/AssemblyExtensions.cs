using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace TNDStudios.Azure.FunctionApp.Mocking.Helpers
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Load a stream of an embedded resource from a given assembly and decode it to a string 
        /// </summary>
        /// <param name="assembly">The assembly to search</param>
        /// <param name="loadFrom">The path in the assembly of the resource</param>
        /// <param name="encoding">The encoding to use to create the string</param>
        /// <returns>The stream encoded in the given string format</returns>
        public static String GetResourceString(this Assembly assembly, String loadFrom, Encoding encoding)
        {
            try
            {
                using (Stream resourceStream = assembly.GetManifestResourceStream(loadFrom))
                {
                    using (StreamReader sr = new StreamReader(resourceStream, encoding))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
