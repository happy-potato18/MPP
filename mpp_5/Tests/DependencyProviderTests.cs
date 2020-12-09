using NUnit.Framework;
using mpp_5;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Tests
{
   
    public class Tests
    {
       
        [Test]
        public void Resolve_SingleNestedDependency_NestedDependencyType()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IService, ServiceImplWithNesting1>();
            dependencies.RegisterTransient<IRepository, RepositoryImpl>();
            var provider = new DependencyProvider(dependencies);
            ServiceImplWithNesting1 service =  provider.Resolve<IService>();
            Type expectedType = typeof(RepositoryImpl);
            Type receivedType = service._rep.GetType();
            Assert.AreEqual(expectedType, receivedType, ":-(");
        }


        [Test]
        public void Resolve_MultipleDependencies_IenumerableDependencyType()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IService, ServiceImpl1>();
            dependencies.RegisterTransient<IService, ServiceImpl2>();
            var provider = new DependencyProvider(dependencies);
            IEnumerable<IService> services = provider.Resolve<IEnumerable<IService>>();
            Type expectedType = typeof(List<IService>);
            Type receivedType = services.GetType();
            Assert.AreEqual(expectedType, receivedType);
        }

        [Test]
        public void Resolve_MultipleNestedDependencies_NestedDependencyType()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IService, ServiceImpl1>();
            dependencies.RegisterTransient<IService, ServiceImpl2>();
            dependencies.RegisterTransient<IRepository, RepositoryImplWiithNesting>();
            var provider = new DependencyProvider(dependencies);
            RepositoryImplWiithNesting service = provider.Resolve<IRepository>();
            Type expectedType = typeof(ServiceImpl1);
            Type receivedType = service._ser.GetType();
            Assert.AreEqual(expectedType, receivedType);
        }

        [Test]
        public void Resolve_ClosedGenericDependency_DependencyType()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IRepository, RepositoryImpl>();
            dependencies.RegisterTransient<IServiceGeneric<IRepository>, ServiceGenericImpl<RepositoryImpl>>();
            var provider = new DependencyProvider(dependencies);
            IServiceGeneric<IRepository> service = provider.Resolve<IServiceGeneric<IRepository>>();
            Type expectedType = typeof(ServiceGenericImpl<RepositoryImpl>);
            Type receivedType = service.GetType();
            Assert.AreEqual(expectedType,receivedType);
        }

        [Test]
        public void Resolve_OpenGenericDependency_ClosedGenericDependencyType()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IRepository, RepositoryImpl>();
            dependencies.RegisterTransient(typeof(IServiceGeneric<>), typeof(ServiceGenericImpl<>));
            var provider = new DependencyProvider(dependencies);
            IServiceGeneric<IRepository> service = provider.Resolve<IServiceGeneric<IRepository>>();
            Type expectedType = typeof(ServiceGenericImpl<IRepository>);
            Type receivedType = service.GetType();
            Assert.AreEqual(expectedType, receivedType);
        }

        [Test]
        public void Resolve_SingletonNestedDelendencyForMultipleDependencies_CommonNestedDependencyReference()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<IService, ServiceImplWithNesting1>();
            dependencies.RegisterTransient<IService2, ServiceImplWithNesting2>();
            dependencies.RegisterSingleton<IRepository, RepositoryImplSingleton>();
            var provider = new DependencyProvider(dependencies);
            ServiceImplWithNesting1 service1 = provider.Resolve<IService>();
            ServiceImplWithNesting2 service2 = provider.Resolve<IService2>();
            Type service1NestedTypeype = service1._rep.GetType();
            Type service2NestedTypeype = service2._rep.GetType();
            Assert.AreEqual(service1NestedTypeype, service2NestedTypeype);
        }

        [Test]
        public void Resolve_RegisterAsSelf_DependencyType()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterTransient<ServiceImpl1, ServiceImpl1>();
            var provider = new DependencyProvider(dependencies);
            var service = provider.Resolve<ServiceImpl1>();
            Type expectedType = typeof(ServiceImpl1);
            Type receivedType = service.GetType();
            Assert.AreEqual(expectedType, receivedType);
        }

        [Test]
        public void ValidateDependenciesInConstructor_IncorrectDependency_ExceptionMessage()
        {
            var dependencies = new DependencyConfiguration();
            string expectedMessage = "Type RepositoryImpl is ineligble implementation for type IService.";
            string receivedMessage = "";
            dependencies.RegisterTransient<IService, RepositoryImpl>();
            try
            {
                var provider = new DependencyProvider(dependencies);
                IService service = provider.Resolve<ServiceImpl1>();
            }
            catch(IneligibleImplementationException e)
            {
                receivedMessage = e.Message;
            }
            
            Assert.AreEqual(expectedMessage, receivedMessage);
        }

    }
}