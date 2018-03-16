﻿using System;

namespace CSULB.GetUsGrub.Models
{
    /// <summary>
    /// The <c>BusinessHour</c> class.
    /// Defines properties pertaining to a business hour.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/10/2018
    /// </para>
    /// </summary>
    [Serializable]
    public class BusinessHour
    {
        public string Day { get; set; }

        public string OpenTime { get; set; }

        public string CloseTime { get; set; }
    }
}
