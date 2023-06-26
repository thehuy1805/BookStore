using FPTBook.Areas.Identity.Data;
namespace FPTBook.Models;
public class OrderDetail{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int OrderID { get; set; }
    public Order? Order { get; set; }
    public int BookID { get; set; }
    public Book? Book { get; set; }
}

