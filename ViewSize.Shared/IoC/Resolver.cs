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
        private readonly Dictionary<Type, Action<IResolver>> postCreationActions = new Dictionary<Type, Action<IResolver>>();

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
            typeMappings[typeof(TFrom)] = new TypeMapping(typeof(TFrom), typeof(TTo), singleton);
        }

        public T Resolve<T>()
        {
            Type type = typeof(T);
            return (T)Resolve(type);
        }

        public object Resolve(Type type)
        {
            return ResolveExternalInstance(type) ?? ResolveInternalInstance(type);
        }

        public void SetPostCreationAction<T>(Action<IResolver> postCreationAction)
        {
            postCreationActions[typeof(T)] = postCreationAction;
        }

        private object ResolveExternalInstance(Type type)
        {
            return externalInstances.ContainsKey(type) ? externalInstances[type] : null;
        }

        private object ResolveInternalInstance(Type type)
        {
            return ResolveExistingSingleton(type) ?? ResolveNonExistingSingleton(type);
        }

        private object ResolveExistingSingleton(Type type)
        {
            return singletons.ContainsKey(type) ? singletons[type] : null;
        }

        private object ResolveNonExistingSingleton(Type type)
        {
            TypeMapping typeMapping = typeMappings.ContainsKey(type) ? typeMappings[type] : new TypeMapping(type, type, false);
            return ResolveInternalInstance(typeMapping);
        }

        private object ResolveInternalInstance(TypeMapping typeMapping)
        {
            Type finalType = ResolveConcreteClass(typeMapping.To);
            if (finalType == null)
            {
                throw new InvalidOperationException($"Cannot resolve type {typeMapping.From}");
            }

            if (finalType.IsGenericType)
            {
                var genericTypeDefinition = finalType.GetGenericTypeDefinition();
                if (genericTypeDefinition.Equals(typeof(Lazy<>)))
                {
                    return ResolveLazy(finalType);
                }
            }

            var constructors = finalType.GetConstructors();
            if (constructors == null || constructors.Length <= 0)
            {
                throw new InvalidOperationException($"No constructors available for type {typeMapping.From}");
            }

            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"Too many constructors available for type {typeMapping.From}");
            }

            var constructor = constructors[0];
            var parameters = constructor.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var parameterValues = parameterTypes.Select(pt => Resolve(pt)).ToArray();
            var result = constructor.Invoke(parameterValues);
            if (typeMapping.Singleton)
            {
                singletons[typeMapping.From] = result;
            }

            var postCreationAction = postCreationActions.ContainsKey(typeMapping.From) ? postCreationActions[typeMapping.From] : null;
            postCreationAction?.Invoke(this);

            return result;
        }

        private object ResolveLazy(Type finalType)
        {
            // e.g. ISimple
            Type interfaceType = finalType.GenericTypeArguments[0];

            // e.g. LazyResolver<ISimpe>
            Type lazyResolverType = typeof(LazyResolver<>).MakeGenericType(interfaceType);

            // e.g. new Lazy<ISimple>
            var ctor = lazyResolverType.GetConstructors().Single();

            // invoke new Lazy<ISimple>(func)
            var lazyResolver = ctor.Invoke(new object[] { this });

            return lazyResolverType.GetMethod("Resolve").Invoke(lazyResolver, null);
        }

        private Type ResolveConcreteClass(Type type)
        {
            Type finalType = type;
            if (finalType.IsInterface)
            {
                finalType = finalType.Assembly.GetType(finalType.Namespace + '.' + finalType.Name.Substring(1));
            }

            return finalType;
        }

        private class LazyResolver<T>
        {
            private readonly IResolver resolver;

            public LazyResolver(IResolver resolver)
            {
                this.resolver = resolver;
            }

            public Lazy<T> Resolve()
            {
                return new Lazy<T>(() => resolver.Resolve<T>());
            }
        }
    }
}
