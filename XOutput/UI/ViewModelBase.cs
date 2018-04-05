using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace XOutput.UI
{
    public abstract class ViewModelBase<M> where M : ModelBase, INotifyPropertyChanged
    {
        public LanguageModel LanguageModel => LanguageModel.Instance;
        private readonly M model;
        public M Model => model;

        public ViewModelBase(M model)
        {
            this.model = model;
        }
    }
}
