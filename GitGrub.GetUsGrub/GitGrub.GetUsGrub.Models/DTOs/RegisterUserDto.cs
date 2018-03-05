﻿using System.Collections.Generic;
using System.Security.Claims;

namespace GitGrub.GetUsGrub.Models
{
    public class RegisterUserDto : IRegisterUserDto
    {
        public UserAccount UserAccount { get; set; }

        public PasswordSalt PasswordSalt { get; set; }

        public IList<SecurityQuestion> SecurityQuestions { get; set; }

        public IList<SecurityAnswerSalt> SecurityAnswerSalts { get; set; }

        public ICollection<Claim> Claims { get; set; }
    }
}