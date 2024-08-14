namespace Data.Helpers;

public class WrongCredentialsException : Exception
{
    public WrongCredentialsException() : base("Wrong Credentials")
    {
    }
}