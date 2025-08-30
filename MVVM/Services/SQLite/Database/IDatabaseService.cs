/**
Interface for all database interactions for ToDoApp
=> Training Exercise
Author: Rico Anzer
*/
namespace ToDoApp.MVVM.Services.SQLite.Database
{
    interface IDatabaseService
    {
        //Starting up Database, if it doesn't exist, create one
        void InitializeDatabase();
        //Add Note to Database
        void AddData(string description, string priority, string dueDate);
        //Fill NoteList with data from Database
        void InitializeList();
        //Remove Note from Database
        void RemoveData(int id);
    }
}
