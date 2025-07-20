using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities;

[Index(nameof(ResourceId), IsUnique = true)]
[Index(nameof(NormalizeUrl), IsUnique = true)]
public class Resource
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public long ResourceId { get; set; }
    public string Url { get; set; }
    public string NormalizeUrl { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; }
    public List<UserInsight> UserInsights { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}