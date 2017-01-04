
namespace LilyApplication.Operations
{
    /// <summary>
    /// Defines custom operation codes
    /// </summary>
    public enum LilyOperationCodes : byte 
    {
        EchoOperation = 100,
        GameOperation = 101,
        UserLogin = 102,
        UserLogout = 103,
        UserRegister = 104
    }

    /// <summary>
    /// Defines custom paramter codes
    /// </summary>
    public enum LilyParameterCodes : byte
    {
        Text = 100,
        Response = 101,
    }
}
