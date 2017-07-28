using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public interface ILanguageResource
    {
        #region properties
        /// <summary>
        /// Get current language tag
        /// </summary>
        string CurrentLanguageTag { get; }

        /// <summary>
        /// Get text that relative to key if exist，or return null
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        string this[string key] { get; }
        #endregion

        #region events
        event EventHandler<LanguageChangedEventArgs> LanguageChanged;
        #endregion

        #region methods
        /// <summary>
        /// Check if support this langauge based on default language folder, return true if the specified language
        /// exist or some similar langauge exist
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag</param>
        /// <returns></returns>
        bool IsSupported(string ietfLanguageTag);

        /// <summary>
        /// Check if specified language exist in default folder
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag</param>
        /// <returns></returns>
        bool IsContains(string ietfLanguageTag);

        /// <summary>
        /// Get all spported languages, ietf language tag list
        /// </summary>
        /// <returns></returns>
        List<string> GetContainsLanguageTags();

        /// <summary>
        /// Set language
        /// </summary>
        /// <param name="ietfLanguageTag">ietf language tag</param>
        void SetLanguageTag(string ietfLanguageTag);
        #endregion
    }
}
