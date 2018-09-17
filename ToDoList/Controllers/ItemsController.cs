using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;

namespace ToDoList.Controllers
{
  public class ItemsController : Controller
  {
    [HttpGet("/categories/{categoryId}/items/new")]
    public ActionResult CreateForm(int categoryId)
    {
      Dictionary<string,object> model = new Dictionary<string,object>();
      Category category = Category.Find(categoryId);
      return View(category);
    }
    [HttpGet("/categories/{categoryId}/items/{itemId}")]
     public ActionResult Details(int categoryId, int itemId)
     {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Category category = Category.Find(categoryId);
        Item item = category.FindItem(itemId);
        model.Add("item", item);
        model.Add("category", category);
        return View(model);
     }
  }
}
