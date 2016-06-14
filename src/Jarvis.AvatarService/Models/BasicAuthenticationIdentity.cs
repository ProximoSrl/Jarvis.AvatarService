using System;

namespace Jarvis.AvatarService.Models
{
    public class BasicAuthenticationIdentity
    {
        public String UserName { get; set; }
        public String Password { get; set; }

        public BasicAuthenticationIdentity(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}