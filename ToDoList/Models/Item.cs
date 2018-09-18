using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ToDoList;
using System;


namespace ToDoList.Models
{
  public class Item
  {
    private int _id;
    private string _description;
    private int _categoryId;

    public Item(string Description, int CategoryId)
    {
      _id = 0;
      _description = Description;
      _categoryId = CategoryId;
    }

    public Item(string Description, int Id, int CategoryId)
    {
      _id = Id;
      _description = Description;
      _categoryId = CategoryId;
    }

    public override bool Equals(System.Object otherItem)
    {
      if (!(otherItem is Item))
      {
        return false;
      }
      else
      {
        Item newItem = (Item) otherItem;
        bool descriptionEquality = (this.GetDescription() == newItem.GetDescription());
        return (descriptionEquality);
      }
    }

    public string GetDescription() //method
    {
      return _description;
    }

    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }

    public int GetId()
    {
      return _id;
    }

    public static List<Item> GetAll()
    {
      List<Item> allItems = new List<Item> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM items;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        string itemDescription = rdr.GetString(1);
        int itemCategoryId = rdr.GetInt32(2);
        Item newItem = new Item(itemDescription, itemId, itemCategoryId);
        allItems.Add(newItem);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allItems;
    }

    public static Item Find(int searchId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM items WHERE id = @searchId;";

      MySqlParameter parameterId = new MySqlParameter();
      parameterId.ParameterName = "@searchId";
      parameterId.Value = searchId;
      cmd.Parameters.Add(parameterId);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      rdr.Read();

      string itemDescription = rdr.GetString(1);
      int itemCategoryId = rdr.GetInt32(2);
      Item newItem = new Item(itemDescription, searchId, itemCategoryId);

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

      return newItem;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO items(description, category) VALUES (@description, @category);";
      MySqlParameter parameterDescription = new MySqlParameter();
      parameterDescription.ParameterName = "@description";
      parameterDescription.Value = this._description;
      cmd.Parameters.Add(parameterDescription);
      MySqlParameter parameterCategory = new MySqlParameter();
      parameterCategory.ParameterName = "@category";
      parameterCategory.Value = this._categoryId;
      cmd.Parameters.Add(parameterCategory);

      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Delete()
    {

    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM items;";

      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
