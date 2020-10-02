using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerTest.Data.Entities
{
    [Table("Notes")]
    public class Note
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Description { get; set; }
    }
}