namespace Browser.FormsApp
{
    using System;

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
            // TODO 見直し
            var deviceManager = resolver.Get<IDeviceManager>();

            var previousDate = DateTime.Today;
            deviceManager.TimerTrigger.Subscribe(x =>
            {
                if (x.Date != previousDate)
                {
                    deviceManager.Restart();
                }
            });

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
