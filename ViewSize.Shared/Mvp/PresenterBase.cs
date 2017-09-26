using System;
namespace CRLFLabs.ViewSize.Mvp
{
    public abstract class PresenterBase<T>
        where T : class
    {
        private T _view;

        public PresenterBase()
        {
        }

        public T View
        {
            get
            {
                return _view;
            }
            set
            {
                if (_view != null)
                {
                    Detach(_view);
                }

                _view = value;

                if (_view != null)
                {
                    Attach(_view);
                }
            }
        }

        protected virtual void Detach(T view)
        {
            
        }

        protected virtual void Attach(T view)
        {
            
        }
    }
}
