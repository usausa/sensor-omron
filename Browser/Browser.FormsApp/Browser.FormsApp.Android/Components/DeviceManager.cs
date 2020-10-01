namespace Browser.FormsApp.Droid.Components
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Preferences;

    using Browser.FormsApp.Components;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    public sealed class DeviceManager : IDeviceManager
    {
        private readonly Activity activity;

        public DeviceManager(Activity activity)
        {
            this.activity = activity;
        }

        public void Restart()
        {
            var context = activity.ApplicationContext;

            // Sync
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            var preference = PreferenceManager.GetDefaultSharedPreferences(activity);
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
            using (var editor = preference.Edit())
            {
                editor.Commit();
            }

            using var intent = new Intent();
            intent.SetClassName(context.PackageName, "com.example.monitor.RestartActivity");
            intent.SetFlags(ActivityFlags.NewTask);
            intent.PutExtra("pid", Process.MyPid());
            context.StartActivity(intent);
        }
    }
}
