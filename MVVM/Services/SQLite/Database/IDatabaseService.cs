/**
Interface for all database interactions for NoteApp
=> Training Exercise
Author: Rico Anzer
*/
namespace NoteApp.MVVM.Services.SQLite.Database
{
    interface IDatabaseService
    {
        //Starting up Database, if it doesn't exist, create one
        void InitializeDatabase();
        //Add Note to Database
        void AddData(string description, string priority, string dueDate);
        //Fill NoteList with data from Database
        void InitializeNoteList();
        //Remove Note from Database
        void RemoveData(int id);
    }
}
