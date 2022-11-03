using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Update_Server.Data;
namespace Update_Server.Pages
{
    public class ChangelogModel : PageModel
    {
        private readonly string _key;
        public Data.Project Project { get; private set; }
        public void OnGet(string key)
        {
            Project = Data.Project.GetProject(key);
        }
    }
}
