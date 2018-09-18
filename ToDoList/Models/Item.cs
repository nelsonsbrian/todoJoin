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

    public Item(string Description, int Id = 0)
    {
      _id = Id;
      _description = Description;
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
        Item newItem = new Item(itemDescription, itemId);
        allItems.Add(newItem);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allItems;
    }

    public void Save()
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
