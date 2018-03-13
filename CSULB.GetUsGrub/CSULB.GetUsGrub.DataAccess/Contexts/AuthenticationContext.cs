﻿using CSULB.GetUsGrub.Models;
using System.Data.Entity;

namespace CSULB.GetUsGrub.DataAccess
{
    // TODO: @Brian Add SecurityAnswerSalts and Claims? [-Jenn]
    /// <summary>
    /// Context containing all tables for user management.
    /// 
    /// @Created by: Brian Fann
    /// @Last Updated: 3/9/18
    /// </summary>
    public class AuthenticationContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<PasswordSalt> PasswordSalts { get; set; }

        public DbSet<Token> Tokens { get; set; }

        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }

        public AuthenticationContext() : base("GetUsGrub") { }
    }

}