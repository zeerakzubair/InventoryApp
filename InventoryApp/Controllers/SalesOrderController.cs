using InventoryApp.Data;
using InventoryApp.Models;
using InventoryApp.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Controllers
{
    public class SalesOrderController : Controller
    {
        private readonly InventoryAppDbContext inventoryAppDbContext;

        public SalesOrderController(InventoryAppDbContext inventoryAppDbContext)
        {
            this.inventoryAppDbContext = inventoryAppDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var salesOrders = await inventoryAppDbContext.SalesOrders.ToListAsync();
            var salesOrderViewModelList =  new List<SalesOrderViewModel>();
            
            foreach (var salesOrder in salesOrders)
            {
                var inventoryItem = await inventoryAppDbContext.Inventories.FindAsync(salesOrder.ItemId);

                if(inventoryItem != null)
                {
                    var salesOrderViewModel = new SalesOrderViewModel()
                    {
                        SalesOrderId = salesOrder.Id,
                        InventoryItemId = salesOrder.ItemId,
                        InventoryItemName = inventoryItem.Name,
                        InventoryItemDescription = inventoryItem.Description,
                        InventoryItemPrice = inventoryItem.Price,
                        Quantity = salesOrder.Quantity,
                        TotalPrice = inventoryItem.Price * salesOrder.Quantity,
                        CreatedDate = salesOrder.CreatedDate,
                    };

                    salesOrderViewModelList.Add(salesOrderViewModel);

                }              

            }           


            return View(salesOrderViewModelList);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var inventoryItems = await inventoryAppDbContext.Inventories.ToListAsync();
            var AddSalesOrderViewModel = new AddSalesOrderViewModel()
            {
                 Items = inventoryItems.Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                }).ToList()
            };

            return View(AddSalesOrderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddSalesOrderViewModel salesOrderViewModel)
        {
            var InventoryItem = await inventoryAppDbContext.Inventories.FindAsync(salesOrderViewModel.ItemId);
            if(InventoryItem.StockQuantity >= salesOrderViewModel.Quantity)
            {
                var saleOrder = new SalesOrder()
                {
                    Id = new Guid(),
                    ItemId = salesOrderViewModel.ItemId,
                    Quantity = salesOrderViewModel.Quantity,
                    CreatedDate = DateTime.UtcNow

                };

                inventoryAppDbContext.SalesOrders.Add(saleOrder);

                InventoryItem.StockQuantity = InventoryItem.StockQuantity - salesOrderViewModel.Quantity;

                await inventoryAppDbContext.SaveChangesAsync();

            }
            else
            {
                return View("Error", new ErrorViewModel() { ControllerName = "SalesOrderController", ActionName = "Add", ErrorMessage = "Stock is low for this item,first update stock." });
            }
            

            return RedirectToAction("Index");
        }
    }
}
