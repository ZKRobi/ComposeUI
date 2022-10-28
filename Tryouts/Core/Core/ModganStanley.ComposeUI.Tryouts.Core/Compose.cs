using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using MorganStanley.ComposeUI.Tryouts.Core.Services.ModulesService;
using System.Reactive;
using System.Text.Json;

namespace ModganStanley.ComposeUI.Tryouts.Core
{
    public class Compose
    {
        public IComposeConfiguration Configuration { get; }
        private IModuleLoader _moduleLoader;
        private Guid _messageRouterInstanceId;
        private ModuleLoaderMessaging _moduleLoaderMessaging;

        internal Compose(IComposeConfiguration configuration)
        {
            Configuration = configuration;
        }
        public static ComposeBuilder Configure => new ComposeBuilder();

        public async Task Run()
        {
            var manifestString = File.ReadAllText(Configuration.ManifestFilePath);
            var manifest = JsonSerializer.Deserialize<Dictionary<string, ModuleManifest>>(manifestString);

            var catalogue = new ModuleCatalogue(manifest);

            var factory = new ModuleLoaderFactory();
            _moduleLoader = await factory.Create(catalogue);

            _moduleLoader.LifecycleEvents.Subscribe(Observer.Create<LifecycleEvent>(HandleLifecycleEvent));

            _moduleLoaderMessaging = new ModuleLoaderMessaging(null, _moduleLoader);
        }

        private void HandleLifecycleEvent(LifecycleEvent lifecycleEvent)
        {
            if (lifecycleEvent.ProcessInfo.instanceId == _messageRouterInstanceId && !lifecycleEvent.IsExpected && lifecycleEvent.EventType == LifecycleEventType.Stopped)
            {
                StartMessageRouter();
            }
        }

        internal void StartMessageRouter()
        {
            if (_messageRouterInstanceId == Guid.Empty)
            {
                _messageRouterInstanceId = Guid.NewGuid();
            }
            _moduleLoader.RequestStartProcess(
                new LaunchRequest
                {
                    instanceId = _messageRouterInstanceId,
                    name = "messageRouter"
                });
        }
    }

    public interface IComposeConfiguration
    {
        bool UseMessageRouter { get; }
        string ManifestFilePath { get; }
    }

    public class ComposeBuilder : IComposeConfiguration
    {
        internal ComposeBuilder() { }

        public bool UseMessageRouter { get; private set; }
        public string ManifestFilePath { get; private set; }

        public ComposeBuilder WithMessageRouter()
        {
            this.UseMessageRouter = true;
            return this;
        }

        public ComposeBuilder WithManifestFile(string path)
        {
            this.ManifestFilePath = Path.GetFullPath(path);
            return this;
        }

        public Compose Build()
        {
            return new Compose(this);
        }
    }
}