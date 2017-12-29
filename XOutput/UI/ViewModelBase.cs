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
using XOutput.UI.Resources;

namespace XOutput.UI
{
    public abstract class ViewModelBase<T> where T: ModelBase
    {
        public LanguageModel LanguageModel { get { return LanguageModel.getInstance(); } }
        protected T model;
        public T Model { get { return model; } }
    }
}
