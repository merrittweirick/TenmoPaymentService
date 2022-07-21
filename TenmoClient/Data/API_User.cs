namespace TenmoClient.Data
{
    /// <summary>
    /// Return value from login endpoint
    /// </summary>
    public class API_User  // Same as the ReturnUser on the Server side except for message property ~~ client side version
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }        
        public decimal Balance { get; set; }
    }
}
