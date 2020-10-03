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
            //Url.Value = configuration.Url;
            Url.Value = "http://192.168.100.10:3000/d/windows-hserver/windows-node?orgId=1&refresh=10s&kiosk";
        }
    }
}
