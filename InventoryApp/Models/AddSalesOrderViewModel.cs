using InventoryApp.Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryApp.Models
{
    public class AddSalesOrderViewModel
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public List<SelectListItem> Items { get; set; } = new List<SelectListItem>();
    }
}
