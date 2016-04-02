using MVCModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Balance.ViewModels
{
    public class InvitationsViewModel
    {
        public ICollection<InvitationItemModel> ItemModels { get; set; }
    }
}