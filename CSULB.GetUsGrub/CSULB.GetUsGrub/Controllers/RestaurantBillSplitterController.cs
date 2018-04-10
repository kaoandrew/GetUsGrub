﻿using CSULB.GetUsGrub.BusinessLogic;
using CSULB.GetUsGrub.Models;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;

namespace CSULB.GetUsGrub.Controllers
{
		/// <summary>
		/// Retreives information from a restaurant by its public restaurant ID
		/// and returns the Menus and MenuItems of each Menu that specific restaurant has.
		/// @author Ryan Luong
		/// @updated 4/4/18
		/// </summary>
		[RoutePrefix("RestaurantBillSplitter")]
		public class RestaurantBillSplitterController : ApiController
		{
				[HttpGet]
				[AllowAnonymous] // TODO: Remove for deployment
				[Route("Restaurant")]
				[EnableCors(origins: "http://localhost:8080", headers: "*", methods: "GET")]
				public IHttpActionResult GetRestaurantMenus(int restaurantID)
				{
						var restaurantDto = new RestaurantDto(restaurantID);

						if (!ModelState.IsValid)
						{
								return BadRequest(ModelState);
						}
						try
						{
								var restaurantBillSplitterManager = new RestaurantBillSplitterManager();
								var response = restaurantBillSplitterManager.GetRestaurantMenus(restaurantDto.RestaurantID);
								if (response.Error != null)
								{
										return BadRequest(response.Error);
								}								
								return Ok(response);
						}
						catch (Exception e)
						{
								return BadRequest(e.Message);
						}
				}
		}
}