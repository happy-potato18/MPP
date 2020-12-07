using System;
using System.Collections.Generic;
using System.Text;

namespace mpp_5
{
    public class DependencyProvider
    {
        private readonly DependencyConfiguration _dependencyConfiguration;
        public DependencyProvider(DependencyConfiguration dependencyConfiguration)
        {
            _dependencyConfiguration = dependencyConfiguration;
        }

        public TImplementation Resolve<TImplementation>()
        {

            return default;
        }
    }
}
