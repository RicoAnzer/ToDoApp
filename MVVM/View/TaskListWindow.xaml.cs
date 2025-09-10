using ToDoApp.MVVM.Model;
using ToDoApp.MVVM.Services;
using ToDoApp.MVVM.Services.SQLite.Database;
using ToDoApp.MVVM.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ToDoApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for TaskListWindow.xaml
    /// Contains custom events for UI Elements (like custom sorting of DataGrid)
    /// => Training Exercise
    ///Author: Rico Anzer
    /// </summary>
    partial class TaskListWindow : Window
    {
        //Instances
        public static LocalizationService? _localizationService;
        public static TaskListWindowViewModel? _TaskListWindowViewModel;
        public static DatabaseService? _dataBaseService;

        //Toggle for ascending/descending sorting of DataGrid items
        private bool sortDirAsc;

        public TaskListWindow()
        {
            InitializeComponent();
            //Initialize services
            _localizationService = DatabaseService._localizationService!;
            _TaskListWindowViewModel = TaskListWindowViewModel.Instance!;
            _dataBaseService = DatabaseService.Instance!;
            //Initalize variables
            sortDirAsc = false;
        }

        //Deselect active DataGrid cell after click
        //Event is bound to Grid "MenuBackground" (background for main window), not DataGrid
        //=> Allows event to fire whenever empty space is clicked
        private void DataGridDeselect(object sender, MouseButtonEventArgs e)
        {
            if (sender != null) 
            {    
                //Number of children of Grid
                int count = VisualTreeHelper.GetChildrenCount((DependencyObject?)sender);
                //Search all children
                for (int i = 0; i < count; i++) 
                {
                    DependencyObject child = VisualTreeHelper.GetChild((DependencyObject)sender, i);
                    //Search for DataGrid among children
                    if (child is DataGrid)
                    {
                        //Convert to DataGrid for access of internal data
                        DataGrid? grid = child as DataGrid;
                        //If at least 1 selected row
                        if (grid!.SelectedItems.Count > 0) 
                        {
                            //Number of current selected rows
                            int selected = grid!.SelectedItems.Count;
                            //Iterate each selected row and deselect them
                            for (int x = 0; x < selected; x++) 
                            {
                                DataGridRow? row = grid!.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                                row!.IsSelected = false;
                                //If click while editing, save edit
                                grid.CommitEdit();
                            }       
                        }
                    }
                }
            }
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
                    SortId(_TaskListWindowViewModel!.TaskList, e);
                    break;
                case "Description":
                    SortDescription(_TaskListWindowViewModel!.TaskList, e);
                    break;
                case "Priority":
                    SortPrioriy(_TaskListWindowViewModel!.TaskList, e);
                    break;
                case "Date":
                    SortDate(_TaskListWindowViewModel!.TaskList, e);
                    break;
            }     
        }

        //Edit description of existing database entry by editing description cell of DataGrid
        private void DataGridCellEdit(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Access and temporarily save content of active cell (not yet saved in Task or Database)
            TextBox? tb = new TextBox();
            if (e.EditingElement is TextBox)
            {
                tb = e.EditingElement as TextBox;
            }

            //Access Task bound to row
            ToDo editObj = (ToDo)e.EditingElement.DataContext;
            //Edit data entry using id of bound Task and changed text of cell
            _dataBaseService!.editData(editObj.Id, tb!.Text);
        }

        //Toggle sort direcrtion
        private void ToggleSortDir(ref bool sortDir) 
        {
            sortDir = !sortDir;
        }

        /**
         * -------------------------------------------------------------------------------------------------------
         * Custom sorting for id
         * -------------------------------------------------------------------------------------------------------
         **/
        private void SortId(ObservableCollection<ToDo> listToSort, DataGridSortingEventArgs e)
        {
            //Secondary list for sorting
            List<ToDo> sortedList = new List<ToDo>();

            //Toggle sort direction for next sorting
            ToggleSortDir(ref sortDirAsc);

            //Sort in ascending or descending order
            //Sort by id => sort by number (1, 2, 3...)
            sortedList = sortDirAsc ? listToSort.OrderBy(x => x.Id)
                                       .ToList()

                                     : listToSort.OrderByDescending(x => x.Id)
                                       .ToList();

            //Change sort direction UI of Datagrid
            e.Column.SortDirection = sortDirAsc ? ListSortDirection.Ascending : ListSortDirection.Descending;

            //Update UI list: Clear list used as ItemsSource and refill ordered items
            listToSort.Clear();
            foreach (ToDo item in sortedList)
            {
                listToSort.Add(item);
            }
        }

        /**
         * -------------------------------------------------------------------------------------------------------
         * Custom sorting for description
         * -------------------------------------------------------------------------------------------------------
         **/
        private void SortDescription(ObservableCollection<ToDo> listToSort, DataGridSortingEventArgs e)
        {
            //Secondary list for sorting
            List<ToDo> sortedList = new List<ToDo>();

            //Toggle sort direction for next sorting
            ToggleSortDir(ref sortDirAsc);

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
            foreach (ToDo item in sortedList)
            {
                listToSort.Add(item);
            }
        }

        /**
         * -------------------------------------------------------------------------------------------------------
         * Custom sorting for priority
         * -------------------------------------------------------------------------------------------------------
         **/
        private void SortPrioriy(ObservableCollection<ToDo> listToSort, DataGridSortingEventArgs e) 
        {
            //Secondary list for sorting
            List<ToDo> sortedList = new List<ToDo>();

            //Toggle sort direction for next sorting
            ToggleSortDir(ref sortDirAsc);

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
            foreach (ToDo item in sortedList) 
            {
                listToSort.Add(item);
            }
        }

        /**
         * -------------------------------------------------------------------------------------------------------
         * Custom sorting for date
         * -------------------------------------------------------------------------------------------------------
         **/
        private void SortDate(ObservableCollection<ToDo> listToSort, DataGridSortingEventArgs e) 
        {
            //Secondary list for sorting
            List<ToDo> sortedList = new List<ToDo>();

            //Toggle sort direction for next sorting
            ToggleSortDir(ref sortDirAsc);

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
            foreach (ToDo item in sortedList)
            {
                listToSort.Add(item);
            }
        }
    }
}