using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// This class is a helper that provides a default implementation
    /// for most of the methods of <see cref="IIoC"/>.
    /// </summary>
    public class IoC : IIoC
    {
        private IoC()
        {
            _aggregateCatalog = new AggregateCatalog();
            _compositionContainer = new CompositionContainer(_aggregateCatalog);
        }

        private CompositionContainer _compositionContainer;
        private AggregateCatalog _aggregateCatalog;

        private static IIoC _instance;
        public static IIoC Instance
        {
            get
            {
                if (_instance == null)
                {
                    var value = new IoC();
                    if (Interlocked.CompareExchange(ref _instance, value, null) != null)
                    {
                        value._compositionContainer.Dispose();
                        value._aggregateCatalog.Dispose();
                    }
                }

                return Volatile.Read(ref _instance);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath">格式为c:\MyProject\MyAssembly.dll</param>
        public virtual void AddAssemblyCatalog(string assemblyPath)
        {
            _aggregateCatalog.Catalogs.Add(new AssemblyCatalog(assemblyPath));
        }

        /// <summary>
        /// Implementation of <see cref="IServiceProvider.GetService"/>.
        /// </summary>
        /// <param name="serviceType">The requested service.</param>
        /// <exception cref="ActivationException">if there is an error in resolving the service instance.</exception>
        /// <returns>The requested object.</returns>
        public virtual object GetService(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }

        /// <summary>
        /// Get an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public virtual object GetInstance(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public virtual object GetInstance(Type serviceType, string key)
        {
            try
            {
                return DoGetInstance(serviceType, key);
            }
            catch (Exception ex)
            {
                throw new ActivationException(
                    FormatActivationExceptionMessage(ex, serviceType, key),
                    ex);
            }
        }

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        public virtual IEnumerable<object> GetAllInstances(Type serviceType)
        {
            try
            {
                return DoGetAllInstances(serviceType);
            }
            catch (Exception ex)
            {
                throw new ActivationException(
                    FormatActivateAllExceptionMessage(ex, serviceType),
                    ex);
            }
        }

        /// <summary>
        /// Get an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public virtual TService GetInstance<TService>()
        {
            return (TService)GetInstance(typeof(TService), null);
        }

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public virtual TService GetInstance<TService>(string key)
        {
            return (TService)GetInstance(typeof(TService), key);
        }

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService"/> currently
        /// registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService"/>.</returns>
        public virtual IEnumerable<TService> GetAllInstances<TService>()
        {
            foreach (object item in GetAllInstances(typeof(TService)))
            {
                yield return (TService)item;
            }
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>The requested service instance.</returns>
        protected object DoGetInstance(Type serviceType, string key)
        {
            IEnumerable<Lazy<object, object>> exports = _compositionContainer.GetExports(serviceType, null, key);
            if ((exports != null) && (exports.Count() > 0))
            {
                // If there is more than one value, this will throw an InvalidOperationException, 
                // which will be wrapped by the base class as an ActivationException.
                return exports.Single().Value;
            }

            throw new ActivationException(
                this.FormatActivationExceptionMessage(new CompositionException("Export not found"), serviceType, key));
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        /// resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            List<object> instances = new List<object>();

            IEnumerable<Lazy<object, object>> exports = _compositionContainer.GetExports(serviceType, null, null);
            if (exports != null)
            {
                instances.AddRange(exports.Select(export => export.Value));
            }

            return instances;
        }

        /// <summary>
        /// Format the exception message for use in an <see cref="ActivationException"/>
        /// that occurs while resolving a single service.
        /// </summary>
        /// <param name="actualException">The actual exception thrown by the implementation.</param>
        /// <param name="serviceType">Type of service requested.</param>
        /// <param name="key">Name requested.</param>
        /// <returns>The formatted exception message string.</returns>
        protected virtual string FormatActivationExceptionMessage(Exception actualException, Type serviceType, string key)
        {
            return $"Activation error occurred while trying to get instance of type {serviceType}, key {key}";
        }

        /// <summary>
        /// Format the exception message for use in an <see cref="ActivationException"/>
        /// that occurs while resolving multiple service instances.
        /// </summary>
        /// <param name="actualException">The actual exception thrown by the implementation.</param>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>The formatted exception message string.</returns>
        protected virtual string FormatActivateAllExceptionMessage(Exception actualException, Type serviceType)
        {
            return $"Activation error occurred while trying to get all instances of type {serviceType}";
        }
    }
}
