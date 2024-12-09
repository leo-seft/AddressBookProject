using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;


namespace TaskManagement{
    class Program{
        static void Main() {
        string connectionString = "Data Source=AddressBook.db";

        using (var connection = new SqliteConnection(connectionString)) {
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Contacts (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    PhoneNumber TEXT NOT NULL UNIQUE,
                    Email TEXT UNIQUE,
                    Address TEXT
                );";

            using (var command = new SqliteCommand(createTableQuery, connection)) {
                command.ExecuteNonQuery();
                Console.WriteLine("Database and table created successfully.");
            }
        }
    }
    public class Contact {
    public int ID { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }  
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
}

    }
}