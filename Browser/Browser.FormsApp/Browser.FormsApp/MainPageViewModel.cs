namespace Browser.FormsApp
{
    using Browser.FormsApp.State;

    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;

    public class MainPageViewModel : ViewModelBase
    {
        public static MainPageViewModel DesignInstance => null; // For design

        public NotificationValue<string> Url { get; } = new NotificationValue<string>();

        public MainPageViewModel(Configuration configuration)
        {
            Url.Value = configuration.Url;
        }
    }
}
