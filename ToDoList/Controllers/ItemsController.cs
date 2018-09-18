using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System;

namespace ToDoList.Controllers
{
  public class ItemsController : Controller
  {
    [HttpGet("/categories/{categoryId}/items")]
    public ActionResult Index(int categoryId)
    {
      Category category = Category.Find(categoryId);
      List<Item> categoryItems = category.GetItems();
      return View(categoryItems);
    }
    [HttpGet("/categories/{categoryId}/items/new")]
    public ActionResult CreateForm(int categoryId)
    {
      // Dictionary<string,object> model = new Dictionary<string,object>();
      Category category = Category.Find(categoryId);
      return View(category);
    }
    [HttpGet("/categories/{categoryId}/items/{itemId}")]
    public ActionResult Details(int categoryId, int itemId)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Category category = Category.Find(categoryId);
      Item item = Item.Find(itemId);
      model.Add("item", item);
      model.Add("category", category);
      return View(model);
    }
    [HttpPost("/items")]
    public ActionResult CreateItem()
    {
      int categoryId = int.Parse(Request.Form["categoryId"]);
      string itemDescription = Request.Form["itemDescription"];
      Dictionary<string, object> model = new Dictionary<string, object>();
      Category foundCategory = Category.Find(categoryId);

      Item newItem;
      if(!Request.Form.ContainsKey("itemDueDate") || Request.Form["itemDueDate"] == "")
      {
        newItem = new Item(itemDescription, categoryId);
      }
      else
      {
        DateTime itemDueDate = DateTime.Parse(Request.Form["itemDueDate"]);
        newItem = new Item(itemDescription, categoryId, itemDueDate);
      }
      newItem.Save();
      // model.Add("item", newItem);
      // model.Add("category", foundCategory);
      return RedirectToAction("Index", new {categoryId = categoryId});
    }
  }
}
