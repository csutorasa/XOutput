using System.ComponentModel;

namespace XOutput.UI
{
    public abstract class ViewModelBase<M> where M : ModelBase, INotifyPropertyChanged
    {
        public LanguageModel LanguageModel => LanguageModel.Instance;
        private readonly M model;
        public M Model => model;

        protected ViewModelBase(M model)
        {
            this.model = model;
        }
    }
}
