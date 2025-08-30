using System.Windows;

/**
Open and close new windows for ToDoApp
=> Training Exercise
Author: Rico Anzer
*/
namespace ToDoApp.MVVM.Services
{
    public class WindowService : IWindowService
    {
        //Initialize and show new window
        public void OpenWindow(Window _window)
        {
            _window.ShowDialog();
        }

        //Get reference of current window and close
        public void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            window?.Close();
        }
    }
}
