namespace InventoryApp.Models
{
    public class SalesOrderViewModel
    {

        public Guid SalesOrderId { get; set; }
        public Guid InventoryItemId { get; set; }
        public string InventoryItemName { get; set; }
        public string InventoryItemDescription { get; set; }
        public double InventoryItemPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
