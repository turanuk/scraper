using System.Collections.Generic;
using System.Web.Mvc;
using Amazon.DynamoDB;
using Amazon.DynamoDB.DataModel;
using Amazon.DynamoDB.Model;
using ClSearcherFrontEnd.Models;

namespace ClSearcherFrontEnd.Controllers
{
    public class SearchTermController : Controller {
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;

        public SearchTermController() {
            _client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(_client);    
        }

        public ActionResult Index()
        {
            var response = _client.Scan(new ScanRequest() {TableName = "SearchTerms"});

            var model = new List<SearchTerm>();

            foreach (var item in response.ScanResult.Items) {
                var searchTerm = new SearchTerm() {Term = item["Term"].S};
                model.Add(searchTerm);
            }

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SearchTerm/Create

        [HttpPost]
        public ActionResult Create(SearchTerm searchterm)
        {
            if (ModelState.IsValid)
            {
                var searchTerm = new SearchTerm() { Term = searchterm.Term };
                _context.Save(searchTerm);

                return RedirectToAction("Index");
            }

            return View(searchterm);
        }

        public ActionResult Delete(string term) {
            var searchTerm = _context.Load<SearchTerm>(hashKey: term);
            _context.Delete(searchTerm);

            var searchDataToDelete = _context.Query<SearchData>(hashKeyValue: term);

            foreach (var item in searchDataToDelete) {
                _context.Delete<SearchData>(hashKey: item.Term, rangeKey: item.Link);
            }

            return RedirectToAction("Index");
        }
    }
}