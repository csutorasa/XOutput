using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.UI.Component;

namespace XOutput.UI
{
    public abstract class ViewModelBase<T> where T: ModelBase
    {
        public LanguageModel LanguageModel => LanguageModel.Instance;
        protected T model;
        public T Model => model;
    }
}
