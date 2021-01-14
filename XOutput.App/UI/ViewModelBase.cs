using System;

namespace XOutput.App.UI
{
    public abstract class ViewModelBase<M> where M : ModelBase
    {
        private readonly M model;
        public M Model => model;
        public TranslationModel Translation => TranslationModel.Instance;

        protected ViewModelBase(M model)
        {
            this.model = model;
        }
    }

    public abstract class DisposableViewModelBase<M> : ViewModelBase<M>, IDisposable where M : DisposableModelBase, IDisposable
    {
        protected bool disposed = false;

        protected DisposableViewModelBase(M model) : base(model)
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                Model.Dispose();
            }
            disposed = true;
        }
    }
}