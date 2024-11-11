using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Entity
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModificationDate { get; set; }

        public int? ModificationBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        public int? DeleteBy { get; set; }

        public bool IsDeleted { get; set; }

    }
}
