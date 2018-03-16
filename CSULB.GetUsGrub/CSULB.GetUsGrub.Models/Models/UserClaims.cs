﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace CSULB.GetUsGrub.Models
{
    /// <summary>
    /// The <c>UserClaims</c> class.
    /// Defines properties pertaining to a user's claims.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/12/2018
    /// </para>
    /// </summary>
    [Table("GetUsGrub.UserClaims")]
    public class UserClaims
    {
        [Key]
        [ForeignKey("UserAccount")]
        public int? Id { get; set; }

        [NotMapped]
        public ICollection<Claim> Claims { get; set; }

        public string ClaimsJson
        {
            get => JsonConvert.SerializeObject(Claims);
            set => Claims = JsonConvert.DeserializeObject<Collection<Claim>>(value);
        }

        // Navigation Property
        public virtual UserAccount UserAccount { get; set; }
    }
}
