using System.ComponentModel.DataAnnotations;

namespace MVCModels.Models
{
    public class AddGroupModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}