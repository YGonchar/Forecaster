using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Forecaster.MvvmUtils
{
    /// <summary>
    ///     Represents an observable property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableObject<T> : INotifyPropertyChanged
    {
        private readonly object _lock = new object();
        private T _data;
        private bool _isDelaying;
        private Action<T> _callback;
        private TimeSpan _delayTime;

        private ObservableObject(T intitState)
        {
            _data = intitState;
            _callback = unused => { };
            _delayTime = TimeSpan.Zero;
        }

        /// <summary>
        ///     Creates a new empty instance.
        /// </summary>
        /// <returns>New instance of the type</returns>
        public static ObservableObject<T> Create()
        {
            return Create(default(T));
        }

        /// <summary>
        ///     Creates a new empty instance with an initial state.
        /// </summary>
        /// <param name="initialState"></param>
        /// <returns>New instance of the type</returns>
        public static ObservableObject<T> Create(T initialState)
        {
            return new ObservableObject<T>(initialState);
        }

        /// <summary>
        ///     The data property to bind.
        /// </summary>
        public T Data
        {
            get { return _data; }
            set
            {
                if (Equals(_data, value))
                    return;
                SetPropertyAndRise(ref _data, value);
            }
        }

        /// <summary>
        ///     Allows to delay an update notification.
        /// </summary>
        /// <param name="delayTime">The delay time</param>
        /// <returns>The same instance</returns>
        public ObservableObject<T> Delay(TimeSpan delayTime)
        {
            _delayTime = delayTime;
            return this;
        }

        /// <summary>
        ///     Allows to set an action that will be processed after the property would changed.
        /// </summary>
        /// <param name="action">Action for executing.</param>
        /// <returns>The same instance</returns>
        public ObservableObject<T> Do(Action<T> action)
        {
            _callback = action;
            return this;
        }

        private void SetPropertyAndRise(ref T data, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(data, value))
                return;
            data = value;
            SynchronizationContext context = SynchronizationContext.Current;
            if (!_isDelaying)
            {
                lock (_lock)
                {
                    _isDelaying = true;

                    Task.Factory.StartNew(() =>
                    {
                        Task.Delay(_delayTime).Wait();
                        _callback?.Invoke(_data);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            OnPropertyChanged(propertyName);
                        });
                        _isDelaying = false;
                    });
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
