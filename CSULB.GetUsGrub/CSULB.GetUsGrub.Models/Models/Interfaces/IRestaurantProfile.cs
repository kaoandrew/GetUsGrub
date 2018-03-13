﻿using System.Collections.Generic;

namespace CSULB.GetUsGrub.Models
{
    /// <summary>
    /// The IRestaurantProfile interface.
    /// A contract with defined properties for the RestaurantProfile class.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/10/2018
    /// </para>
    /// </summary>
    public interface IRestaurantProfile
    {
        IList<BusinessHour> BusinessHours { get; }
        string PhoneNumber { get; }
        Address Address { get; }
        double Longitude { get; }
        double Latitude { get; }
    }
}
