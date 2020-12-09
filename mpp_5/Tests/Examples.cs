using mpp_5;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{

    interface IService { }
    class ServiceImpl : IService
    {
        public ServiceImpl() { }
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

    interface IServiceGeneric<out TRepository> where TRepository : IRepository { }
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

    class RepositoryImplSingleton : IRepository
    {
        public RepositoryImplSingleton() { }
    }
    class RepositoryImplWiithNesting : IRepository
    {
        public IService _ser;
        public RepositoryImplWiithNesting(IService service) { _ser = service; }
    }

    enum ServiceImplementations
    {
        First,
        Second
    }

    class SomeAnotherService
    {
        public IService _ser;
        public SomeAnotherService([DependencyKey((int)ServiceImplementations.Second)] IService service)
        {
            _ser = service;
        }
    }


}
