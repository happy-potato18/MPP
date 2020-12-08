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

    class ServiceImplWithNesting1 : IService
    {
        public IRepository _rep;
        public ServiceImplWithNesting1(IRepository repository) { _rep = repository; }
    }

    interface IService2 { }
    class ServiceImplWithNesting2 : IService2
    {
        public IRepository _rep; 
        public ServiceImplWithNesting2(IRepository repository) { _rep = repository; }
    }

    class ServiceImpl1 : IService { }

    class ServiceImpl2 : IService { }

    interface IServiceGeneric<out TRepository> where TRepository : IRepository  { }

    class ServiceGenericImpl<TRepository> : IServiceGeneric<TRepository>
        where TRepository : IRepository
    {
        public ServiceGenericImpl(TRepository repository) { }

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

        [Test]
        public void Test4()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IRepository, RepositoryImpl>();
            dependencies.RegisterTransient<IServiceGeneric<IRepository>, ServiceGenericImpl<RepositoryImpl>>();
            var provider = new DependencyProvider(dependencies);
            IServiceGeneric<IRepository> service = provider.Resolve<IServiceGeneric<IRepository>>();
            Assert.AreEqual(service.GetType(), typeof(ServiceGenericImpl<RepositoryImpl>));
        }

        [Test]
        public void Test5()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IRepository, RepositoryImpl>();
            dependencies.RegisterTransient(typeof(IServiceGeneric<>), typeof(ServiceGenericImpl<>));
            var provider = new DependencyProvider(dependencies);
            IServiceGeneric<IRepository> service = provider.Resolve<IServiceGeneric<IRepository>>();
            Assert.AreEqual(service.GetType(), typeof(ServiceGenericImpl<IRepository>));
        }

        [Test]
        public void Test6()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IService, ServiceImplWithNesting1>();
            dependencies.RegisterTransient<IService2, ServiceImplWithNesting2>();
            dependencies.RegisterSingleton<IRepository, RepositoryImpl>();
            var provider = new DependencyProvider(dependencies);
            ServiceImplWithNesting1 result1 = provider.Resolve<IService>();
            ServiceImplWithNesting2 result2 = provider.Resolve<IService2>();
            Assert.AreEqual(result1._rep, result2._rep);
        }


    }
}