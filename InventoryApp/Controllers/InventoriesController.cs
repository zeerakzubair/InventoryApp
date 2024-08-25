using InventoryApp.Data;
using InventoryApp.Models;
using InventoryApp.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly InventoryAppDbContext inventoryAppDbContext;

        public InventoriesController(InventoryAppDbContext inventoryAppDbContext)
        {
            this.inventoryAppDbContext = inventoryAppDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var inventory = await inventoryAppDbContext.Inventories.ToListAsync();

            return View(inventory);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddInventoryViewModel viewModel)
        {
            if (await isNameDuplicate(viewModel.Name))
            {
                return View("Error", new ErrorViewModel() { ControllerName = "InventoriesController", ActionName = "Add", ErrorMessage = "Name of inventory item cannot be duplicate." });

            }
            else
            {
                var inventory = new Inventory()
                {
                    Id = new Guid(),
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    StockQuantity = viewModel.StockQuantity,
                    DateAdded = DateTime.UtcNow
                };

                await inventoryAppDbContext.Inventories.AddAsync(inventory);
                await inventoryAppDbContext.SaveChangesAsync();

                return RedirectToAction("Index");

            }
            

        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            var inventoryItem = await inventoryAppDbContext.Inventories.FindAsync(Id);
            if (inventoryItem != null)
            {
                inventoryAppDbContext.Inventories.Remove(inventoryItem);
                await inventoryAppDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var inventoryItem = await inventoryAppDbContext.Inventories.FindAsync(Id);
            if (inventoryItem != null)
            {
                var updateInventoryModel = new UpdateInventoryViewModel()
                {
                    Id = inventoryItem.Id,
                    Name = inventoryItem.Name,
                    Description = inventoryItem.Description,
                    Price = inventoryItem.Price,
                    StockQuantity = inventoryItem.StockQuantity,
                };


                return View(updateInventoryModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateInventoryViewModel updateModel)
        {
            var inventoryInDb = await inventoryAppDbContext.Inventories.FindAsync(updateModel.Id);

            if (inventoryInDb != null)
            {
                //here the duplicate name check should only be made when the new name is different from the previous name
                if ((updateModel.Name != inventoryInDb.Name) && (await isNameDuplicate(updateModel.Name)))
                {
                    return View("Error", new ErrorViewModel() {ControllerName= "InventoriesController", ActionName= "Edit", ErrorMessage = "Name of inventory item cannot be duplicate."});

                }
                else
                {
                    inventoryInDb.Name = updateModel.Name;
                    inventoryInDb.Description = updateModel.Description;
                    inventoryInDb.Price = updateModel.Price;
                    inventoryInDb.StockQuantity = updateModel.StockQuantity;

                    await inventoryAppDbContext.SaveChangesAsync();
                }
         
            }

            return RedirectToAction("Index");          

        }


        /// <summary>
        /// This wil be used to check if the inventory item name already exits in db
        /// </summary>
        /// <param name="name">name to be entered</param>
        /// <returns>tue or false</returns>
        private async Task<bool> isNameDuplicate(string name)
        {

            var inventory = await inventoryAppDbContext.Inventories.FirstOrDefaultAsync(x=>x.Name == name);

            if(inventory != null)
            {
                return true;
            }

            return false;
        }

    }
}
