using FPTBook.Areas.Identity.Data;
namespace FPTBook.Models;
public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }

    public string? FPTBOOKUserId { get; set; }
    public FPTBookUser? FPTBookUser { get; set; }

    public ICollection<OrderDetail>? OrderDetails { get; set; }

}

