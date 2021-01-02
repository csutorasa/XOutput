namespace XOutput.App.UI
{
    public interface IViewBase<VM, M> where VM : ViewModelBase<M> where M : ModelBase
    {
        VM ViewModel { get; }
    }
}
