using System.ComponentModel.DataAnnotations.Schema;
namespace Entities;

public class Resource
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Url { get; set; }
    public string Content { get; set; }
}

