using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace XOutput.UI
{
    public interface IViewBase<VM, M> where VM : ViewModelBase<M> where M : ModelBase
    {
        VM ViewModel { get; }
    }
}
