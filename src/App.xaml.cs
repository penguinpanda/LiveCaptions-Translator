using System.Windows;

using LiveCaptionsTranslator.apis;
using LiveCaptionsTranslator.utils;

namespace LiveCaptionsTranslator
{
    public partial class App : Application
    {
        App()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Translator.Setting?.Save();

            Task.Run(() => Translator.SyncLoop());
            Task.Run(() => Translator.TranslateLoop());
            Task.Run(() => Translator.DisplayLoop());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Applied here (not in the constructor) because App.xaml resources are
            // only loaded after the constructor runs.
            LocalizationService.Apply(Translator.Setting?.Language);
            TranslateAPI.ApplyTimeout(Translator.Setting?.NetworkTimeout ?? 30);

            base.OnStartup(e);
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            if (Translator.Window != null)
            {
                LiveCaptionsHandler.RestoreLiveCaptions(Translator.Window);
                LiveCaptionsHandler.KillLiveCaptions(Translator.Window);
            }
        }
    }
}
