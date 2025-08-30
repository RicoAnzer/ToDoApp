using CommunityToolkit.Mvvm.ComponentModel;

/**
 Note Class for Note-App
=> Training Exercise
Author: Rico Anzer
*/
namespace ToDoApp.MVVM.Model
{
    public partial class ToDo : ObservableObject
    {
        //Individual ID
        [ObservableProperty]
        private int id;
        //Description for activity to do
        [ObservableProperty]
        private string? description;
        //Date of creation of note
        [ObservableProperty]
        private string? date;
        //Priority (High, Medium, Low) for display in UI
        [ObservableProperty]
        string priority;
        //Index of above priority (High - 0, Medium - 1, Low - 2) for sorting
        [ObservableProperty]
        int priorityIndex;

        //Constructor
        public ToDo(int _id, string? _description, string _date, string _priority, int _priorityIndex)
        {
            Id = _id;
            Description = _description;
            Date = _date;
            Priority = _priority;
            PriorityIndex = _priorityIndex;
            
        }
    }
}
