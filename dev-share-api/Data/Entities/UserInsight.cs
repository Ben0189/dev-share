using System.ComponentModel.DataAnnotations.Schema;
namespace Entities;

public class UserInsight
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Url { get; set; }
    public string Insight { get; set; }
}

