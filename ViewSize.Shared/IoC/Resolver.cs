using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize.IoC
{
    public class Resolver
    {
        private readonly Dictionary<Type, TypeMapping> _typeMappings = new Dictionary<Type, TypeMapping>();
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _externalInstances = new Dictionary<Type, object>();

        public void Map(Type existingType, object existingInstance)
        {
            if (!existingType.IsInstanceOfType(existingInstance))
            {
                throw new ArgumentException($"Instance was not of type {existingType}", nameof(existingInstance));
            }

            _externalInstances.Add(existingType, existingInstance);
        }

        public void Map<TFrom, TTo>(bool singleton = false)
        {
            _typeMappings[typeof(TFrom)] = new TypeMapping(typeof(TTo), singleton);
        }

        public T Resolve<T>()
        {
            Type type = typeof(T);
            return (T)Resolve(type);
        }

        public object Resolve(Type type)
        {
            object externalInstance = _externalInstances.ContainsKey(type) ? _externalInstances[type] : null;
            if (externalInstance != null)
            {
                return externalInstance;
            }

            TypeMapping typeMapping = _typeMappings.ContainsKey(type) ? _typeMappings[type] : new TypeMapping(type, false);
            if (typeMapping.Singleton && _singletons.ContainsKey(type))
            {
                return _singletons[type];
            }

            Type finalType = typeMapping.ActualType;
            var constructors = finalType.GetConstructors();
            if (constructors == null || constructors.Length <= 0)
            {
                throw new InvalidOperationException($"No constructors available for type {type}");
            }

            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"Too many constructors available for type {type}");
            }

            var constructor = constructors[0];
            var parameters = constructor.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var parameterValues = parameterTypes.Select(pt => Resolve(pt)).ToArray();
            var result = constructor.Invoke(parameterValues);
            if (typeMapping.Singleton)
            {
                _singletons[type] = result;
            }

            return result;
        }
    }
}
