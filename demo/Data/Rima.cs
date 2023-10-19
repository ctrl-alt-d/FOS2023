using System.ComponentModel.DataAnnotations;

namespace demo.Data;

public class Rima
{
    [Required]
    public string Text { get; set; } = default!;
    public string Author { get; set; } = default!;
    public float Rate { get; set; }
    public string Valoracio { get; set; } = "";
}