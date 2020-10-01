namespace Browser.FormsApp
{
    using System;

    using Browser.FormsApp.Components;
    using Browser.FormsApp.State;

    using Smart.Forms.Resolver;
    using Smart.Resolver;

    using Xamarin.Forms;

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

        private static SmartResolver CreateResolver(IComponentProvider provider)
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
            var configuration = resolver.Get<Configuration>();

            var previousDate = DateTime.Today;
            Device.StartTimer(
                TimeSpan.FromMinutes(1),
                () =>
                {
                    if (configuration.DailyRestart && (DateTime.Now.Date != previousDate))
                    {
                        deviceManager.Restart();
                    }

                    return true;
                });
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
