namespace MVC_News.MVC.Models;

public class ControllerExceptionViewModel : BaseViewModel
{
    public ControllerExceptionViewModel(string? excepetionMessage)
    {
        ExcepetionMessage = excepetionMessage;
    }

    public string? ExcepetionMessage { get; }
}