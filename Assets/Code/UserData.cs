using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public string Name;
    public string Surname;
    public string Email;
    public string Password;

    public UserData(string name, string surname, string email, string password)
    {
        Name = name;
        Surname = surname;
        Email = email;
        Password = password;
    }
}

[Serializable]
public class UserListWrapper
{
    public List<UserData> users;

    public UserListWrapper(List<UserData> userList)
    {
        users = userList;
    }
}