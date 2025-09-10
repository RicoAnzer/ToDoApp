/**
Languages Class for Task-App
Languages can be switched using a comboBox
Adding Languages is dynamically 
=> only adding new res file to LanguageFile folder and Icon of equal name to Icons folder necessary
Language object contains name shortcut and path to Icon
=> Training Exercise
Author: Rico Anzer
*/
namespace ToDoApp.MVVM.Model
{
    public partial class Languages
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
