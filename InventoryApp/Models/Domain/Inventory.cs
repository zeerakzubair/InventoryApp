namespace InventoryApp.Models.Domain
{
    public class Inventory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime DateAdded { get; set; }
        public int StockQuantity {  get; set; }
    }
}
