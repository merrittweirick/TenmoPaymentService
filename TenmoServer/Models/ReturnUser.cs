namespace TenmoServer.Models
{
    /// <summary>
    /// Model to return upon successful login
    /// </summary>
    public class ReturnUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        //public string Role { get; set; }    Don't need to worry about role based authentication
        public string Token { get; set; }
    }
}
