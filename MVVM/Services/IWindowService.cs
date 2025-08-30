using System.Windows;

/**
Interface to open and close new windows for ToDoApp
=> Training Exercise
Author: Rico Anzer
*/
namespace ToDoApp.MVVM.Services
{
    interface IWindowService
    {
        //Open a new window
        void OpenWindow(Window _window);
        //Close current window
        void CloseWindow();
    }
}
