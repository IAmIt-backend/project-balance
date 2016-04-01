using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCModels.Models
{
    public class AddUserToGroupModel
    {
        [Required]
        public string Email { get; set; }
    }
}
