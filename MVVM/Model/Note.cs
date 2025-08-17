using CommunityToolkit.Mvvm.ComponentModel;

/**
 Note Class for Note-App
=> Training Exercise
Author: Rico Anzer
*/
namespace NoteApp.MVVM.Model
{
    partial class Note : ObservableObject
    {
        //Individual ID
        [ObservableProperty]
        private int iD;
        //Description for activity to do
        [ObservableProperty]
        private string? description;
        //Date of creation of note
        [ObservableProperty]
        private string? dueDate;
        //Assigned priority of note
        [ObservableProperty]
        private string priority;

        //Constructor
        public Note(int _id, string? _description, string _priority, string _dueDate)
        {
            iD = _id;
            Description = _description;
            Priority = _priority!;
            DueDate = _dueDate;
        }
    }
}
