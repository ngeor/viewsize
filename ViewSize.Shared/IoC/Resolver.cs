// <copyright file="Resolver.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace CRLFLabs.ViewSize.IoC
{
    public class Resolver : IResolver
    {
        private readonly Dictionary<Type, TypeMapping> typeMappings = new Dictionary<Type, TypeMapping>();
        private readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> externalInstances = new Dictionary<Type, object>();

        public Resolver()
        {
            // register this instance as the IResolver
            MapExistingInstance(typeof(IResolver), this);
        }

        public void MapExistingInstance(Type existingType, object existingInstance)
        {
            if (!existingType.IsInstanceOfType(existingInstance))
            {
                throw new ArgumentException($"Instance was not of type {existingType}", nameof(existingInstance));
            }

            externalInstances.Add(existingType, existingInstance);
        }

        public void Map<TFrom, TTo>(bool singleton = false)
        {
            typeMappings[typeof(TFrom)] = new TypeMapping(typeof(TTo), singleton);
        }

        public T Resolve<T>()
        {
            Type type = typeof(T);
            return (T)Resolve(type);
        }

        public object Resolve(Type type)
        {
            object externalInstance = externalInstances.ContainsKey(type) ? externalInstances[type] : null;
            if (externalInstance != null)
            {
                return externalInstance;
            }

            TypeMapping typeMapping = typeMappings.ContainsKey(type) ? typeMappings[type] : new TypeMapping(type, false);
            if (typeMapping.Singleton && singletons.ContainsKey(type))
            {
                return singletons[type];
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
                singletons[type] = result;
            }

            return result;
        }
    }
}
