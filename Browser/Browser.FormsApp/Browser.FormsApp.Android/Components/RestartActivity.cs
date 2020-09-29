namespace Browser.FormsApp.Droid.Components
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Util;

    [Activity(
        Name = "com.example.monitor.RestartActivity",
        Process = ":restart",
        Label = "Restarting...")]
    public class RestartActivity : Activity
    {
        private const string Tag = "Restart";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Log.Info(Tag, "**** RestartActivity.OnCreate ****");

            // Kill main
            var intent = Intent;
            var pid = intent.GetIntExtra("pid", -1);
            Process.KillProcess(pid);

            Log.Info(Tag, $"**** Kill {pid} ****");

            // Restart
            var context = ApplicationContext;
            using (var restartIntent = new Intent(Intent.ActionMain))
            {
                restartIntent.SetClassName(context.PackageName, "com.example.monitor.MainActivity");
                restartIntent.SetFlags(ActivityFlags.NewTask);
                context.StartActivity(restartIntent);
            }

            // Exit
            Finish();
            Process.KillProcess(Process.MyPid());

            Log.Info(Tag, $"**** Kill restart {Process.MyPid()} ****");
        }
    }
}
