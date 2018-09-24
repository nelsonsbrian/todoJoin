using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDoList.Models;
using System.Collections.Generic;
using System;

namespace ToDoList.Tests
{
  [TestClass]
  public class ItemTests : IDisposable
  {
    public void Dispose()
    {
      Item.DeleteAll();
      Category.DeleteAll();
    }
    public ItemTests()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=to_do_list_test;Convert Zero Datetime=True";
    }
    [TestMethod]
    public void GetAll_DbStartsEmpty_0()
    {
      //Arrange
      //Act
      int result = Item.GetAll().Count;

      //Assert
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Equals_ReturnsTrueIfDescriptionsAreTheSame_Item()
    {
      // Arrange, Act
      Item firstItem = new Item("Mow the lawn");
      Item secondItem = new Item("Mow the lawn");

      // Assert
      Assert.AreEqual(firstItem, secondItem);
    }

    [TestMethod]
    public void Save_SavesToDatabase_ItemList()
    {
      //Arrange
      Item testItem = new Item("Mow the lawn");

      //Act
      testItem.Save();
      List<Item> result = Item.GetAll();
      List<Item> testList = new List<Item>{testItem};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void Edit_UpdatesItemInDatabase_String()
    {
      //Arrange
      string firstDescription = "Walk the dog";
      Item testItem = new Item (firstDescription);
      testItem.Save();

      string secondDescription = "Mow the lawn";

      //Act
      testItem.Edit(secondDescription);
      string result = Item.Find(testItem.GetId()).GetDescription();

      //Assert
      Assert.AreEqual(secondDescription,result);
    }

    [TestMethod]
    public void Delete_DeleteItemInDatabase()
    {
      //Arrange
      Item testItem = new Item ("Walk the dog");
      testItem.Save();
      int testItemId = testItem.GetId();

      //Act
      Item.Delete(testItemId);
      int count = Item.GetAll().Count;

      //Assert
      Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void GetCategories_List()
    {
      //Arrange
      Item testItem = new Item ("Walk the dog");
      testItem.Save();
      Category testCategory1 = new Category("category1");
      testCategory1.Save();
      Category testCategory2 = new Category("category2");
      testCategory2.Save();
      testItem.AddCategory(testCategory1.Id);
      testItem.AddCategory(testCategory2.Id);
      List <Category> expectedCategories = new List<Category>{testCategory1, testCategory2};

      //Act
      List <Category> categories = testItem.GetCategories();

      //Assert
      CollectionAssert.AreEqual(expectedCategories, categories);
    }

    [TestMethod]
    public void Complete_SetDone()
    {
      //Arrange
      Item testItem = new Item ("Walk the dog");
      testItem.Save();
      Category testCategory1 = new Category("category1");
      testCategory1.Save();
      testItem.AddCategory(testCategory1.Id);

      //Act
      testItem.Complete();

      //Assert
      Assert.AreEqual(true, testItem.GetDone());
    }
  }
}
