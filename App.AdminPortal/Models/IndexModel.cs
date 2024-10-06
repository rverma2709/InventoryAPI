namespace App.AdminPortal.Models
{
    public class IndexModel
    {
        public IndexModel()
        {
            hasExport = false;
        }
        public object model { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string PartialViewName { get; set; }
        public bool hasExport { get; set; }
    }
}
