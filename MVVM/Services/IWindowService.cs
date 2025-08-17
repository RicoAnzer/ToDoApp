using System.Windows;

/**
Interface to open and close new windows for NoteApp
=> Training Exercise
Author: Rico Anzer
*/
namespace NoteApp.MVVM.Services
{
    interface IWindowService
    {
        //Open a new window
        void OpenWindow(Window _window);
        //Close current window
        void CloseWindow();
    }
}
