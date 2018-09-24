using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDoList.Models;
using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;

namespace ToDoList.Tests
{
  [TestClass]
  public class CategoryTests : IDisposable
  {
    public void Dispose()
    {
      Category.DeleteAll();
      Item.DeleteAll();
    }
    public CategoryTests()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=to_do_list_test;Convert Zero Datetime=True";
    }
    [TestMethod]
    public void GetAll_DbStartsEmpty_0()
    {
      int result = Category.GetAll().Count;

      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetAll_ReturnAllRows_2()
    {
      //Arrange
      Category firstCategory = new Category("desc1");
      firstCategory.Save();
      Category secondCategory = new Category("desc2");
      secondCategory.Save();
      //Acct
      int categoryCount = Category.GetAll().Count;
      //Assert
      Assert.AreEqual(2, categoryCount);
    }

    [TestMethod]
    public void Edit_UpdatesCategorInDatabase_String()
    {
      //Arrange
      Category newCategory = new Category("input");
      newCategory.Save();
      string newName = "new input";
      //Act
      newCategory.Edit(newName);
      //Assert
      Assert.AreEqual(newName, newCategory.Name);
    }

    [TestMethod]
    public void GetItems_List()
    {
      //Arrange
      Item testItem1 = new Item ("Walk the dog");
      testItem1.Save();
      Item testItem2 = new Item ("Walk the cat");
      testItem2.Save();
      Category testCategory = new Category("pets");
      testCategory.Save();
      testCategory.AddItem(testItem1.GetId());
      testCategory.AddItem(testItem2.GetId());
      List<Item> expectedItems = new List<Item>{testItem1, testItem2};
      //Act
      List<Item> items = testCategory.GetItems();
      //Assert
      CollectionAssert.AreEqual(expectedItems, items);
    }

    [TestMethod]
    public void Delete_DeleteIndividualCategory()
    {
      //Arrange
      Category testCategory = new Category("pets");
      testCategory.Save();
      Item testItem = new Item ("Walk the dog");
      testItem.Save();
      testCategory.AddItem(testItem.GetId());
      List <Category> expectedCategories = new List <Category>{};

      //Act
      Category.Delete(testCategory.Id);
      List <Category> categories = Category.GetAll();

      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT COUNT(*) FROM category_item;";

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      int category_item_count = 0;
      while (rdr.Read())
      {
        category_item_count = rdr.GetInt32(0);
      }

      //Assert
      Assert.AreEqual(0, category_item_count);
      CollectionAssert.AreEqual(expectedCategories, categories);
    }
  }
}
