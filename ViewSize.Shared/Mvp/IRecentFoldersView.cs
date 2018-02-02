// <copyright file="IRecentFoldersView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace CRLFLabs.ViewSize.Mvp
{
    public interface IRecentFoldersView : IView<IMainModel>
    {
        void SetRecentFolders(IEnumerable<string> folders);
    }
}
