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
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (await isNameDuplicate(viewModel.Name))
            {
                ModelState.AddModelError("Name", "Name of inventory item cannot be duplicate.");
                return View(viewModel);
            }

            try
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

                TempData["SuccessMessage"] = "Inventory item added succefully.";

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occured; {ex.Message} ";
            }


            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                var inventoryItem = await inventoryAppDbContext.Inventories.FindAsync(Id);
                if (inventoryItem != null)
                {

                    inventoryAppDbContext.Inventories.Remove(inventoryItem);
                    await inventoryAppDbContext.SaveChangesAsync();

                    TempData["DeleteMessage"] = "Inventory item deleted successfully.";

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occured; {ex.Message} ";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            try
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
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occured; {ex.Message} ";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateInventoryViewModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(updateModel);
            }

            try
            {
                var inventoryInDb = await inventoryAppDbContext.Inventories.FindAsync(updateModel.Id);

                if (inventoryInDb != null)
                {
                    //here the duplicate name check should only be made when the new name is different from the previous name
                    if ((updateModel.Name != inventoryInDb.Name) && (await isNameDuplicate(updateModel.Name)))
                    {
                        ModelState.AddModelError("Name", "Name of inventory item cannot be duplicate.");
                        return View(updateModel);
                    }

                    inventoryInDb.Name = updateModel.Name;
                    inventoryInDb.Description = updateModel.Description;
                    inventoryInDb.Price = updateModel.Price;
                    inventoryInDb.StockQuantity = updateModel.StockQuantity;

                    await inventoryAppDbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Inventory item updated successfully.";

                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occured; {ex.Message} ";
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

            var inventory = await inventoryAppDbContext.Inventories.FirstOrDefaultAsync(x => x.Name == name);

            if (inventory != null)
            {
                return true;
            }

            return false;
        }

    }
}
