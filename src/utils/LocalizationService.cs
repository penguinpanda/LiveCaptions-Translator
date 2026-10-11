using System.IO;
using System.Windows;

namespace LiveCaptionsTranslator.utils
{
    /// <summary>
    /// Interface localization service: swaps the merged <see cref="ResourceDictionary"/>
    /// at runtime so that every <c>{DynamicResource ...}</c> reference updates instantly.
    /// </summary>
    public static class LocalizationService
    {
        public const string English = "en-US";
        public const string ChineseSimplified = "zh-CN";

        public static readonly List<string> SupportedLanguages = [English, ChineseSimplified];

        // Relative pack URI resolved against this assembly, e.g.
        // "/LiveCaptionsTranslator;component/src/Localization/Strings.zh-CN.xaml".
        private const string DictionarySourceFormat = "/{0};component/src/Localization/Strings.{1}.xaml";
        private const string DictionarySourceMarker = "/Localization/Strings.";

        private static readonly string AssemblyName =
            typeof(LocalizationService).Assembly.GetName().Name!;

        public static string CurrentLanguage { get; private set; } = English;

        /// <summary>
        /// Switches the UI language. Unsupported or empty values fall back to English.
        /// </summary>
        public static void Apply(string? language)
        {
            if (string.IsNullOrWhiteSpace(language) || !SupportedLanguages.Contains(language))
                language = English;

            var app = Application.Current;
            if (app == null)
                return;

            ResourceDictionary target;
            try
            {
                target = new ResourceDictionary
                {
                    Source = new Uri(string.Format(DictionarySourceFormat, AssemblyName, language), UriKind.Relative)
                };
            }
            catch (IOException)
            {
                // The WPF resource system is not ready yet (for example this was called while
                // deserializing setting.json from a static constructor). Keep the previous
                // language; App.OnStartup applies it again once resources are available.
                return;
            }

            CurrentLanguage = language!;

            var dictionaries = app.Resources.MergedDictionaries;
            for (int i = 0; i < dictionaries.Count; i++)
            {
                var source = dictionaries[i].Source;
                if (source != null && source.OriginalString.Contains(DictionarySourceMarker))
                {
                    dictionaries[i] = target;
                    return;
                }
            }

            dictionaries.Add(target);
        }

        /// <summary>
        /// Gets the localized string for <paramref name="key"/>, falling back to the key itself.
        /// </summary>
        public static string Get(string key)
        {
            var value = Application.Current?.TryFindResource(key) as string;
            return string.IsNullOrEmpty(value) ? key : value;
        }

        /// <summary>
        /// Gets the localized string for <paramref name="key"/> and formats it with <paramref name="args"/>.
        /// </summary>
        public static string Format(string key, params object?[] args) =>
            string.Format(Get(key), args);
    }
}
