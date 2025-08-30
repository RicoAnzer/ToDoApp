using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToDoApp.MVVM.Services;
using ToDoApp.MVVM.Services.SQLite.Database;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

/**
ViewModel Class for AddNoteWindow in Note-App
Is Implementing INotifyDataErrorInfo Interface according to Microsofts documentation for error display:
https://learn.microsoft.com/en-us/previous-versions/windows/silverlight/dotnet-windows-silverlight/ee652637(v=vs.95)?redirectedfrom=MSDN
=> Training Exercise
Author: Rico Anzer
*/
namespace ToDoApp.MVVM.ViewModel
{
    public partial class AddNoteViewModel : ObservableObject, INotifyDataErrorInfo
    {
        //Instance of WindowsService
        private readonly WindowService _windowService;
        //Instance of DatabaseService
        private readonly DatabaseService _databaseService;
        //Instance of LocalizationService
        private readonly LocalizationService _localizationService;
        //Instance of this ViewModel
        public static AddNoteViewModel? Instance { get; private set; }

        //Dynamic margin for TextBox while errors are displayed
        [ObservableProperty]
        private Thickness textBoxErrorMargin;

        //Description for activity to do
        [ObservableProperty]
        public string? textBoxContent;
        //Date of creation of note
        [ObservableProperty]
        private DateTime calendarDate;
        //Assigned priority of note
        [ObservableProperty]
        private string? selectedPriority;

        //Strings to bind text and resource files
        //Both, NoteListWindow and AddNoteWindow store translated strings in LocalizationService and access theirs in ViewModel
        public string HighPriority => _localizationService.GetString("PriorityHigh");
        public string MediumPriority => _localizationService.GetString("PriorityMedium");
        public string LowPriority => _localizationService.GetString("PriorityLow");
        public string NoteDesc => _localizationService.GetString("HeaderDescription");
        public string NotePrio => _localizationService.GetString("HeaderPriority");
        public string NoteDate => _localizationService.GetString("HeaderDueDate");
        public string CancelNote => _localizationService.GetString("BtnCancelNote");
        public string SaveNote => _localizationService.GetString("BtnSaveNote");
        public string DescNoTextError => _localizationService.GetString("DescErrorNoText");
        public string DescMaxLengthError => _localizationService.GetString("DescErrorMaxLength");

        //List containing all 3 Priority strings
        [ObservableProperty]
        private ObservableCollection<string> priorities;

        public AddNoteViewModel()
        {
            //Initialize services
            Instance = this;
            _windowService = new WindowService();
            //_databaseService = new DatabaseService();
            _databaseService = DatabaseService.Instance!;
            //Use same instance of LocalizationService as NoteListWIndow for same translations
            _localizationService = NoteListWindowViewModel._localizationService!;

            //Subscribing to ErrorsChanged Event => part of INotifyDataErrorInfo
            ErrorsChanged += OnErrorsChanged!;

            //TextBoxErrorMargin contains margin of Description TextBox if errors occur
            TextBoxErrorMargin = new Thickness();

            //Default values for Priority, TextBoxContent and CalendarDate
            //=> To be able to use xaml viewer inside VisualStudio, comment out Priorities and SelectedPriority
            TextBoxContent = string.Empty;
            Priorities = [HighPriority, MediumPriority, LowPriority];
            SelectedPriority = Priorities[1];
            CalendarDate = DateTime.Now;
        }

        //Close current window
        [RelayCommand]
        private void CloseWindow()
        {
            _windowService.CloseWindow();
        }

        //Add new note to DB after click on button
        [RelayCommand]
        private void AddNote()
        {
            //Check if Description is correct
            ValidateTextBoxContent();

            //Error check: max numbers of database entries must be kept and TextBoxContent needs text
            //Otherwise, no Note added and error messages of TextBoxContent etc. will be displayed as defined at variables block
            if (TextBoxContent!.Length <= 2048 && TextBoxContent!.Length > 0)
            {
                //Add new note to database
                _databaseService.AddData(TextBoxContent!, SelectedPriority!, CalendarDate.ToString("dd.MM.yyyy"));
                //Update UI
                _databaseService.updateList();
                //Close window
                CloseWindow();
            }
        }

