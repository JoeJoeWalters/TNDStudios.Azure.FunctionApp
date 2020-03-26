
using System;
using System.Collections.Generic;

namespace TNDStudios.Azure.FunctionApp.Security
{

    public class SecurityResult<T>
    {
        public Boolean Initialised { get; set; } = false;

        /// <summary>
        /// LIst of permissions derived from the token
        /// </summary>
        public List<T> Permissions { get; set; } = new List<T>();

        /// <summary>
        /// Does the current context have permissions of a given type
        /// </summary>
        /// <param name="value">The enum item of the pre-defined type</param>
        /// <returns>If the context has the permission</returns>
        public Boolean HasPermission(T value) => Initialised ? Permissions.Contains(value) : false;
    }
}