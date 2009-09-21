namespace ElmsConnector
{
    public interface IAuthenticatonService
    {
        bool AuthenticateUser(string username, string password);
    }
}