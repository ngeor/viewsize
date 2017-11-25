// <copyright file="PresenterFactory.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Linq;
using CRLFLabs.ViewSize.IoC;

namespace CRLFLabs.ViewSize.Mvp
{
    public static class PresenterFactory
    {
        public static void Create(object view)
        {
            Create(ResolverContainer.Resolver, view);
        }

        public static void Create(Resolver resolver, object view)
        {
            var presenterAttributes = view.GetType().GetCustomAttributes(typeof(PresenterAttribute), false)
                .OfType<PresenterAttribute>();

            foreach (var presenterAttribute in presenterAttributes)
            {
                Create(presenterAttribute, resolver, view);
            }
        }

        private static void Create(PresenterAttribute presenterAttribute, Resolver resolver, object view)
        {
            var presenterType = presenterAttribute.PresenterType;
            var viewProperty = presenterType.GetProperty("View");
            if (viewProperty == null)
            {
                throw new InvalidOperationException($"Presenter {presenterType} does not have a View property");
            }

            var viewType = viewProperty.PropertyType;
            if (!typeof(IView).IsAssignableFrom(viewType))
            {
                throw new InvalidOperationException($"Presenter {presenterType} has a View property which does not inherit from IView");
            }

            if (!viewType.IsInstanceOfType(view))
            {
                throw new InvalidOperationException($"View is not of expected type {viewType}");
            }

            // map the view object to the view type, so that resolver will now what to do with view type
            resolver.MapExistingInstance(viewType, view);

            // create presenter (ignore presenter object returned)
            resolver.Resolve(presenterType);
        }
    }
}
