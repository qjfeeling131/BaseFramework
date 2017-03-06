using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace Abp.Dependency
{
    public class IocManager : IIocManager
    {
        public static IocManager Instance { get; private set; }

        public IContainer IocContainer
        {
            get;
            private set;
        }

        private readonly List<IConventionalDependencyRegistrar> _conventionalRegistrars;

        static IocManager()
        {
            Instance = new IocManager();
        }

        public ContainerBuilder Builder { get; private set; }

        public IocManager()
        {
            Builder = new ContainerBuilder();
            _conventionalRegistrars = new List<IConventionalDependencyRegistrar>();

            Builder.RegisterType<IocManager>().As<IIocManager>();

        }

        public bool BuildComponent()
        {
            try
            {

                IocContainer = Builder.Build();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"build fails, detailed message as below:{ex.Message}");
                return false;
            }

            return true;
        }
        public void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar)
        {
            _conventionalRegistrars.Add(registrar);
        }

        public void Dispose()
        {
            IocContainer.Dispose();
        }

        public bool IsRegistered(Type type)
        {
            return this.IocContainer.IsRegistered(type);
        }

        public bool IsRegistered<T>()
        {
            return this.IocContainer.IsRegistered<T>();
        }

        public void Register(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Singleton:
                    Builder.RegisterType(type).SingleInstance();
                    break;
                case DependencyLifeStyle.Transient:
                    Builder.RegisterType(type).InstancePerLifetimeScope();
                    break;
                default:
                    Builder.RegisterType(type).SingleInstance();
                    break;
            }
        }

        public void Register(Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Singleton:
                    Builder.RegisterType(impl).As(type).SingleInstance();
                    break;
                case DependencyLifeStyle.Transient:
                    Builder.RegisterType(impl).As(type).InstancePerLifetimeScope();
                    break;
                default:
                    Builder.RegisterType(impl).As(type).SingleInstance();
                    break;
            }
        }

        public void Register<TType, TImpl>(DependencyLifeStyle lifeStyle)
             where TType : class
             where TImpl : class, TType
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Singleton:
                    Builder.RegisterType<TImpl>().As<TType>().SingleInstance();
                    break;
                case DependencyLifeStyle.Transient:
                    Builder.RegisterType<TImpl>().As<TType>().InstancePerLifetimeScope();
                    break;
                default:
                    Builder.RegisterType<TImpl>().As<TType>().SingleInstance();
                    break;
            }

        }

        public void Register<T>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Singleton:
                    Builder.RegisterType<T>().SingleInstance();
                    break;
                case DependencyLifeStyle.Transient:
                    Builder.RegisterType<T>().InstancePerLifetimeScope();
                    break;
                default:
                    Builder.RegisterType<T>().SingleInstance();
                    break;
            }
        }

        public void RegisterAssemblyByConvention(Assembly assembly)
        {
            RegisterAssemblyByConvention(assembly, new ConventionalRegistrationConfig());
        }

        public void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config)
        {
            var context = new ConventionalRegistrationContext(assembly, this, config);
            foreach (var register in _conventionalRegistrars)
            {
                register.RegisterAssembly(context);
            }
            if (config.InstallInstallers)
            {
                Builder.RegisterAssemblyModules(assembly);
            }
        }

        public void Release(IDisposable obj)
        {
            IocContainer.Disposer.AddInstanceForDisposal(obj);
        }

        public object Resolve(Type type)
        {
            return IocContainer.Resolve(type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentsAsAnonymousType"></param>
        /// <returns></returns>
        public object Resolve(Type type, Parameter[] argumentsAsAnonymousType)
        {
            return IocContainer.Resolve(type, argumentsAsAnonymousType);
        }

        public T Resolve<T>()
        {
            return IocContainer.Resolve<T>();
        }

        public T Resolve<T>(Parameter[] argumentsAsAnonymousType)
        {
            return IocContainer.Resolve<T>(argumentsAsAnonymousType);
        }

        public T Resolve<T>(Type type)
        {
            return (T)IocContainer.Resolve(type);
        }
        public IEnumerable<T> ResolveAll<T>()
        {
            return IocContainer.Resolve<IEnumerable<T>>();
        }

        public IEnumerable<T> ResolveAll<T>(Parameter[] argumentsAsAnonymousType)
        {
            return IocContainer.Resolve<IEnumerable<T>>(argumentsAsAnonymousType);
        }

        public void Release(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
