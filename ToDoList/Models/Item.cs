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
    private DateTime? _duedate;

    public Item(string Description, int CategoryId, DateTime Duedate)
    {
      _id = 0;
      _description = Description;
      _categoryId = CategoryId;
      _duedate = Duedate;
    }

    public Item(string Description, int Id, int CategoryId, DateTime Duedate)
    {
      _id = Id;
      _description = Description;
      _categoryId = CategoryId;
      _duedate = Duedate;
    }

    public Item(string Description, int CategoryId)
    {
      _id = 0;
      _description = Description;
      _categoryId = CategoryId;
      _duedate = null;
    }

    public Item(string Description, int Id, int CategoryId)
    {
      _id = Id;
      _description = Description;
      _categoryId = CategoryId;
      _duedate = null;
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

    public DateTime? GetDueDate()
    {
      return _duedate;
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
      cmd.CommandText = @"SELECT * FROM items ORDER BY duedate;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        string itemDescription = rdr.GetString(1);
        int itemCategoryId = rdr.GetInt32(2);

        Item newItem;
        //check duedate has null value or not
        if(rdr.IsDBNull(3))
        {
          //if current row's duedate column's value is null
          newItem = new Item(itemDescription, itemId, itemCategoryId);
        }
        else
        {
          //if current row's duedate column's value is not null and has specific value
          DateTime itemDueDate = rdr.GetDateTime(3);
          newItem = new Item(itemDescription, itemId, itemCategoryId, itemDueDate);
        }
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

      Item newItem;

      if(rdr.IsDBNull(3))
      {
        //if current row's duedate column's value is null
        newItem = new Item(itemDescription, searchId, itemCategoryId);
      }
      else
      {
        //if current row's duedate column's value is not null and has specific value
        DateTime itemDueDate = rdr.GetDateTime(3);
        newItem = new Item(itemDescription, searchId, itemCategoryId, itemDueDate);
      }

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

      if (this._duedate == null)
      {
        cmd.CommandText = @"INSERT INTO items(description, category) VALUES (@description, @category);";
        MySqlParameter parameterDescription = new MySqlParameter();
        parameterDescription.ParameterName = "@description";
        parameterDescription.Value = this._description;
        cmd.Parameters.Add(parameterDescription);
        MySqlParameter parameterCategory = new MySqlParameter();
        parameterCategory.ParameterName = "@category";
        parameterCategory.Value = this._categoryId;
        cmd.Parameters.Add(parameterCategory);
      }
      else
      {
        cmd.CommandText = @"INSERT INTO items(description, category, duedate) VALUES (@description, @category, @duedate);";

        MySqlParameter parameterDescription = new MySqlParameter();
        parameterDescription.ParameterName = "@description";
        parameterDescription.Value = this._description;
        cmd.Parameters.Add(parameterDescription);

        MySqlParameter parameterCategory = new MySqlParameter();
        parameterCategory.ParameterName = "@category";
        parameterCategory.Value = this._categoryId;
        cmd.Parameters.Add(parameterCategory);

        MySqlParameter parameterDueDate = new MySqlParameter();
        parameterDueDate.ParameterName = "@duedate";
        parameterDueDate.Value = this._duedate;
        cmd.Parameters.Add(parameterDueDate);
      }

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
