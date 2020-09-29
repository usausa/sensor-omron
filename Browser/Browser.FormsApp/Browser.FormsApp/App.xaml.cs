namespace Browser.FormsApp
{
    using Browser.FormsApp.Components;
    using Browser.FormsApp.State;

    using Smart.Forms.Resolver;
    using Smart.Resolver;

    public partial class App
    {
        private readonly SmartResolver resolver;

        public App(IComponentProvider provider)
        {
            InitializeComponent();

            // Config Resolver
            resolver = CreateResolver(provider);
            ResolveProvider.Default.UseSmartResolver(resolver);

            MainPage = resolver.Get<MainPage>();
        }

        private SmartResolver CreateResolver(IComponentProvider provider)
        {
            var config = new ResolverConfig()
                .UseAutoBinding()
                .UseArrayBinding()
                .UseAssignableBinding()
                .UsePropertyInjector();

            config.BindSingleton<Configuration>();

            provider.RegisterComponents(config);

            return config.ToResolver();
        }

        protected override void OnStart()
        {
            var deviceManager = resolver.Get<IDeviceManager>();

            // TODO 再起動設定

            deviceManager.StartTimer();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
