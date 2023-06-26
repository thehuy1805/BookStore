namespace FPTBook.Models;
using System.ComponentModel.DataAnnotations;
public class Book
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public double Price { get; set; }

    public string? Description { get; set; }

    [DataType(DataType.ImageUrl)]
    public string? UploadImage { get; set; }
 
    public int AuthorID { get; set; }
    public Author? Author { get; set; }
 
    public int CategoryID { get; set; }
    public Category? Category { get; set; }
  
    public int PublisherID { get; set; }

    public Publisher? Publisher { get; set; }

    public ICollection<OrderDetail>? OrderDetails { get; set; }
}