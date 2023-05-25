using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using NodesTestJob.Models;

namespace NodesTestJob.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataCreator _dataCreator;
        private readonly DataManager _dataManager;

        public IndexModel(DataCreator dataCreator, DataManager dataManager)
        {
            _dataCreator = dataCreator;
            _dataManager = dataManager;
        }

        public string Notes { get; set; }

        public void OnGet()
        {
            if (_dataCreator.GetCC() is string errorMessage) { ViewData["ErrorMessage"] = errorMessage; return; }
            Notes = _dataManager.Select(QuerySQL.GetFullNotes);
        }
    }
}