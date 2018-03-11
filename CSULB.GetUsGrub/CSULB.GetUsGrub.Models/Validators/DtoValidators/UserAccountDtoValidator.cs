﻿using FluentValidation;

namespace CSULB.GetUsGrub.Models
{
    /// <summary>
    /// The <c>UserAccountValidator</c> class.
    /// Defines rules to validate a UserAccountDto.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/10/2018
    /// </para>
    /// </summary>
    public class UserAccountDtoValidator : AbstractValidator<UserAccountDto>
    {
        public UserAccountDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .NotNull().WithMessage("Username is required.")
                .Matches(@"^[A-Za-z\d]+$").WithMessage("Username must not contain spaces and special characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .NotNull().WithMessage("Password is required.")
                .Length(8, 64).WithMessage("Password must be at least 8 characters and less than or equal to 64.")
                .Matches(@"^[^\s]+$").WithMessage("Password must not contain spaces.");
        }
    }
}
