using Microsoft.Data.Sqlite;
using ToDoApp.MVVM.Model;
using ToDoApp.MVVM.ViewModel;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;

/**
Interact with database for ToDoApp
=> Training Exercise
Author: Rico Anzer
*/
namespace ToDoApp.MVVM.Services.SQLite.Database
{
   public class DatabaseService : IDatabaseService
    {
        //Instance of this Service
        public static DatabaseService? Instance { get; private set; }
        //Instance of LocalizationService
        public static LocalizationService? _localizationService;

        //Name of database defined in App.config
        private static string dbName = ConfigurationManager.AppSettings["dbName"]!;
        //Path to workingDirectory
        private static string workingDirectory = Path.GetDirectoryName(Environment.ProcessPath)!.Split("\\bin")[0];
        //Path to database => combining Path of working directory and Path inside working directory defined in App.config
        private static string dbPath = workingDirectory + ConfigurationManager.AppSettings["dbPath"];

        public string HighPriority => _localizationService!.GetString("PriorityHigh");
        public string MediumPriority => _localizationService!.GetString("PriorityMedium");
        public string LowPriority => _localizationService!.GetString("PriorityLow");

        //Constructor
        public DatabaseService()
        {
            Instance = this;
            //Use same instance of LocalizationService as TaskListWIndow for same translations
            _localizationService = new LocalizationService();
        }

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
                    "EXISTS Tasks (" +
                    "Description VARCHAR(2048) NOT NULL," +
                    "Priority VARCHAR(15) NOT NULL," +
                    "DueDate VARCHAR(15) NOT NULL" +
                    ")";

                var createTable = new SqliteCommand(createTableCommand, db);

                createTable.ExecuteReader();
            }
        }

        //Add new Task to database
        public void AddData(string description, string priority, string date)
        {
            string newPrio = string.Empty;

            //Save Priority in same format using palceholders, regardless of used language
            //=> Allows later back translation
            if (priority == HighPriority)
            {
                newPrio = "HighPriority";
            }
            else if (priority == MediumPriority)
            {
                newPrio = "MediumPriority";
            }
            else if (priority == LowPriority)
            {
                newPrio = "LowPriority";
            }

            using (var db = new SqliteConnection($"Filename={dbPath + @"\" + dbName}"))
            {
                //Open database
                db.Open();

                var insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                //Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO Tasks (Description, Priority, DueDate) VALUES (@Desc, @Prio, @Date);";
                insertCommand.Parameters.AddWithValue("@Desc", description);
                insertCommand.Parameters.AddWithValue("@Prio", newPrio);
                insertCommand.Parameters.AddWithValue("@Date", date);

                //Error handling: If entry is empty => Error
                try
                {
                    //Execute AddData command
                    insertCommand.ExecuteReader();
                }
                catch (InvalidOperationException)
                {
                    Debug.WriteLine("Error: Description, Priority and Date must be set");
                }
            }
        }

        //Rebuild TaskList with all Tasks in db
        //=> Shows content of Tasks in UI
        public void InitializeList()
        {
            using (var db = new SqliteConnection($"Filename={dbPath + @"\" + dbName}"))
            {
                //Open database
                db.Open();

                //Select table columns you want to extract information from
                var selectCommand = new SqliteCommand
                    ("SELECT rowid, * FROM Tasks", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                //For each entry, extract description, creationDate and priority and fill TaskList in TaskListWindowViewModel
                //query.GetString(0) = rowId,
                //query.GetString(1) = description
                //query.GetString(2) = priority
                //query.GetString(3) = date;

                while (query.Read())
                {
                    string priority = string.Empty;
                    int index = 0;
                    //Use placeholder of priority in AddData() for translation purposes
                    //=> Allows translation of all Priorities in same language, regardless which language was used to write Task
                    //For example: If Tasks were written while English language was selected, once German is chosen,
                    //Priorities are getting translation in German as well
                    switch (query.GetString(2))
                    {
                        case "HighPriority":
                            priority = HighPriority;
                            break;
                        case "MediumPriority":
                            priority = MediumPriority;
                            index = 1;
                            break;
                        case "LowPriority":
                            priority = LowPriority;
                            index = 2;
                            break;
                    }

                    ObservableCollection<ToDo> TaskList = TaskListWindowViewModel.Instance!.TaskList;
                    //Create and add Tasks to list
                    ToDo Task = new ToDo(query.GetInt32(0), query.GetString(1), query.GetString(3), priority, index);
                    TaskList.Add(Task);
                }
            }
        }

        //Remove Task from Database
        public void RemoveData(int id)
        {
            //Connect to database, create database if it doesn't exist
            using (var db = new SqliteConnection($"Filename={dbPath + @"\" + dbName}"))
            {
                //Open database
                db.Open();

                //1. Delete Task
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
                deleteCommand.CommandText = "DELETE FROM Tasks WHERE rowid = @ID;";
                deleteCommand.Parameters.AddWithValue("@ID", id);

                //Recreate Table Tasks to reset rowids
                createNewTableCommand.CommandText = "CREATE TABLE newTasks AS SELECT * FROM Tasks;";
                dropOldTableCommand.CommandText = "DROP TABLE TaskS;";
                renameTableCommand.CommandText = "ALTER TABLE newTasks RENAME TO Tasks;";

                deleteCommand.ExecuteReader();
                createNewTableCommand.ExecuteReader();
                dropOldTableCommand.ExecuteReader();
                renameTableCommand.ExecuteReader();
            }
        }

        //Updating description of existing database entry
        public void editData(int id, string description)
        {
            //Connect to database, create database if it doesn't exist
            using (var db = new SqliteConnection($"Filename={dbPath + @"\" + dbName}"))
            {
                //Open database
                db.Open();

                //Edit database entry
                var editCommand = new SqliteCommand();

                editCommand.Connection = db;

                //Use parameterized query to prevent SQL injection attacks
                editCommand.CommandText = "UPDATE Tasks SET Description = @Desc WHERE rowid = @ID;";
                editCommand.Parameters.AddWithValue("@ID", id);
                editCommand.Parameters.AddWithValue("@Desc", description);

                editCommand.ExecuteReader();
            }
        }

        //Recreates TaskList to update Tasks, if their content changed
        public void updateList()
        {
            ObservableCollection<ToDo> TaskList = TaskListWindowViewModel.Instance!.TaskList;
            //Delete old information
            TaskList.Clear();
            //Recreate TaskList with updated information of Tasks
            InitializeList();
        }
    }
}
