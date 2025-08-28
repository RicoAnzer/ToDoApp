using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteApp.MVVM.Model;
using NoteApp.MVVM.Services;
using NoteApp.MVVM.Services.SQLite.Database;
using NoteApp.MVVM.View;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;

/**
ViewModel Class for NoteListWindow in Note-App
=> Training Exercise
Author: Rico Anzer
*/
namespace NoteApp.MVVM.ViewModel
{
    public partial class NoteListWindowViewModel : ObservableObject
    {
        //Instance of WindowsService
        private readonly WindowService _windowService;
        //Instance of DatabaseService
        private readonly DatabaseService _databaseService;
        //Instance of LocalizationService
        public static LocalizationService? _localizationService;
        //Instance of AddNoteViewModel
        public static AddNoteViewModel? _AddNoteViewModel;

        //Instance of this ViewModel
        public static NoteListWindowViewModel? Instance { get; private set; }

        //Path to workingDirectory
        private static string workingDirectory = Path.GetDirectoryName(Environment.ProcessPath)!.Split("\\bin")[0];
        //Path to LanguageFile Folder => combining Path of working directory and Path inside working directory defined in App.config
        private static string langPath = workingDirectory + ConfigurationManager.AppSettings["languagesPath"];

        //Currently in ComboBox selected Languages Object
        //=> Dynamically changes the current localization, whenever a new language is selected in ComboBox
        private Languages selectedLang = null!;
        public Languages SelectedLang {
            get { return selectedLang; }
            set { selectedLang = value;
                if (value != null) 
                {
                    ChangeLang(value.Lang);
                }
            }
        }

        //List containing all Notes to display
        [ObservableProperty]
        private ObservableCollection<Note> noteList;
        //List containing all Languages (each object has name and path to icon)
        public List<Languages> LangList { get; set; }

        //Strings to bind text and resource files
        //Both, NoteListWindow and AddNoteWindow store translated strings in LocalizationService and access theirs in ViewModel
        public string AddNoteBtn => _localizationService!.GetString("BtnAddNote");
        public string IdHeader => _localizationService!.GetString("HeaderID");
        public string DescHeader => _localizationService!.GetString("HeaderDescription");
        public string PriorityHeader => _localizationService!.GetString("HeaderPriority");
        public string DueDateHeader => _localizationService!.GetString("HeaderDueDate");
        public string HighPriority => _localizationService!.GetString("PriorityHigh");
        public string MediumPriority => _localizationService!.GetString("PriorityMedium");
        public string LowPriority => _localizationService!.GetString("PriorityLow");

        //Constructor
        public NoteListWindowViewModel()
        {
            //Initialize services
            Instance = this;
            _AddNoteViewModel = AddNoteViewModel.Instance!;
            _windowService = new WindowService();
            _databaseService = DatabaseService.Instance!;
            _localizationService = DatabaseService._localizationService;

            //Initialize selected Languages Object
            SelectedLang = null!;

            //Initialize lists
            noteList = new ObservableCollection<Note>();
            LangList = new List<Languages>(); 

            //Generate change-language menu dynamically, depending on files in Services/LanguageFile folder
            GenerateLang();

            //Populate NoteList using entries from DB
            //=> To be able to use xaml viewer inside VisualStudio, comment out _databaseService.InitializeNoteList()
            _databaseService.InitializeList();

            //Example entries to populate NoteList without DB
            //=> Use those as example entries inside xaml viewer of VisualStudio
            //NoteList.Add(new Note(1, "test", _databaseService.HighPriority, DateTime.Now.ToString("dd.MM.yyyy")));
            //NoteList.Add(new Note(2, "Zweiter Test", _databaseService.HighPriority, DateTime.Now.ToString("dd.MM.yyyy")));
        }

        //Open new AddNote Window
        [RelayCommand]
        private void OpenAddNoteWin()
        {
            _windowService.OpenWindow(new AddNoteWindow());
        }

        //Deletes current Note
        [RelayCommand]
        private void RemoveNote(Note note)
        {
            _databaseService.RemoveData(note.Id);
            //Reload database
            _databaseService.updateList();
        }

        //Search for all lang files and Icons and fill LangList
        private void GenerateLang()
        {
            //For each resource file in LanguageFile folder => Generate LanguageObject containing Icon
            foreach (string test in Directory.GetFiles(langPath, "*.resx"))
            {
                //Name is file name without 'Strings.' and ending (Strings.de.resx = de)
                string langName = Path.GetFileNameWithoutExtension(test).Remove(0, 8);
                //Path to Icon folder: workingDirectory + iconFolderPath in AppSettings
                string iconFolderPath = workingDirectory +
                                        ConfigurationManager.AppSettings["iconFolderPath"]!;
                //Get specific file based on langName
                var icon = Directory.GetFiles(iconFolderPath)
                                            .Where(currentFile => Path.GetFileName(currentFile).Contains(langName)).ToList();
                //Error if no Icon
                if (!icon.Any()) 
                {
                    throw new FileNotFoundException($"No Icon for language '{langName}' found");
                }
                //Add new Language object
                LangList.Add(new Languages(langName, icon[0]!));
            }
        }

        //Change translated button texts, headers, ...
        private void ChangeLang(string langCode) 
        {
            //Change culture = change language
            _localizationService!.SetCulture(langCode);
            //Change translated texts
            OnPropertyChanged(nameof(AddNoteBtn));
            OnPropertyChanged(nameof(IdHeader));
            OnPropertyChanged(nameof(DescHeader));
            OnPropertyChanged(nameof(PriorityHeader));
            OnPropertyChanged(nameof(DueDateHeader));

            //Update Database after language was changed
            _databaseService.updateList();
        }     
    }
}
