using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Exceptions
{
    public class RoleCreationException : Exception
    {
        public RoleCreationException(string message) : base(message)
        {
        }
    }
}
