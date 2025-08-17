using Microsoft.Data.Sqlite;
using NoteApp.MVVM.Model;
using NoteApp.MVVM.ViewModel;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;

/**
Interact with database for NoteApp
=> Training Exercise
Author: Rico Anzer
*/
namespace NoteApp.MVVM.Services.SQLite.Database
{
    class DatabaseService : IDatabaseService
    {
        //Name of database defined in App.config
        private static string dbName = ConfigurationManager.AppSettings["dbName"]!;
        //Path to workingDirectory
        private static string workingDirectory = Path.GetDirectoryName(Environment.ProcessPath)!.Split("\\bin")[0];
        //Path to database => combining Path of working directory and Path inside working directory defined in App.config
        private static string dbPath = workingDirectory + ConfigurationManager.AppSettings["dbPath"];

        //Create database if it doesn't exist, open database if it does
        public void InitializeDatabase()
        {
            //Connect to database, create database if it doesn't exist
            using (var db = new SqliteConnection($"Filename={dbPath + @"\" + dbName}"))
            {
                //Open database
                db.Open();

                //Create table if it doesn't exist
                string createTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Notes (" +
                    "Description VARCHAR(2048) NOT NULL," +
                    "Priority VARCHAR(15) NOT NULL," +
                    "DueDate VARCHAR(15) NOT NULL" +
                    ")";

                var createTable = new SqliteCommand(createTableCommand, db);

                createTable.ExecuteReader();
            }
        }

        //Add new note to database
        public void AddData(string description, string priority, string dueDate)
        {
            using (var db = new SqliteConnection($"Filename={dbPath + @"\" + dbName}"))
            {
                //Open database
                db.Open();

                var insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                //Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO Notes (Description, Priority, DueDate) VALUES (@Desc, @Prio, @Date);";
                insertCommand.Parameters.AddWithValue("@Desc", description);
                insertCommand.Parameters.AddWithValue("@Prio", priority);
                insertCommand.Parameters.AddWithValue("@Date", dueDate);

                //Error handling: If entry is empty => Error
                try
                {
                    //Execute AddData command
                    insertCommand.ExecuteReader();
                }
                catch(InvalidOperationException)
                {
                    Debug.WriteLine("Error: Description, Priority and Date must be set");
                }
   
            }
        }

        //Fill NoteList with all notes in db
        public void InitializeNoteList()
        {
            using (var db = new SqliteConnection($"Filename={dbPath + @"\" + dbName}"))
            {
                //Open database
                db.Open();

                //Select table columns you want to extract information from
                var selectCommand = new SqliteCommand
                    ("SELECT rowid, * FROM Notes", db);

                SqliteDataReader query = selectCommand.ExecuteReader();
               
                //For each entry, extract description, priority and creationDate and fill NoteList in NoteListWindowViewModel
                while (query.Read())
                {
                    ObservableCollection<Note> noteList = NoteListWindowViewModel.Instance!.NoteList;
                    Note note = new Note(query.GetInt32(0), query.GetString(1), query.GetString(2), query.GetString(3));
                    noteList.Add(note);
                   
                }
            }
        }

        //Remove Note from Database
        public void RemoveData(int id)
        {
            //Connect to database, create database if it doesn't exist
            using (var db = new SqliteConnection($"Filename={dbPath + @"\" + dbName}"))
            {
                //Open database
                db.Open();

                //1. Delete Note
                //2. Copy Database => New Database has realigned rowid
                //3. Delete old Database
                //4. Change name of new Database to name of old Database
                var deleteCommand = new SqliteCommand();
                var createNewTableCommand = new SqliteCommand();
                var dropOldTableCommand = new SqliteCommand();
                var renameTableCommand = new SqliteCommand();

                deleteCommand.Connection = db;
                createNewTableCommand.Connection = db;
                dropOldTableCommand.Connection = db;
                renameTableCommand.Connection = db;
                
                //Use parameterized query to prevent SQL injection attacks
                deleteCommand.CommandText = "DELETE FROM Notes WHERE rowid = @ID;";
                deleteCommand.Parameters.AddWithValue("@ID", id);

                //Recreate Table Notes to reset rowids
                createNewTableCommand.CommandText = "CREATE TABLE newNotes AS SELECT * FROM Notes;";
                dropOldTableCommand.CommandText = "DROP TABLE NOTES;";
                renameTableCommand.CommandText = "ALTER TABLE newNotes RENAME TO Notes;";
              
                deleteCommand.ExecuteReader();
                createNewTableCommand.ExecuteReader();
                dropOldTableCommand.ExecuteReader();
                renameTableCommand.ExecuteReader();
                
            }
        }
    }
}
