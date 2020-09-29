namespace Browser.FormsApp.Droid.Components
{
    using Android.App;
    using Android.Content;
    using Android.OS;

    using Browser.FormsApp.Components;

    public sealed class DeviceManager : IDeviceManager
    {
        private readonly Activity activity;

        public DeviceManager(Activity activity)
        {
            this.activity = activity;
        }

        public void StartTimer()
        {
            // TODO
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
