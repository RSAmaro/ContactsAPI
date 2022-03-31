﻿namespace ContactsAPI.Models.User
{
    public class AuthDTO
    {
        public string UserName { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public string[] Roles { get; set; }
    }
}
