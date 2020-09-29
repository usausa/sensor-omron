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
    }
}
