using NoteApp.MVVM.Model;
using NoteApp.MVVM.Services;
using NoteApp.MVVM.Services.SQLite.Database;
using NoteApp.MVVM.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace NoteApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for NoteListWindow.xaml
    /// Contains custom events for UI Elements (like custom sorting of DataGrid)
    /// => Training Exercise
    ///Author: Rico Anzer
    /// </summary>
    partial class NoteListWindow : Window
    {
        //Instances
        public static LocalizationService? _localizationService;
        public static NoteListWindowViewModel? _noteListWindowViewModel;
        public static DatabaseService? _dataBaseService;

        //Toggle for ascending/descending sorting of DataGrid items
        private bool sortDirAsc;

        public NoteListWindow()
        {
            InitializeComponent();
            //Initialize services
            _localizationService = DatabaseService._localizationService!;
            _noteListWindowViewModel = NoteListWindowViewModel.Instance!;
            _dataBaseService = DatabaseService.Instance!;
            //Initalize variables
            sortDirAsc = false;
        }

        //Custom Sorting Event
        private void DataGridHeaderSort(object sender, DataGridSortingEventArgs e)
        {
            //Deactivate default sorting
            e.Handled = true;
            //Custom sorting
            switch (e.Column.SortMemberPath) 
            {
                case "Id":
                    sortId(_noteListWindowViewModel!.NoteList, e);
                    break;
                case "Description":
                    sortDescription(_noteListWindowViewModel!.NoteList, e);
                    break;
                case "Priority":
                    sortPrioriy(_noteListWindowViewModel!.NoteList, e);
                    break;
                case "Date":
                    sortDate(_noteListWindowViewModel!.NoteList, e);
                    break;
            }     
        }

        //Edit description of existing database entry by editing description cell of DataGrid
        private void DataGridCellEdit(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Access and temporarily save content of active cell (not yet saved in Note or Database)
            TextBox? tb = new TextBox();
            if (e.EditingElement is TextBox)
            {
                tb = e.EditingElement as TextBox;
            }

            //Access Note bound to row
            Note editObj = (Note)e.EditingElement.DataContext;
            //Edit data entry using id of bound Note and changed text of cell
            _dataBaseService!.editData(editObj.Id, tb!.Text);
        }

        //Toggle sort direcrtion
        private void toggleSortDir(ref bool sortDir) 
        {
            sortDir = !sortDir;
        }

        /**
         * -------------------------------------------------------------------------------------------------------
         * Custom sorting for id
         * -------------------------------------------------------------------------------------------------------
         **/
        private void sortId(ObservableCollection<Note> listToSort, DataGridSortingEventArgs e)
        {
            //Secondary list for sorting
            List<Note> sortedList = new List<Note>();

            //Toggle sort direction for next sorting
            toggleSortDir(ref sortDirAsc);

            //Sort in ascending or descending order
            //Sort by id => sort by number (1, 2, 3...)
            sortedList = sortDirAsc ? listToSort.OrderBy(x => x.Id)
                                       .ToList()

                                     : listToSort.OrderByDescending(x => x.Id)
                                       .ToList();

            //Update UI list: Clear list used as ItemsSource and refill ordered items
            listToSort.Clear();
            foreach (Note item in sortedList)
            {
                listToSort.Add(item);
            }
        }

        /**
         * -------------------------------------------------------------------------------------------------------
         * Custom sorting for description
         * -------------------------------------------------------------------------------------------------------
         **/
        private void sortDescription(ObservableCollection<Note> listToSort, DataGridSortingEventArgs e)
        {
            //Secondary list for sorting
            List<Note> sortedList = new List<Note>();

            //Toggle sort direction for next sorting
            toggleSortDir(ref sortDirAsc);

            //Sort in ascending or descending order
            //Sort by description => sort by first letter, (a first, then b, then c, ...)
            sortedList = sortDirAsc ? listToSort.OrderBy(x => x.Description)
                                       .ToList()

                                     : listToSort.OrderByDescending(x => x.Description)
                                       .ToList();

            //Change sort direction UI of Datagrid
            e.Column.SortDirection = sortDirAsc ? ListSortDirection.Ascending : ListSortDirection.Descending;
        
            //Update UI list: Clear list used as ItemsSource and refill ordered items
            listToSort.Clear();
            foreach (Note item in sortedList)
            {
                listToSort.Add(item);
            }
        }

        /**
         * -------------------------------------------------------------------------------------------------------
         * Custom sorting for priority
         * -------------------------------------------------------------------------------------------------------
         **/
        private void sortPrioriy(ObservableCollection<Note> listToSort, DataGridSortingEventArgs e) 
        {
            //Secondary list for sorting
            List<Note> sortedList = new List<Note>();

            //Toggle sort direction for next sorting
            toggleSortDir(ref sortDirAsc);

            //Sort in ascending or descending order
            //First sort by priority => high - medium - low
            //then by date => newest date first
            //then by description => sort by first letter, (a first, then b, then c, ...)
            sortedList = sortDirAsc ? listToSort.OrderBy(x => x.PriorityIndex)
                                       .ThenByDescending(x => {
                                           DateTime date;
                                           DateTime.TryParse(x.Date, out date);
                                           return date;
                                       })
                                       .ThenBy(x => x.Description)
                                       .ToList()

                                     : listToSort.OrderByDescending(x => x.PriorityIndex)
                                       .ThenByDescending(x => {
                                           DateTime date;
                                           DateTime.TryParse(x.Date, out date);
                                           return date;
                                       })
                                       .ThenBy(x => x.Description)
                                       .ToList();

            //Change sort direction UI of Datagrid
            e.Column.SortDirection = sortDirAsc ? ListSortDirection.Ascending : ListSortDirection.Descending;

            //Update UI list: Clear list used as ItemsSource and refill ordered items
            listToSort.Clear();
            foreach (Note item in sortedList) 
            {
                listToSort.Add(item);
            }
        }

        /**
         * -------------------------------------------------------------------------------------------------------
         * Custom sorting for date
         * -------------------------------------------------------------------------------------------------------
         **/
        private void sortDate(ObservableCollection<Note> listToSort, DataGridSortingEventArgs e) 
        {
            //Secondary list for sorting
            List<Note> sortedList = new List<Note>();

            //Toggle sort direction for next sorting
            toggleSortDir(ref sortDirAsc);

            //Sort in ascending or descending order
            //First sort by date => newest date first
            //then by priority => high - medium - low
            //then by description => sort by first letter, (a first, then b, then c, ...)
            sortedList = sortDirAsc ? listToSort.OrderByDescending(x => {
                                        DateTime date;
                                        DateTime.TryParse(x.Date, out date);
                                        return date;
                                       })
                                        .ThenBy(x => x.Priority)
                                        .ThenBy(x => x.Description)
                                        .ToList()

                                     : listToSort.OrderBy(x => {
                                         DateTime date;
                                         DateTime.TryParse(x.Date, out date);
                                         return date;
                                       })
                                        .ThenBy(x => x.Priority)
                                        .ThenBy(x => x.Description)
                                        .ToList();

            //Change sort direction UI of Datagrid
            e.Column.SortDirection = sortDirAsc ? ListSortDirection.Ascending : ListSortDirection.Descending;
            
            //Update UI list: Clear list used as ItemsSource and refill ordered items
            listToSort.Clear();
            foreach (Note item in sortedList)
            {
                listToSort.Add(item);
            }
        }
    }
}