using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MorganStanley.ComposeUI.ModuleLoader;
using MorganStanley.Fdc3.AppDirectory;
using Nito.AsyncEx.Synchronous;
using static MorganStanley.ComposeUI.Shell.Modules.ModuleCatalog;

namespace MorganStanley.ComposeUI.Shell.Fdc3
{
    internal sealed class Fdc3ModuleCatalog : IModuleCatalog
    {
        private readonly IAppDirectory _appDirectory;

        public Fdc3ModuleCatalog(IAppDirectory fdc3AppDirectory)
        {
            _appDirectory = fdc3AppDirectory;
        }

        public IModuleManifest GetManifest(string moduleId)
        {
            var task = GetManifestInternal(moduleId);
            return task.WaitAndUnwrapException();
        }

        private async Task<IModuleManifest> GetManifestInternal(string moduleId)
        {
            var app = await _appDirectory.GetApp(moduleId);
            var url = new Uri(((WebAppDetails) (app.Details)).Url, UriKind.Absolute);
            var iconSrc = app.Icons?.FirstOrDefault()?.Src;

            return new WebModuleManifest
            {
                Id = app.AppId,
                Details = new WebManifestDetails
                {
                    Url = url,
                    IconUrl = iconSrc != null ? new Uri(iconSrc, UriKind.Absolute) : null
                },
                ModuleType = ModuleType.Web,
                Name = app.Name
            };
        }

        public IEnumerable<string> GetModuleIds()
        {
            return GetModuleIdsInternal().WaitAndUnwrapException();
        }

        private async Task<IEnumerable<string>> GetModuleIdsInternal()
        {
            var apps = await _appDirectory.GetApps();
            return apps.Select(x => x.AppId);
        }
    }
}
