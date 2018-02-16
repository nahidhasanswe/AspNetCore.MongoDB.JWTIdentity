using AspNetCore.MongoDB;
using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    public class SampleModel : IMongoEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ContactNo { get; set; }
    }
}
