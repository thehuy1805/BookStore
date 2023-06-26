namespace FPTBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
public class Category {
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }
    
    public ICollection<Book>? Books { get; set; }
}