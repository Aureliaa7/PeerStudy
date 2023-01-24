﻿using System;
using System.ComponentModel.DataAnnotations;

namespace PeerStudy.Core.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "The password and confirmed password do not match.")]
        public string ConfirmedPassword { get; set;}
    }
}
