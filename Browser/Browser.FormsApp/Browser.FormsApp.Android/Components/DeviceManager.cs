namespace Browser.FormsApp.Droid.Components
{
    using System;
    using System.Reactive.Subjects;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;

    using Browser.FormsApp.Components;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    public sealed class DeviceManager : IDeviceManager
    {
        private readonly Activity activity;

        private readonly AlarmManager alarmManager;

        private readonly Intent alarmIntent = new Intent();

        private readonly IntentFilter intentFilter = new IntentFilter();

        private readonly Subject<DateTime> timerTrigger = new Subject<DateTime>();

        public DeviceManager(Activity activity)
        {
            this.activity = activity;
            alarmManager = activity.GetSystemService(Context.AlarmService).JavaCast<AlarmManager>();

            alarmIntent.SetAction("com.example.monitor.ALARM_UPDATE");
            intentFilter.AddAction("com.example.monitor.ALARM_UPDATE");
        }

        public IObservable<DateTime> TimerTrigger => timerTrigger;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
        public void StartTimer()
        {
            activity.RegisterReceiver(new AlarmBroadcastReceiver(this), intentFilter);

            var next = CalcNextEventDateTime();
            var pendingIntent = PendingIntent.GetBroadcast(activity, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            alarmManager.SetAndAllowWhileIdle(AlarmType.RtcWakeup, next, pendingIntent);
        }

        private void OnTimer()
        {
            var next = CalcNextEventDateTime();
            var pendingIntent = PendingIntent.GetBroadcast(activity, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            alarmManager.SetAndAllowWhileIdle(AlarmType.RtcWakeup, next, pendingIntent);

            timerTrigger.OnNext(DateTime.Now);
        }

        private static long CalcNextEventDateTime()
        {
            var now = DateTime.UtcNow;
            var next = new DateTimeOffset(new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1));
            return next.ToUnixTimeMilliseconds();
        }

        private sealed class AlarmBroadcastReceiver : BroadcastReceiver
        {
            private readonly DeviceManager deviceManager;

            public AlarmBroadcastReceiver(DeviceManager deviceManager)
            {
                this.deviceManager = deviceManager;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                deviceManager.OnTimer();
            }
        }

        public void Restart()
        {
            var context = activity.ApplicationContext;

            using var intent = new Intent();
            intent.SetClassName(context.PackageName, "com.example.monitor.RestartActivity");
            intent.SetFlags(ActivityFlags.NewTask);
            intent.PutExtra("pid", Process.MyPid());
            context.StartActivity(intent);
        }
    }
}
