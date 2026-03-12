namespace ToDoApplication.Models;

public class User
{
       private string _username;
       private string _email;
       private string _password;

       public User()
       {
              
       }

       public string username
       {
              get { return this._username;  }
              set{ this._username = value; }
       }
       
       public string email
       {
              get { return this._email; }
              set { this._email = value; }
       }
       
       public string password
       {
              get { return this._password; }
              set { this._password = value; }
       }
}