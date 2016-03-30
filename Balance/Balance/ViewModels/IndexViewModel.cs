using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MVCModels.Models;

namespace Balance.ViewModels
{
    public class IndexViewModel
    {
        public ICollection<GroupListItemModel> Groups { get; set; }
    }
}