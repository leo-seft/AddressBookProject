using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace TaskManagement {
    class Program {
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

            var addressBook = new AddressBook();

while (true) {
    Console.WriteLine("Address Book Menu:");
    Console.WriteLine("1. Add Contact");
    Console.WriteLine("2. View Contacts");
    Console.WriteLine("3. Update Contact");
    Console.WriteLine("4. Delete Contact");
    Console.WriteLine("5. Exit");
    Console.Write("Enter your choice: ");

    if (!int.TryParse(Console.ReadLine(), out int choice)) {
        Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
        continue;
    }

    switch (choice) {
        case 1:
            var newContact = new Contact {
                FirstName = Prompt("Enter first name: "),
                LastName = Prompt("Enter last name: "),
                PhoneNumber = Prompt("Enter phone number: "),
                Email = Prompt("Enter email (optional): "),
                Address = Prompt("Enter address (optional): ")
            };
            Console.WriteLine(newContact.LastName);
            addressBook.AddContact(newContact);
            Console.WriteLine("Contact added successfully.");
            break;
            //views contacts
        case 2:
            var contacts = addressBook.GetContacts();
            Console.WriteLine("Contacts:");
            foreach (var contact in contacts) {
                Console.WriteLine($"ID: {contact.ID}, Name: {contact.FirstName} {contact.LastName}, Phone: {contact.PhoneNumber}, Email: {contact.Email}, Address: {contact.Address}");
            }
            break;
            // updates contact
        case 3:
            Console.Write("Enter the ID of the contact to update: ");
            if (int.TryParse(Console.ReadLine(), out int updateId)) {
                var updatedContact = new Contact {
                    ID = updateId,
                    FirstName = Prompt("Enter new first name: "),
                    LastName = Prompt("Enter new last name: "),
                    PhoneNumber = Prompt("Enter new phone number: "),
                    Email = Prompt("Enter new email (optional): "),
                    Address = Prompt("Enter new address (optional): ")
                };
                addressBook.UpdateContact(updatedContact);
                Console.WriteLine("Contact updated successfully.");
            } else {
                Console.WriteLine("Invalid ID.");
            }
            break;
            //deletes contact
        case 4:
            // Console.Write("Enter the ID of the contact to delete: ");
            // int.TryParse(Console.ReadLine(), out int deleteId);
            // getContactById(deleteId);
            if (int.TryParse(Console.ReadLine(), out int deleteId)) {
                addressBook.DeleteContact(deleteId);
                Console.WriteLine("Contact deleted successfully.");
            } else {
                Console.WriteLine("Invalid ID.");
            }
            break;
            //close application
        case 5:
            Console.WriteLine("Exiting Address Book. Goodbye!");
            return;
        case 6: 
            Console.Write("Enter the ID of the contact to delete: ");
            int.TryParse(Console.ReadLine(), out int findId);
            Contact contact = getContactById(findId);
            if (contact != null) {
                Console.WriteLine("Conact found");
            } else{
                Console.WriteLine("Contact not found");
            }
            break;
        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }
}
        }
static string Prompt(string message) {
    Console.Write(message);
    return Console.ReadLine()?.Trim();
        }
    }
        //contact class
    public class Contact {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
        //All CRUD commands
    public class AddressBook {
        
        private const string DatabasePath = "AddressBook.db";

        public void CheckDatabase() {
                // checks that database is found
            if (!File.Exists(DatabasePath)) {
                Console.WriteLine("Error: Database not found. Please ensure the database exists.");
                Environment.Exit(1);
            }
            try {   //checks database connects successfully
                using (var connection = new SqliteConnection($"Data Source={DatabasePath}")) {
                    connection.Open();
                    Console.WriteLine("Database connection successful.");
                }
            } catch (Exception ex) {
                Console.WriteLine("An error occurred while connecting to the database: " + ex.Message);
                Environment.Exit(1);
            }
        }

    
            // Adding a new contact
        public void AddContact(Contact contact) {
            //validation
            if (string.IsNullOrWhiteSpace(contact.FirstName)) {
            Console.WriteLine("Error: First name cannot be null or empty.");
            return;
            }
            if (string.IsNullOrWhiteSpace(contact.LastName)) {
            Console.WriteLine("Error: Last name cannot be null or empty.");
            return;
            }
            if (string.IsNullOrWhiteSpace(contact.PhoneNumber)) {
            Console.WriteLine("Error: Phone number cannot be null or empty.");
            return;
            }
            using (var connection = new SqliteConnection("Data Source=AddressBook.db")) {
                connection.Open();
                var query = "INSERT INTO Contacts (FirstName, LastName, PhoneNumber, Email, Address) VALUES (@FirstName, @LastName, @PhoneNumber, @Email, @Address)";
                using (var command = new SqliteCommand(query, connection)) {
                    command.Parameters.AddWithValue("@FirstName", contact.FirstName);
                    command.Parameters.AddWithValue("@LastName", contact.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", contact.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", contact.Address ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Reads the contacts from the database
        public List<Contact> GetContacts() {
            var contacts = new List<Contact>();
            using (var connection = new SqliteConnection("Data Source=AddressBook.db")) {
                connection.Open();
                var query = "SELECT * FROM Contacts";
                using (var command = new SqliteCommand(query, connection)) {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            contacts.Add(new Contact {
                                ID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                PhoneNumber = reader.GetString(3),
                                Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Address = reader.IsDBNull(5) ? null : reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return contacts;
        }

        // Updates a contact via ID
        public void UpdateContact(Contact contact) {
            if (string.IsNullOrWhiteSpace(contact.FirstName)) {
            Console.WriteLine("Error: First name cannot be null or empty.");
            return;
            }
            if (string.IsNullOrWhiteSpace(contact.LastName)) {
            Console.WriteLine("Error: Last name cannot be null or empty.");
            return;
            }
            if (string.IsNullOrWhiteSpace(contact.PhoneNumber)) {
            Console.WriteLine("Error: Phone number cannot be null or empty.");
            return;
            }
            using (var connection = new SqliteConnection("Data Source=AddressBook.db")) {
                connection.Open();
                var query = "UPDATE Contacts SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber, Email = @Email, Address = @Address WHERE ID = @ID";
                using (var command = new SqliteCommand(query, connection)) {
                    command.Parameters.AddWithValue("@ID", contact.ID);
                    command.Parameters.AddWithValue("@FirstName", contact.FirstName);
                    command.Parameters.AddWithValue("@LastName", contact.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", contact.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Address", contact.Address ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Deletes a contact by ID
        public void DeleteContact(int id) {
            using (var connection = new SqliteConnection("Data Source=AddressBook.db")) {
                connection.Open();
                var query = "DELETE FROM Contacts WHERE ID = @ID";
                using (var command = new SqliteCommand(query, connection)) {
                    command.Parameters.AddWithValue("@ID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Contact getContactById(int id) {
            using (var connection = new SqliteConnection("Data Source=AddressBook.db")){
                connection.Open();
                var query = "SELECT * FROM Contacts WHERE ID = @ID";
                using (var command = new SqliteCommand(query, connection)) {
                    command.Parameters.AddWithValue("@ID",id);
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            Contact contact = new Contact{
                                ID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                PhoneNumber = reader.GetString(3),
                                Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                            };
                            return contact;
                        }
                        else
                        {
                            return null;
                        };
                    }
                }
            }
        }
    }
}
