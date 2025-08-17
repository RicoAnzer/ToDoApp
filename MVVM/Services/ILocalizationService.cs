using System.Globalization;
using System.Resources;

/**
Interface to change languages in NoteApp
=> Training Exercise
Author: Rico Anzer
*/
namespace NoteApp.MVVM.Services
{
    internal interface ILocalizationService
    {
        //Instance of localizationService
        public static LocalizationService? Instance { get; private set; }
        //Manages access to resource files
        private static ResourceManager? _resourceManager;
        //Contains information of current language
        private static CultureInfo? _currentCulture;

        //Retrieves localized string for given key and current language
        public string GetString(string key);
        //Updates current language based on selected language
        public void SetCulture(string cultureCode){}
    }
}
