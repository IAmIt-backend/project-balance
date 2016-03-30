using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCModels.Models
{
    public class GroupListItemModel
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}