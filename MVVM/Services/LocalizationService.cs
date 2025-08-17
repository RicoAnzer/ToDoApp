using System.Globalization;
using System.Resources;

/**
Changes languages in NoteApp
=> Training Exercise
Author: Rico Anzer
*/
namespace NoteApp.MVVM.Services
{
    public partial class LocalizationService: ILocalizationService
    {
        //Instance of localizationService
        public static LocalizationService? Instance { get; private set; }
        //Manages access to resource files
        private ResourceManager _resourceManager;
        //Contains information of current language
        private CultureInfo _currentCulture;

        //Constructor
        public LocalizationService() 
        {
            Instance = this;
            //Define resource files
            _resourceManager = new ResourceManager("NoteApp.MVVM.Services.LanguageFiles.Strings", typeof(LocalizationService).Assembly);
            _currentCulture = CultureInfo.CurrentCulture;
        }
        //Retrieves localized string for given key and current language
        public string GetString(string key) 
        {
            return _resourceManager.GetString(key, _currentCulture)!;
        }
        //Updates current language based on selected language
        public void SetCulture(string cultureCode) 
        {
            _currentCulture = new CultureInfo(cultureCode);
        }
    }
}
