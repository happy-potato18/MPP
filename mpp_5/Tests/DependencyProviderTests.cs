using NUnit.Framework;
using mpp_5;

namespace Tests
{
    interface IService { }
    class ServiceImpl : IService
    {
        public ServiceImpl(IRepository repository)
        {
           
        }
    }

    interface IRepository { }
    class RepositoryImpl : IRepository
    {
        public RepositoryImpl() { } 
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
            Assert.IsTrue( result.GetType() == typeof(ServiceImpl));
        }
    }
}