        //EventHandler for ErrorsChanged Event, part of INotifyDataErrorInfo Interface
        //Debug output to check if errors are correctly assigned
        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            Debug.WriteLine($"Fehler für Eigenschaft '{e.PropertyName}' haben sich geändert.");

            var errorService = sender as AddNoteViewModel;

            if (errorService != null)
            {
                var errors = GetErrors(e.PropertyName);
                Debug.WriteLine(errors);
                if (errors != null)
                {
                    Debug.WriteLine($"Aktuelle Fehler für {e.PropertyName}:");
                    foreach (var error in errors)
                    {
                        Debug.WriteLine($"- {error}");
                    }
                }
                else
                {
                    Debug.WriteLine($"Keine Fehler für {e.PropertyName}.");
                }
            }
        }

        //Adds all current errors for TextBox (Description Input)
        private void ValidateTextBoxContent()
        {
            //Clear past errors
            RemoveError(nameof(TextBoxContent), DescNoTextError);
            RemoveError(nameof(TextBoxContent), DescMaxLengthError);

            //Check if Description contains characters
            if (string.IsNullOrWhiteSpace(TextBoxContent))
            {
                AddError(nameof(TextBoxContent), DescNoTextError);
            }
            //Check if Description entry has permitted length for Database column
            if (TextBoxContent!.Length > 2048) 
            {
                AddError(nameof(TextBoxContent), DescMaxLengthError);
            }

            //Bottom margin for display of error
            int bottomMargin = 5;

            //Change Bottom margin, depending on existance of errors
            if (errors.ContainsKey(nameof(TextBoxContent))) 
            {
                bottomMargin = errors[nameof(TextBoxContent)].Count * 20;
            }
            //Dynamically change bottom margin of Description TextBox depending on number of errors
            TextBoxErrorMargin = new Thickness(5, 
                                               5,
                                               5,
                                               bottomMargin);
        }

        /**-------------------------------------------------------------------------------------------------------
        Start of implementing INotifyDataErrorInfo Interface => Allows adding and displaying custom errors
        ---------------------------------------------------------------------------------------------------------*/

        //Dictionary containing all errors
        //Each property has a list containing all existing errors
        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        //EventHandler to signal if an errors has been added or removed
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        //Signals changes in errors Dictionary (error has been added or removed)
        public void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
        //Adds a new error
        public void AddError(string propertyName, string error)
        {
            //If property isn't in Dictionary, add new entry with empty error list
            if (!errors.ContainsKey(propertyName))
            {
                errors[propertyName] = new List<string>();
            }
            //If existing error list doesn't contain error,
            //add new error and signal that new error has been added
            if (!errors[propertyName].Contains(error))
            {
                errors[propertyName].Add(error);
                RaiseErrorsChanged(propertyName);
            }
        }
        //Removes an existing error
        public void RemoveError(string propertyName, string error)
        {
            //Check if property exists in Dictionary and has error in it's error list
            if (errors.ContainsKey(propertyName) && errors[propertyName].Contains(error))
            {
                //Remove error from error list of property
                errors[propertyName].Remove(error);
                //If error list of property is now empty => Delete property
                if (errors[propertyName].Count == 0)
                {
                    errors.Remove(propertyName);
                }
                //Signal that an error has been removed
                RaiseErrorsChanged(propertyName);
            }
        }
        //Show all existing errors of a property
        public System.Collections.IEnumerable GetErrors(string? propertyName)
        {
            //If property is empty or doesn't exist => abort
            if (string.IsNullOrEmpty(propertyName) || !errors.ContainsKey(propertyName))
            {
                return null!;
            }
            //Show error list of property
            return errors[propertyName];
        }

        //True if there is at least one error
        public bool HasErrors
        {
            get { return errors.Count > 0; }
        }

        /**-------------------------------------------------------------------------------------------------------
        End implementing INotifyDataErrorInfo Interface => Allows adding and displaying custom errors
        ---------------------------------------------------------------------------------------------------------*/
    }
}
