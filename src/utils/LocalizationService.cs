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

        private const string DictionarySourceFormat = "pack://application:,,,/Localization/Strings.{0}.xaml";
        private const string DictionarySourceMarker = "/Localization/Strings.";

        public static string CurrentLanguage { get; private set; } = English;

        /// <summary>
        /// Switches the UI language. Unsupported or empty values fall back to English.
        /// </summary>
        public static void Apply(string? language)
        {
            if (string.IsNullOrWhiteSpace(language) || !SupportedLanguages.Contains(language))
                language = English;

            CurrentLanguage = language!;

            var app = Application.Current;
            if (app == null)
                return;

            var target = new ResourceDictionary
            {
                Source = new Uri(string.Format(DictionarySourceFormat, language))
            };

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
