using System;
using System.Collections.Generic;
using System.Text;

namespace mpp_5
{
    public class IneligibleImplementationException : Exception
    {
        public IneligibleImplementationException()
        {
        }

        public IneligibleImplementationException(string message)
            : base(message)
        {
        }

    }
}
