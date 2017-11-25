// <copyright file="PresenterBase`2.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;

namespace CRLFLabs.ViewSize.Mvp
{
    public abstract class PresenterBase<TView, TModel> : PresenterBase<TView>
        where TModel : class
        where TView : class, IView<TModel>
    {
        public PresenterBase(TView view, TModel model)
            : base(view)
        {
            Model = model;
        }

        protected TModel Model { get; }

        protected override void OnViewLoad(object sender, EventArgs e)
        {
            base.OnViewLoad(sender, e);
            View.Model = Model;
        }
    }
}
