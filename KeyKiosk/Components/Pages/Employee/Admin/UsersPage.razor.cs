using KeyKiosk.Data;
using KeyKiosk.Services;
using Microsoft.AspNetCore.Components;

namespace KeyKiosk.Components.Pages.Employee.Admin;

public partial class UsersPage
{
    [Inject]
    private UserService UserService { get; set; }

    /// <summary>
    /// Displays list of existing users
    /// </summary>
    private List<User> UserList { get; set; } = new List<User>();

    /// <summary>
    /// Model for adding user form
    /// </summary>
    private User UserToAdd { get; set; } = new User();

    /// <summary>
    /// Model for updating user form
    /// </summary>
    private User UserToUpdate { get; set; } = new User();

    /// <summary>
    /// Loads existing users to display on page
    /// </summary>
    /// <returns></returns>
    protected override Task OnInitializedAsync()
    {
        RefreshUsersList();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Refreshes displayed users after changes are made
    /// </summary>
    private void RefreshUsersList()
    {
        var users = UserService.GetAllUsers();
        PopulateUserList(users);
    }

    /// <summary>
    /// Populates UserList with data from database
    /// </summary>
    /// <param name="users"></param>
    private void PopulateUserList(List<User> users)
    {
        UserList.Clear();

        foreach (User u in users)
        {
            UserList.Add(u);
        }
    }

    /// <summary>
    /// Method to add new user
    /// </summary>
    public void AddNewUser()
    {
        UserService.AddUser(UserToAdd);
        RefreshUsersList();
        UserToAdd = new User();
    }

    /// <summary>
    /// Method to delete existing user using id
    /// </summary>
    /// <param name="id"></param>
    public void DeleteUser(int id)
    {
        UserService.DeleteUser(id);
        RefreshUsersList();
    }

    /// <summary>
    /// Method to update existing user
    /// </summary>
    private void UpdateExistingUser()
    {
        UserService.UpdateUser(UserToUpdate);
        RefreshUsersList();
        UserToUpdate = new User();
    }
}
