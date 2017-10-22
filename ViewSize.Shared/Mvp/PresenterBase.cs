using System;
namespace CRLFLabs.ViewSize.Mvp
{
    public abstract class PresenterBase<TView>
        where TView: class, IView
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

    public abstract class PresenterBase<TView, TModel> : PresenterBase<TView>
        where TModel : class
        where TView : class, IView<TModel>
    {
        public PresenterBase(TView view, TModel model) : base(view)
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
