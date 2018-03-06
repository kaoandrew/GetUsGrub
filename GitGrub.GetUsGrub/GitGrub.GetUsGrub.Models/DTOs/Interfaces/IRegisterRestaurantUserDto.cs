﻿using System.ComponentModel.DataAnnotations;

namespace GitGrub.GetUsGrub.Models
{
    /// <summary>
    /// The IRegisterRestaurantUserDto interface.
    /// A contract with defined properties for the RegisterRestaurantUserDto class.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/05/2017
    /// </para>
    /// </summary>
    public interface IRegisterRestaurantUserDto : IRegisterUserDto
    {
        [Required]
        RestaurantAccount RestaurantAccount { get; set; }
    }
}