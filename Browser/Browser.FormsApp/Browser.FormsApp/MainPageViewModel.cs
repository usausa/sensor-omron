namespace Browser.FormsApp
{
    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;

    public class MainPageViewModel : ViewModelBase
    {
        public NotificationValue<string> Url { get; } = new NotificationValue<string>();

        public MainPageViewModel()
        {
            Url.Value = "http://www.google.co.jp";
        }
    }
}
