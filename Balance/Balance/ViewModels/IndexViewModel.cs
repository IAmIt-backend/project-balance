using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace Balance.ViewModels
{
    public class IndexViewModel
    {
        public Dictionary<ObjectId, string> Groups { get; set; }
    }
}