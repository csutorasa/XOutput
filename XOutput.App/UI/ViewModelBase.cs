using System.ComponentModel;

namespace XOutput.App.UI
{
    public abstract class ViewModelBase<M> where M : ModelBase, INotifyPropertyChanged
    {
        private readonly M model;
        public M Model => model;
        public TranslationModel Translation => TranslationModel.Instance;

        protected ViewModelBase(M model)
        {
            this.model = model;
        }
    }
}