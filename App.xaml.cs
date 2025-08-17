using NoteApp.MVVM.Services.SQLite.Database;
using System.Windows;

namespace NoteApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Instance of DatabaseService
        private readonly DatabaseService _databaseService = new();
        public App()
        {
            this.InitializeComponent();
            _databaseService.InitializeDatabase();
        }
    }

}
