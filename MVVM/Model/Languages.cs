/**
Languages Class for in Note-App
Languages can be switched using a comboBox
Adding Languages should be dynamically 
=> only adding new file to LanguageFile folder and Icon to Icons folder necessary
Language object contains name shortcut and path to Icon
=> Training Exercise
Author: Rico Anzer
*/
namespace NoteApp.MVVM.Model
{
    partial class Languages
    {
        public string Lang { get; set; }
        public string IconPath { get; set; }
        public Languages(string lang, string iconPath)
        {
            this.Lang = lang;
            this.IconPath = iconPath;
        }
    }
}
