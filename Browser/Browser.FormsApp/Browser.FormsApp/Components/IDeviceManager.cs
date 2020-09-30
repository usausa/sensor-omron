namespace Browser.FormsApp.Components
{
    using System;

    public interface IDeviceManager
    {
        IObservable<DateTime> TimerTrigger { get; }

        void StartTimer();

        void Restart();
    }
}
