using System;
using System.Collections.Generic;
using System.Text;

namespace mpp_5
{
    public class UnregisteredAbstractionTypeException : Exception
    {
       
        public UnregisteredAbstractionTypeException()
        {
        }

        public UnregisteredAbstractionTypeException(string message)
            : base(message)
        {
        }

       
    }
}
