﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GitGrub.GetUsGrub.Models.Models;

namespace GitGrub.GetUsGrub.Controllers
{
    public class DeactivateUserController : ApiController
    {
        //Creating the service that will handle the business logic (UserManager.cs)
        public UserManager UserManager { get; }
        //parameterless constructor
        public DeactivateUserController()
        {
        }
        //create a controller for the service...
        public DeactivateUserController(UserManager userManager)
        {
            UserManager = userManager;
        }


        [Authorize(Roles = "Administrators")]
        [Route("api/DeactivateUser/")]
        [HttpPost]
        public IHttpActionResult deactivateUser(User user)
        {
            if (!ModelState.IsValid)//if the model is not valid return a bad request
            {
                return BadRequest("inside invalid model state");
            }

            try
            {
                UserManager.deactivateUser(user);//call manager/service/business logic

                //call gateway
            }
            catch //not sure what i am catching..
            {
                return BadRequest("Inside catch");
            }

            return Ok(user);

        }

    }
}
