using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionApp1
{
    public class FunctionSecurityBase
    {
        public Boolean HasPermission(String permission)
        {
            return true;
        }
    }
}
