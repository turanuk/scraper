var request = require('request'),
  jsdom = require('jsdom'),
  dynode = require('dynode'),
  async = require('async');

//Core logic for processing a search term
var processSearchTerm = function (searchTerm) {
  async.waterfall([
    //Initial hit to query the URL provided
    function (callback) {
      console.log('Issuing request for', searchTerm);
      request({uri: 'http://seattle.craigslist.org/search/?areaID=2&subAreaID=&query=' + searchTerm + '&catAbb=sss'}, function (err, response, body) {
        var self = this;
        self.items = new Array();
        if (err || !response || response.statusCode !== 200) {
          callback(err, response.statusCode);
        } else {
          callback(null, body);
        }
      }); 
    },
    //Parse information out of the URL
    function (body, callback) {
      jsdom.env({
        html: body,
        scripts: ['https://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js'],
      }, function (err, window) {
        var $ = window.jQuery;
        var items = [];
        $('p.row').each(function (index, element) {
          var link = $(element).children('a');
          var href = link.attr('href');
          var title = link.html();
          var price = $(element).children('.itempp').html();
          var additionalInfo = $(element).children('.itempn').children('font').html();
          if (additionalInfo) { 
            title += additionalInfo; 
          }
          items.push({ "Term": searchTerm, "Link": href, "Title": title, "Price": price });
        });
        callback(null, items);
      });
    },
    //Push found items back into the database
    function (items, callback) {
      //Construct batch write data structure
      var writes = { "SearchData" : []}
      var i = 0;
      for (i = 0; i < 25; i++) {
        writes.SearchData.push({ put : items[i]});
      }
      dynode.batchWriteItem(writes, function (err) {
        if (err) {
          callback(err);
        } else {
          callback(null);
        }
      });
    }
  ], function (err, result) {
    if (err) {
      if (!result) {
        console.log('Error:', err.message);
      } else {
        console.log('HTTP Error, status code:', result);
      }
    } else {
      console.log('Finished with', searchTerm);
    }
  });
}

//Set up dynode
dynode.auth({accessKeyId: "AKIAJJXSO4YKURKU7QKQ", secretAccessKey: "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"});

//Get terms to hit from the database, and then run queries on them
dynode.scan('SearchTerms', function (err, result) {
  if (err) {
    console.log(err.message);
  } else {
    var i = 0;
    for (i = 0; i < result.length; i++) {
      processSearchTerm(result[i].Term);
    }
  }
});