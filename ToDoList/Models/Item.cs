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
    private DateTime? _duedate;
    private bool _done;

    public Item(string Description, DateTime Duedate)
    {
      _id = 0;
      _description = Description;
      _duedate = Duedate;
      _done = false;
    }

    public Item(string Description, int Id, DateTime Duedate, bool Done)
    {
      _id = Id;
      _description = Description;
      _duedate = Duedate;
      _done = Done;
    }

    public Item(string Description)
    {
      _id = 0;
      _description = Description;
      _duedate = null;
      _done = false;
    }

    public Item(string Description, int Id, bool Done)
    {
      _id = Id;
      _description = Description;
      _duedate = null;
      _done = Done;
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
        bool doneEquality = (this._done == newItem.GetDone());
        return (descriptionEquality && doneEquality);
      }
    }

    public override int GetHashCode()
    {
     return this.GetDescription().GetHashCode();
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

    public bool GetDone()
    {
      return _done;
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
        bool done = rdr.GetBoolean(3);

        Item newItem;
        //check duedate has null value or not
        if(rdr.IsDBNull(2))
        {
          //if current row's duedate column's value is null
          newItem = new Item(itemDescription, itemId, done);
        }
        else
        {
          //if current row's duedate column's value is not null and has specific value
          DateTime itemDueDate = rdr.GetDateTime(2);
          newItem = new Item(itemDescription, itemId, itemDueDate, done);
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
      cmd.Parameters.AddWithValue("searchId", searchId);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      string itemDescription = "";
      bool done = false;
      if (rdr.Read())
      {
        itemDescription = rdr.GetString(1);
        done = rdr.GetBoolean(3);
      }
      Item newItem;
      if(rdr.IsDBNull(2))
      {
        //if current row's duedate column's value is null
        newItem = new Item(itemDescription, searchId, done);
      }
      else
      {
        //if current row's duedate column's value is not null and has specific value
        DateTime itemDueDate = rdr.GetDateTime(2);
        newItem = new Item(itemDescription, searchId, itemDueDate, done);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newItem;
    }

    public void AddCategory(int categoryId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO category_item (category_id, item_id) VALUES(@categoryId, @itemId);";

      MySqlParameter parameterCategoryId = new MySqlParameter();
      parameterCategoryId.ParameterName = "@categoryId";
      parameterCategoryId.Value = categoryId;
      cmd.Parameters.Add(parameterCategoryId);

      MySqlParameter parameterItemId = new MySqlParameter();
      parameterItemId.ParameterName = "@itemId";
      parameterItemId.Value = this._id;
      cmd.Parameters.Add(parameterItemId);

      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn == null)
      {
        conn.Dispose();
      }
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

      if (this._duedate == null)
      {
        cmd.CommandText = @"INSERT INTO items(description, done) VALUES (@description, @done);";
      }
      else
      {
        cmd.CommandText = @"INSERT INTO items(description, duedate, done) VALUES (@description, @duedate, @done);";
        cmd.Parameters.AddWithValue("@duedate", this._duedate);
      }
      cmd.Parameters.AddWithValue("@description", this._description);
      cmd.Parameters.AddWithValue("@done", this._done);

      cmd.ExecuteNonQuery();

      this._id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Edit(string newDescription)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE items SET description = @newDescription WHERE id = @searchId;";
      cmd.Parameters.AddWithValue("@searchId", this._id);
      cmd.Parameters.AddWithValue("@newDescription", newDescription);

      cmd.ExecuteNonQuery();
      this._description = newDescription;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Complete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

      cmd.CommandText = @"UPDATE items SET done = 1 WHERE id = @itemId;";
      cmd.Parameters.AddWithValue("@itemId", this._id);

      cmd.ExecuteNonQuery();
      this._done = true;
      
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Category> GetCategories()
    {
      List<Category> newCategories = new List<Category> {};

      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

      cmd.CommandText = @"SELECT categories.* FROM categories JOIN category_item ON (categories.id = category_item.category_id) JOIN items ON (category_item.item_id = items.id) WHERE items.id = @itemId;";

      MySqlParameter itemId = new MySqlParameter();
      itemId.ParameterName = "@itemId";
      itemId.Value = this._id;
      cmd.Parameters.Add(itemId);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

      while (rdr.Read())
      {
        int foundId = rdr.GetInt32(0);
        string foundName = rdr.GetString(1);

        Category foundCategory = new Category(foundName, foundId);
        newCategories.Add(foundCategory);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newCategories;
    }

    public static void Delete(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

      cmd.CommandText = @"DELETE FROM category_item WHERE item_id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);
      cmd.ExecuteNonQuery();

      cmd.CommandText = @"DELETE FROM items WHERE id = @searchId;";

      searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);
      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

      cmd.CommandText = @"DELETE FROM category_item;";
      cmd.ExecuteNonQuery();

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
