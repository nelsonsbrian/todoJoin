using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ToDoList;
using System;

namespace ToDoList.Models
{
  public class Category
  {
    public string Name { get; set; }
    public int Id { get; set; }

    public Category(string categoryName, int id = 0)
    {
      Name = categoryName;
      Id = id;
    }

    public override bool Equals(System.Object otherCategory)
    {
      if (!(otherCategory is Category))
      {
        return false;
      }
      else
      {
        Category newCategory = (Category) otherCategory;
        bool descriptionEquality = (this.Name == newCategory.Name);
        return (descriptionEquality);
      }
    }

    public override int GetHashCode()
    {
         return this.Name.GetHashCode();
    }

    public static List<Category> GetAll()
    {
      List<Category> allCategories = new List<Category> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM categories;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        string categoryName = rdr.GetString(1);
        int categoryId = rdr.GetInt32(0);

        Category newCategory = new Category(categoryName, categoryId);
        allCategories.Add(newCategory);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCategories;
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;

      cmd.CommandText = @"DELETE FROM category_item;";
      cmd.ExecuteNonQuery();

      cmd.CommandText = @"DELETE FROM categories;";
      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static void Delete(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

      cmd.CommandText = @"DELETE FROM category_item WHERE category_id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);
      cmd.ExecuteNonQuery();

      cmd.CommandText = @"DELETE FROM categories WHERE id = @searchId;";

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

    public static Category Find(int searchId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM categories WHERE id = @searchId;";

      MySqlParameter parameterId = new MySqlParameter();
      parameterId.ParameterName = "@searchId";
      parameterId.Value = searchId;
      cmd.Parameters.Add(parameterId);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      rdr.Read();

      string categoryName = rdr.GetString(1);
      int categoryId = rdr.GetInt32(0);
      Category newCategory = new Category(categoryName, categoryId);

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

      return newCategory;
    }

    public void AddItem(int itemId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO category_item (category_id, item_id) VALUES(@categoryId, @itemId);";

      MySqlParameter parameterCategoryId = new MySqlParameter();
      parameterCategoryId.ParameterName = "@categoryId";
      parameterCategoryId.Value = this.Id;
      cmd.Parameters.Add(parameterCategoryId);

      MySqlParameter parameterItemId = new MySqlParameter();
      parameterItemId.ParameterName = "@itemId";
      parameterItemId.Value = itemId;
      cmd.Parameters.Add(parameterItemId);

      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Edit(string newName)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE categories SET name = @newName WHERE id=@categoryId;";

      MySqlParameter parameterNewName = new MySqlParameter() as MySqlParameter;
      parameterNewName.ParameterName = "@newName";
      parameterNewName.Value = newName;
      cmd.Parameters.Add(parameterNewName);

      MySqlParameter categoryId = new MySqlParameter() as MySqlParameter;
      categoryId.ParameterName = "@categoryId";
      categoryId.Value = this.Id;
      cmd.Parameters.Add(categoryId);

      cmd.ExecuteNonQuery();
      this.Name = newName;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO categories(name) VALUES (@name);";

      MySqlParameter parameterName = new MySqlParameter();
      parameterName.ParameterName = "@name";
      parameterName.Value = this.Name;
      cmd.Parameters.Add(parameterName);

      cmd.ExecuteNonQuery();
      this.Id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Item> GetItems()
    {
      List<Item> newItems = new List<Item> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT items.* FROM categories JOIN category_item ON (categories.id = category_item.category_id) JOIN items ON (category_item.item_id = items.id) WHERE categories.id=@categoryId ORDER BY duedate;";
      MySqlParameter parameterId = new MySqlParameter();
      parameterId.ParameterName = "@categoryId";
      parameterId.Value = this.Id;
      cmd.Parameters.Add(parameterId);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        string itemDescription = rdr.GetString(1);
        bool itemDone = rdr.GetBoolean(3);

        Item newItem;
        if(rdr.IsDBNull(2))
        {
          //if current row's duedate column's value is null
          newItem = new Item(itemDescription, itemId, itemDone);
        }
        else
        {
          //if current row's duedate column's value is not null and has specific value
          DateTime itemDueDate = rdr.GetDateTime(2);
          newItem = new Item(itemDescription, itemId, itemDueDate, itemDone);
        }

        newItems.Add(newItem);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newItems;
    }
  }
}
