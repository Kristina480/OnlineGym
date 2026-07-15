namespace OnlineGym.Application.Domain;

public class UserAccount
{
    public long AccountId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string UserType { get; set; }

    public UserAccount(long accountId, string username, string password, string userType)
    {
        AccountId = accountId;
        Username = username;
        Password = password;
        UserType = userType;
    }
}