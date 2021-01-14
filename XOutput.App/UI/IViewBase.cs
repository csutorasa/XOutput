namespace XOutput.App.UI
{
    public interface IViewBase<VM, M> where VM : ViewModelBase<M> where M : ModelBase
    {
        VM ViewModel { get; }
    }

    public static class DispoableViewBaseHelper
    {
        public static void DisposeViewModel<VM, M>(this IViewBase<VM, M> dispoableViewBase) where VM : DisposableViewModelBase<M> where M : DisposableModelBase
        {
            dispoableViewBase.ViewModel.Dispose();
        }
    }
}
