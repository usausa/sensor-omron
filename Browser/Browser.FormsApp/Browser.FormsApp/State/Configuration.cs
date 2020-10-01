namespace Browser.FormsApp.State
{
    using Xamarin.Essentials;

    public class Configuration
    {
        public string Url
        {
            get => Preferences.Get(nameof(Url), string.Empty);
            set => Preferences.Set(nameof(Url), value);
        }

        public bool DailyRestart
        {
            get => Preferences.Get(nameof(DailyRestart), true);
            set => Preferences.Set(nameof(DailyRestart), value);
        }
    }
}
