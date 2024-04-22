﻿using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}