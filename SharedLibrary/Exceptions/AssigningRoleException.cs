using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Exceptions
{
    public class AssigningRoleException : Exception
    {
        public AssigningRoleException(string message) : base(message)
        {

        }
    }
}
