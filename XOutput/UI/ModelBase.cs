using System.ComponentModel;

namespace XOutput.UI
{
    public abstract class ModelBase : INotifyPropertyChanged, ICleanUp
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool cleanup = false;

        /// <summary>
        /// Invokes the property changed event
        /// </summary>
        /// <param name="name">Name of the property that changed</param>
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            if (cleanup)
            {
                return;
            }
            foreach (var propertyName in propertyNames)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool Set<T>(T newValue, ref T currentValue, params string[] propertyNames)
        {
            if (cleanup)
            {
                return false;
            }
            bool changed = false;
            if (!Equals(currentValue, newValue))
            {
                currentValue = newValue;
                OnPropertyChanged(propertyNames);
                changed = true;
            }
            return changed;
        }

        public virtual void CleanUp()
        {
            cleanup = true;
        }
    }
}
