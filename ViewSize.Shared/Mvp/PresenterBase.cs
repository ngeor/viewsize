using System;
namespace CRLFLabs.ViewSize.Mvp
{
    public abstract class PresenterBase<TView>
        where TView: class
    {
        public PresenterBase(TView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            View = view;
            Registry.Instance.Register(view);
        }

        public TView View { get; }
    }

    public abstract class PresenterBase<TView, TModel> : PresenterBase<TView>
        where TView: class
        where TModel: class
    {
        public PresenterBase(TView view, TModel model)
            : base(view)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            Model = model;
            Registry.Instance.Register(model);
        }

        public TModel Model { get; }
    }
}
