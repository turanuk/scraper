using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Amazon.DynamoDB.DataModel;

namespace ClSearcherFrontEnd.Models {
    public class MyContext : DbContext {
        public DbSet<SearchTerm> SearchTerms { get; set; }

        public DbSet<SearchData> SearchDatas { get; set; }
    }

    [DynamoDBTable("SearchTerms")]
    public class SearchTerm {
        [Key]
        [DynamoDBHashKey]
        [Display(Name = "Search Term")]
        public string Term { get; set; }
    }

    [DynamoDBTable("SearchData")]
    public class SearchData {
        [Key]
        [DynamoDBHashKey]
        [Display(Name = "Search Term")]
        public string Term { get; set; }
        [DynamoDBRangeKey]
        public string Link { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
    }
}