using System.Web.Mvc;
using Amazon.DynamoDB;
using Amazon.DynamoDB.DataModel;
using ClSearcherFrontEnd.Models;

namespace ClSearcherFrontEnd.Controllers
{
    public class SearchDataController : Controller
    {
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;

        public SearchDataController() {
            _client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(_client);    
        }

        public ActionResult Index(string term) {
            var searchData = _context.Query<SearchData>(hashKeyValue: term);

            return View(searchData);
        }
    }
}