using NoteApp.MVVM.Model;
using NoteApp.MVVM.Services;
using NoteApp.MVVM.Services.SQLite.Database;
using NoteApp.MVVM.ViewModel;
using System.Collections.ObjectModel;
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
            switch (e.Column.SortMemberPath) 
            {
                //Custom sorting
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
            if (sortDirAsc)
            {
                //Sort in ascending order
                sortedList = listToSort.OrderBy(x => x.Id)
                                        .ToList();
                //Change sort direction UI of Datagrid to ascending
                e.Column.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }
            else
            {
                //Sort in descending order
                sortedList = listToSort.OrderByDescending(x => x.Id)
                                        .ToList();
                //Change sort direction UI of Datagrid to descending
                e.Column.SortDirection = System.ComponentModel.ListSortDirection.Descending;
            }

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
            if (sortDirAsc)
            {
                //Sort in ascending order
                sortedList = listToSort.OrderBy(x => x.Description)
                                        .ToList();
                //Change sort direction UI of Datagrid to ascending
                e.Column.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }
            else
            {
                //Sort in descending order
                sortedList = listToSort.OrderByDescending(x => x.Description)
                                        .ToList();
                //Change sort direction UI of Datagrid to descending
                e.Column.SortDirection = System.ComponentModel.ListSortDirection.Descending;
            }

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
            if (sortDirAsc)
            {
                //Sort in ascending order
                sortedList = listToSort.OrderBy(x => x.PriorityIndex)
                                       .ThenByDescending(x => {
                                           DateTime date;
                                           DateTime.TryParse(x.Date, out date);
                                           return date;
                                       })
                                       .ThenBy(x => x.Description)
                                       .ToList();
                //Change sort direction UI of Datagrid to ascending
                e.Column.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }
            else 
            {
                //Sort in descending order
                sortedList = listToSort.OrderByDescending(x => x.PriorityIndex)
                                       .ThenByDescending(x => {
                                           DateTime date;
                                           DateTime.TryParse(x.Date, out date);
                                           return date;
                                       })
                                       .ThenBy(x => x.Description)
                                       .ToList();
                //Change sort direction UI of Datagrid to descending
                e.Column.SortDirection = System.ComponentModel.ListSortDirection.Descending;
            }

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
            if (sortDirAsc)
            {
                //Sort in ascending order
                sortedList = listToSort.OrderByDescending(x => {
                                           DateTime date;
                                           DateTime.TryParse(x.Date, out date);
                                           return date;
                                         })
                                        .ThenBy(x => x.Priority)
                                        .ThenBy(x => x.Description)
                                        .ToList();
                //Change sort direction UI of Datagrid to ascending
                e.Column.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }
            else
            {
                //Sort in descending order
                sortedList = listToSort.OrderBy(x => {
                                            DateTime date;
                                            DateTime.TryParse(x.Date, out date);
                                            return date;
                                          })
                                        .ThenBy(x => x.Priority)
                                        .ThenBy(x => x.Description)
                                        .ToList();
                //Change sort direction UI of Datagrid to descending
                e.Column.SortDirection = System.ComponentModel.ListSortDirection.Descending;
            }

            //Update UI list: Clear list used as ItemsSource and refill ordered items
            listToSort.Clear();
            foreach (Note item in sortedList)
            {
                listToSort.Add(item);
            }
        }
    }
}