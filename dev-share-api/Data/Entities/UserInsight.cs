using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities;

[Index(nameof(ResourceId), IsUnique = false)]
public class UserInsight
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public long ResourceId { get; set; }
    public string Content { get; set; }
    [ForeignKey("ResourceId")]
    public Resource Resource { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}