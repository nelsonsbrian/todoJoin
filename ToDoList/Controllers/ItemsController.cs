using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System;

namespace ToDoList.Controllers
{
  public class ItemsController : Controller
  {
    [HttpGet("/items")]
    public ActionResult Index()
    {
      List <Item> allItems = Item.GetAll();
      return View(allItems);
    }
    [HttpGet("/items/new")]
    public ActionResult CreateForm(int categoryId)
    {
      List <Category> allCategories = Category.GetAll();
      return View(allCategories);
    }
    [HttpGet("/items/{itemId}")]
    public ActionResult Details(int itemId)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Item item = Item.Find(itemId);
      List <Category> categories = item.GetCategories();

      model.Add("item", item);
      model.Add("categories", categories);
      return View(model);
    }
    [HttpPost("/items")]
    public ActionResult CreateItem(int categoryId, string itemDescription)
    {
      Category foundCategory = Category.Find(categoryId);

      Item newItem;
      if(!Request.Form.ContainsKey("itemDueDate") || Request.Form["itemDueDate"] == "")
      {
        newItem = new Item(itemDescription);
        newItem.AddCategory(categoryId);
      }
      else
      {
        DateTime itemDueDate = DateTime.Parse(Request.Form["itemDueDate"]);
        newItem = new Item(itemDescription, itemDueDate);
        newItem.AddCategory(categoryId);

      }
      newItem.Save();
      return RedirectToAction("Index");
    }
    [HttpGet("/items/{itemId}/update")]
    public ActionResult UpdateForm(int itemId)
    {
      Item thisItem = Item.Find(itemId);
      return View(thisItem);
    }
    [HttpPost("/items/{itemId}/update")]
    public ActionResult Update(int itemId, string newDescription)
    {
      Item thisItem = Item.Find(itemId);
      thisItem.Edit(newDescription);
      return RedirectToAction("Index");
    }
    [HttpPost("/items/{itemId}/delete")]
    public ActionResult Delete(int itemId)
    {
      Item thisItem = Item.Find(itemId);
      Item.Delete(itemId);
      return RedirectToAction("Index");
    }
  }
}
