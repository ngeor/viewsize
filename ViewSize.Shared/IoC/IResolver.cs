// <copyright file="IResolver.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.IoC
{
    public interface IResolver
    {
        void SetPostCreationAction<T>(Action<IResolver> postCreationAction);

        void MapExistingInstance(Type existingType, object existingInstance);

        T Resolve<T>();
    }
}
