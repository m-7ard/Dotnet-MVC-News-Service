namespace MVC_News.MVC.Models.Partials;

public class FormFieldVPartialiewModel
{
    public FormFieldVPartialiewModel(string label, List<string>? errors, string partialRoute, object partialProps, bool isRow = false)
    {
        Label = label;
        Errors = errors;
        PartialRoute = partialRoute;
        PartialProps = partialProps;
        IsRow = isRow;
    }

    public string Label { get; set; }
    public List<string>? Errors { get; set; }
    public string PartialRoute { get; set; }
    public object PartialProps { get; set; }
    public bool IsRow { get; set; }
}