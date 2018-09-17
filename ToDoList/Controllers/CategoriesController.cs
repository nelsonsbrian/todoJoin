using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;

namespace ToDoList.Controllers
{
  public class CategoriesController : Controller
  {
    [HttpGet("/categories")]
    public ActionResult Index()
    {
        List<Category> allCategories = Category.GetAll();
        return View("Categories",allCategories);
    }

    [HttpGet("/categories/new")]
    public ActionResult CreateForm()
    {
        return View();
    }

    [HttpPost("/categories")]
    public ActionResult Create(string categoryName)
    {
        Category newCategory = new Category(categoryName);
        List<Category> allCategories = Category.GetAll();
        return View("Categories", allCategories);
    }

    [HttpGet("/categories/{id}")]
    public ActionResult Details(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Category selectedCategory = Category.Find(id);
      List<Item> categoryItems = selectedCategory.Items;
      model.Add("category",selectedCategory);
      model.Add("items",categoryItems);
      return View(model);
    }

    [HttpPost("/items")]
    public ActionResult CreateItem()
    {
      int categoryId = int.Parse(Request.Form["categoryId"]);
      string itemDescription = Request.Form["itemDescription"];
      Dictionary<string, object> model = new Dictionary<string, object>();
      Category foundCategory = Category.Find(categoryId);
      Item newItem = new Item(itemDescription);
      foundCategory.AddItem(newItem);
      List<Item> categoryItems = foundCategory.Items;
      model.Add("items", categoryItems);
      model.Add("category", foundCategory);
      return View("Details", model);
    }

  }
}
