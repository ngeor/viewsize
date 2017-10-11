using System;
using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    public class Registry
    {
        #region Singleton
        private Registry()
        {
        }

        private static readonly Lazy<Registry> _instance = new Lazy<Registry>(() => new Registry());

        public static Registry Instance => _instance.Value;
        #endregion

        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        /// <summary>
        /// Register the specified instance.
        /// </summary>
        public T Register<T>(T instance)
        {
            lock (_instances)
            {
                Type type = typeof(T);
                if (_instances.ContainsKey(type))
                {
                    throw new InvalidOperationException("Type " + type + " already registered");
                }

                _instances.Add(type, instance);
                return instance;
            }    
        }

        /// <summary>
        /// Get the registered instance of this type.
        /// </summary>
        public T Get<T>()
        {
            lock (_instances)
            {
                Type type = typeof(T);
                if (!_instances.ContainsKey(type))
                {
                    throw new IndexOutOfRangeException("Type " + type + " is not registered");
                }

                return (T)_instances[type];
            }
        }
    }
}
