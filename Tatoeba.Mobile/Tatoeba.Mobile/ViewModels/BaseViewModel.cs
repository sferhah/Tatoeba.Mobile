using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tatoeba.Mobile.Services;

namespace Tatoeba.Mobile.ViewModels
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(TatoebaStatus item) => Status = item;
        public TatoebaStatus Status { get; }
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        protected bool firstAppear = true;
        public async Task OnShow()
        {
            if (firstAppear)
            {
                await Task.Delay(100); //Allow the view to render.
                firstAppear = false;
                OnFirstAppear();
            }
            else
            {
                OnReappear();
            }
        }

        protected virtual void OnFirstAppear() { }
        protected virtual void OnReappear() { }

        public event EventHandler<ErrorEventArgs> Error;
        protected virtual void OnError(TatoebaStatus status)
        {
            Error?.Invoke(this, new ErrorEventArgs(status));
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
