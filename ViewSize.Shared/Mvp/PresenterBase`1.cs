// <copyright file="PresenterBase`1.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public abstract class PresenterBase<TView>
        where TView : class, IView
    {
        public PresenterBase(TView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            View = view;
            view.Load += OnViewLoad;
        }

        public TView View { get; }

        protected virtual void OnViewLoad(object sender, EventArgs e)
        {
        }
    }
}
