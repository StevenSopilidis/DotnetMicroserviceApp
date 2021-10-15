using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommandService.Models
{
    public class Platform
    {
        [Key]
        [Required]
        public int Id { get; set; }
        //from platforms service
        [Required]
        public int ExternalID { get; set; }
        [Required]        
        public string Name { get; set; }
        public ICollection<Command> Commands = new List<Command>();
    }
}