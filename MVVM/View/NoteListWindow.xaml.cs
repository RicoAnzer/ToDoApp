using NoteApp.MVVM.Services;
using NoteApp.MVVM.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace NoteApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for NoteListWindow.xaml
    /// </summary>
    partial class NoteListWindow : Window
    {
        //Instance of LocalizationService
        public static LocalizationService? _localizationService;

        //Priority Strings for custom sorting logic
        public string highPriority => _localizationService!.GetString("PriorityHigh");
        public string mediumPriority => _localizationService!.GetString("PriorityMedium");
        public string lowPriority => _localizationService!.GetString("PriorityLow");

        public NoteListWindow()
        {
            InitializeComponent();
            //Initialize services
            _localizationService = NoteListWindowViewModel._localizationService!;
        }

        //Custom Sorting 
        private void DataGridHeaderSort(object sender, DataGridSortingEventArgs e)
        {
            Debug.WriteLine(highPriority);
        }
    }
}