﻿using CSULB.GetUsGrub.DataAccess;
using CSULB.GetUsGrub.Models;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace CSULB.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// Performs business logic and executes requests regarding restaurant profiles
    /// 
    /// @author: Andrew Kao
    /// @updated: 3/18/18
    /// </summary>
    public class RestaurantProfileManager : IProfileManager<RestaurantProfileDto>
    {
        public ResponseDto<RestaurantProfileDto> GetProfile(string username)
        {
            // Retrieve profile from database
            var profileGateway = new RestaurantProfileGateway();

            var responseDtoFromGateway = profileGateway.GetRestaurantProfileByUsername(username);

            return responseDtoFromGateway;
        }

        public ResponseDto<bool> EditProfile(RestaurantProfileDto restaurantProfileDto)
        {
            // Prelogic validation strategy TODO: @andrew move this to after the 
            var editRestaurantProfilePreLogicValidationStrategy = new EditRestaurantUserProfilePreLogicValidationStrategy(restaurantProfileDto);

            var result = editRestaurantProfilePreLogicValidationStrategy.ExecuteStrategy();

            if (result.Error != null)
            {
                return new ResponseDto<bool>
                {
                    Data = false,
                    Error = "Something went wrong. Please try again later."
                };
            }

            // Extract DTO contents and map DTO to domain model

            var username = restaurantProfileDto.Username;

            // Extract restaurant profile domain
            var restaurantProfileDomain = new RestaurantProfile(
                restaurantProfileDto.PhoneNumber,
                restaurantProfileDto.Address,
                restaurantProfileDto.Details,
                restaurantProfileDto.GeoCoordinates.Latitude,
                restaurantProfileDto.GeoCoordinates.Longitude);

            // Extract business hours domains
            var businessHourDomains = restaurantProfileDto.BusinessHours;


            // Extract restaurant menu dictionary
            var restaurantMenuDomains = restaurantProfileDto.MenuDictionary;

            // Extract menu items

            // Execute update of database
            var profileGateway = new RestaurantProfileGateway();

            var responseDtoFromGateway = profileGateway.EditRestaurantProfile(username, restaurantProfileDomain, businessHourDomains, restaurantMenuDomains);

            return responseDtoFromGateway;
        }

        //ImageUploadManager
        // TODO: @Angelica Add image profile upload here
        public ResponseDto<bool> MenuItemImageUpload(HttpPostedFile image, string username)
        {
            var user = new UserProfileDto() { Username = username };

            var ImageUploadValidationStrategy = new ImageUploadValidationStrategy(user, image);
            var result = ImageUploadValidationStrategy.ExecuteStrategy();

            var renameImage = Path.GetExtension(image.FileName);
            var newImagename = username + renameImage;

            // Save image to path
            string savePath = ConfigurationManager.AppSettings["MenuImagePath"];
            string filename = Path.GetFileName(image.FileName);// file name should be username.png
            user.DisplayPicture = savePath + filename; // Store image path to DTO

            // Call gateway to save path to database
            using (var gateway = new UserProfileGateway())
            {
                var gatewayresult = gateway.UploadImage(user);
                if (gatewayresult.Data == false)
                {
                    return new ResponseDto<bool>()
                    {
                        Data = false,
                        Error = gatewayresult.Error
                    };
                }

                image.SaveAs(savePath + filename);

                return new ResponseDto<bool>
                {
                    Data = true
                };
            }
        }
    }
}