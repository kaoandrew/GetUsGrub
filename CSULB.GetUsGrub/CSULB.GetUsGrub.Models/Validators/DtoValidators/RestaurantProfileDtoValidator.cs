﻿using FluentValidation;

namespace CSULB.GetUsGrub.Models
{
    /// <summary>
    /// The <c>RestaurantProfileDtoValidator</c> class.
    /// Defines rules to validate a RestaurantProfileDto.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/10/2018
    /// </para>
    /// </summary>
    public class RestaurantProfileDtoValidator : AbstractValidator<RestaurantProfileDto>
    {
        public RestaurantProfileDtoValidator()
        {
            RuleFor(x => x.BusinessHours)
                .NotEmpty().WithMessage("At least one business hour must be filled.")
                .NotNull().WithMessage("At least one business hour must be filled.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .NotNull().WithMessage("Phone number is required.")
                .Matches(@"^\([2-9]\d{2}\)\d{3}\-\d{4}$").WithMessage("Phone number must be in (XXX)XXX-XXXX format.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .NotNull().WithMessage("Address is required");
        }
    }
}
