using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToDoApp.MVVM.Model;
using ToDoApp.MVVM.Services;
using ToDoApp.MVVM.Services.SQLite.Database;
using ToDoApp.MVVM.View;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;

/**
ViewModel Class for TaskListWindow in Task-App
=> Training Exercise
Author: Rico Anzer
*/
namespace ToDoApp.MVVM.ViewModel
{
    public partial class TaskListWindowViewModel : ObservableObject
    {
        //Instance of WindowsService
        private readonly WindowService _windowService;
        //Instance of DatabaseService
        private readonly DatabaseService _databaseService;
        //Instance of LocalizationService
        public static LocalizationService? _localizationService;
        //Instance of AddTaskViewModel
        public static AddTaskViewModel? _AddTaskViewModel;

        //Instance of this ViewModel
        public static TaskListWindowViewModel? Instance { get; private set; }

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

        //List containing all Tasks to display
        [ObservableProperty]
        private ObservableCollection<ToDo> taskList;
        //List containing all Languages (each object has name and path to icon)
        public List<Languages> LangList { get; set; }

        //Strings to bind text and resource files
        //Both, TaskListWindow and AddTaskWindow store translated strings in LocalizationService and access theirs in ViewModel
        public string AddTaskBtn => _localizationService!.GetString("BtnAddTask");
        public string IdHeader => _localizationService!.GetString("HeaderID");
        public string DescHeader => _localizationService!.GetString("HeaderDescription");
        public string PriorityHeader => _localizationService!.GetString("HeaderPriority");
        public string DueDateHeader => _localizationService!.GetString("HeaderDueDate");
        public string HighPriority => _localizationService!.GetString("PriorityHigh");
        public string MediumPriority => _localizationService!.GetString("PriorityMedium");
        public string LowPriority => _localizationService!.GetString("PriorityLow");

        //Constructor
        public TaskListWindowViewModel()
        {
            //Initialize services
            Instance = this;
            _AddTaskViewModel = AddTaskViewModel.Instance!;
            _windowService = new WindowService();
            _databaseService = DatabaseService.Instance!;
            _localizationService = DatabaseService._localizationService;

            //Initialize selected Languages Object
            SelectedLang = null!;

            //Initialize lists
            taskList = new ObservableCollection<ToDo>();
            LangList = new List<Languages>(); 

            //Generate change-language menu dynamically, depending on files in Services/LanguageFile folder
            GenerateLang();

            //Populate TaskList using entries from DB
            //=> Potentially redundant
            //_databaseService.InitializeList();
        }

        //Open new AddTask Window
        [RelayCommand]
        private void OpenAddTaskWin()
        {
            _windowService.OpenWindow(new AddTaskWindow());
        }

        //Deletes current Task
        [RelayCommand]
        private void RemoveTask(ToDo Task)
        {
            _databaseService.RemoveData(Task.Id);
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
            OnPropertyChanged(nameof(AddTaskBtn));
            OnPropertyChanged(nameof(IdHeader));
            OnPropertyChanged(nameof(DescHeader));
            OnPropertyChanged(nameof(PriorityHeader));
            OnPropertyChanged(nameof(DueDateHeader));

            //Update Database after language was changed
            _databaseService.updateList();
        }     
    }
}
