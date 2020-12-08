using NUnit.Framework;
using mpp_5;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    interface IService { }
    class ServiceImpl : IService
    {
        public ServiceImpl()  {}
    }

    class ServiceImpl1 : IService   { }

    class ServiceImpl2 : IService { }

    interface IRepository { }
    class RepositoryImpl : IRepository
    {
        public RepositoryImpl(IService service) { } 
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
          
        }

        [Test]
        public void Test1()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IService, ServiceImpl>();
            dependencies.RegisterTransient<IRepository, RepositoryImpl>();
            var provider = new DependencyProvider(dependencies);
            var result =  provider.Resolve<IService>();
            Assert.AreEqual( result.GetType(), typeof(ServiceImpl));
        }


        [Test]
        public void Test2()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IService, ServiceImpl1>();
            dependencies.RegisterTransient<IService, ServiceImpl2>();
            var provider = new DependencyProvider(dependencies);
            IEnumerable<IService> services = provider.Resolve<IEnumerable<IService>>();
            Assert.AreEqual(services.GetType(), typeof(List<IService>));
        }

        [Test]
        public void Test3()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IService, ServiceImpl1>();
            dependencies.RegisterTransient<IService, ServiceImpl2>();
            dependencies.RegisterTransient<IRepository, RepositoryImpl>();
            var provider = new DependencyProvider(dependencies);
            IRepository service = provider.Resolve<IRepository>();
            Assert.AreEqual(service.GetType(), typeof(RepositoryImpl));
        }
    }
}