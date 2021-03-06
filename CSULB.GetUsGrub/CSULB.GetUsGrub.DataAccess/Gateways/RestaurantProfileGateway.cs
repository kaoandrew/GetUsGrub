﻿using CSULB.GetUsGrub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CSULB.GetUsGrub.DataAccess
{
    /// <summary>
    /// Restaurant profile queries
    /// 
    /// @author: Andrew Kao
    /// @updated: 4/26/18
    /// </summary>
    public class RestaurantProfileGateway: IDisposable
    {
        // Open the Restaurant context
        RestaurantContext context = new RestaurantContext();

        /// <summary>
        /// Returns restaurant profile dto inside response dto
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// 
        // move context up to here
        public ResponseDto<RestaurantProfileDto> GetRestaurantProfileById(int? id)
        {       
            using (context)
            {
                var dbUserProfile = (from profile in context.UserProfiles
                                     where profile.Id == id
                                     select profile).SingleOrDefault();

                var userProfileDomain = new UserProfile(dbUserProfile.Id, dbUserProfile.DisplayName, dbUserProfile.DisplayPicture);
                // Find restaurant associated with the ID
                var dbRestaurantProfile = (from restaurantProfile in context.RestaurantProfiles
                                           where restaurantProfile.Id == dbUserProfile.Id
                                           select restaurantProfile).SingleOrDefault();

                var restaurantProfileDomain = new RestaurantProfile(dbRestaurantProfile.PhoneNumber, dbRestaurantProfile.Address, dbRestaurantProfile.Details);

                // Find restaurant's business hours
                var businessHourDtos = (from businessHour in context.BusinessHours
                                          where businessHour.RestaurantId == dbRestaurantProfile.Id
                                          select new RestaurantBusinessHourDto()
                                          {
                                              Id = businessHour.Id,
                                              Day = businessHour.Day,
                                              OpenDateTime = businessHour.OpenTime,
                                              CloseDateTime = businessHour.CloseTime,
                                              TimeZone = businessHour.TimeZone
                                          }).ToList();

                IList<RestaurantMenuWithItems> restaurantMenusList = new List<RestaurantMenuWithItems>();

                var dbRestaurantMenus = dbRestaurantProfile.RestaurantMenu;

                if (dbRestaurantMenus.Count > 0)
                {
                    foreach (var menu in dbRestaurantMenus)
                    {
                        // Create the menu domain
                        var menuDomain = new RestaurantMenu(menu.Id, menu.MenuName, menu.IsActive, menu.Flag);

                        // Create the list for the menu items
                        var menuItemDomains = new List<RestaurantMenuItem>();
                        // Then, find all menu items associated with each menu and turn that into a list
                        var dbMenuItems = (from menuItems in context.RestaurantMenuItems
                                     where menuItems.MenuId == menu.Id
                                     select menuItems).ToList();

                        foreach (var item in dbMenuItems)
                        {
                            var menuItemDomain = new RestaurantMenuItem(item.Id, item.ItemName, item.ItemPrice, item.ItemPicture, item.Tag, item.Description, item.IsActive, item.Flag);
                            menuItemDomains.Add(menuItemDomain);
                            // Map menu items to menus in a dictionary
                        }
                        var restaurantMenuWithItems = new RestaurantMenuWithItems(menuDomain, menuItemDomains);
                        restaurantMenusList.Add(restaurantMenuWithItems);
                    }
                }

                else
                {
                    restaurantMenusList = new List<RestaurantMenuWithItems>();
                }
                
                ResponseDto<RestaurantProfileDto> responseDto = new ResponseDto<RestaurantProfileDto>
                {
                    Data = new RestaurantProfileDto(userProfileDomain, restaurantProfileDomain, businessHourDtos, restaurantMenusList),
                    Error = null
                };
                return responseDto;
            }
            
        }

        /// <summary>
        /// Returns true if update process succeeds, false if fails
        /// </summary>
        /// <param name="restaurantProfileDto"></param>
        /// <returns></returns>
        public ResponseDto<bool> EditRestaurantProfileById(int? id, UserProfile userProfileDomain, RestaurantProfile restaurantProfileDomain, IList<RestaurantBusinessHourDto> RestaurantBusinessHourDtos, IList<RestaurantMenuWithItems> restaurantMenuDomains)
        {
            using (context)
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Find profile associated with account
                        var dbUserProfile = (from profile in context.UserProfiles
                                             where profile.Id == id
                                             select profile).SingleOrDefault();

                        // Find restaurant associated with profile
                        var dbRestaurantProfile = dbUserProfile.RestaurantProfile;

                        // Find restaurant's business hours
                        var dbBusinessHours = dbRestaurantProfile.BusinessHours;

                        // Then, find all menus associated with this restaurant and their menu items
                        IList<RestaurantMenuWithItems> dbRestaurantMenusList = new List<RestaurantMenuWithItems>();

                        var dbRestaurantMenus = dbRestaurantProfile.RestaurantMenu;

                        foreach (var menu in dbRestaurantMenus)
                        {
                            // create the list for the menu items
                            var dbMenuItemsList = new List<RestaurantMenuItem>();

                            // Then, find all menu items associated with each menu and turn that into a list
                            var dbMenuItems = (from menuItems in context.RestaurantMenuItems
                                               where menuItems.MenuId == menu.Id
                                               select menuItems).ToList();

                            foreach (var item in dbMenuItems)
                            {
                                dbMenuItemsList.Add(item);
                                // Map menu items to menus in a dictionary
                            }
                            var dbRestaurantMenuWithItems = new RestaurantMenuWithItems(menu, dbMenuItemsList);
                            dbRestaurantMenusList.Add(dbRestaurantMenuWithItems);
                        }

                        // Update associated user profile
                        dbUserProfile.DisplayName = userProfileDomain.DisplayName;
                        context.SaveChanges();

                        // Update restaurant profile
                        dbRestaurantProfile.PhoneNumber = restaurantProfileDomain.PhoneNumber;
                        dbRestaurantProfile.Address = restaurantProfileDomain.Address;
                        dbRestaurantProfile.Details = restaurantProfileDomain.Details;
                        dbRestaurantProfile.GeoCoordinates = restaurantProfileDomain.GeoCoordinates;
                        dbRestaurantProfile = restaurantProfileDomain;
                        context.SaveChanges();

                        // Find the business hours on the database that have the same Ids as the new business hours

                        foreach (var restaurantBusinessHourDto in RestaurantBusinessHourDtos)
                        {
                            Flag flag = restaurantBusinessHourDto.Flag;
                            switch (flag)
                            {
                                case Flag.NotSet:
                                    break;
                                case Flag.Add:
                                    // Reset flag
                                    restaurantBusinessHourDto.Flag = 0;
                                    var businessHourDomain = new BusinessHour(restaurantBusinessHourDto.TimeZone, restaurantBusinessHourDto.Day, restaurantBusinessHourDto.OpenDateTime, restaurantBusinessHourDto.CloseDateTime);
                                    dbBusinessHours.Add(businessHourDomain);
                                    context.SaveChanges();
                                    break;
                                case Flag.Edit:
                                    // find the corresponding businessHour by ID
                                    var dbBusinessHour = (from dbHour in context.BusinessHours
                                                          where dbHour.Id == restaurantBusinessHourDto.Id
                                                          select dbHour).SingleOrDefault();
                                    dbBusinessHour.Day = restaurantBusinessHourDto.Day;
                                    dbBusinessHour.OpenTime = restaurantBusinessHourDto.OpenDateTime;
                                    dbBusinessHour.CloseTime = restaurantBusinessHourDto.CloseDateTime;
                                    dbBusinessHour.TimeZone = restaurantBusinessHourDto.TimeZone;
                                    context.SaveChanges();
                                    break;
                                case Flag.Delete:
                                    // Find the corresponding businessHour by ID
                                    dbBusinessHour = (from hour in context.BusinessHours
                                                        where hour.Id == restaurantBusinessHourDto.Id
                                                        select hour).SingleOrDefault();
                                    context.BusinessHours.Remove(dbBusinessHour);
                                    context.SaveChanges();
                                    break;
                            }
                        }

                        // Update menu
                        foreach (var restaurantMenuWithItems in restaurantMenuDomains)
                        {
                            Flag flag = restaurantMenuWithItems.RestaurantMenu.Flag;
                            switch(flag)
                            {
                                case Flag.NotSet:
                                    break;
                                case Flag.Add:
                                    restaurantMenuWithItems.RestaurantMenu.Flag = 0;
                                    restaurantMenuWithItems.RestaurantMenu.RestaurantMenuItems = new Collection<RestaurantMenuItem>();
                                    dbRestaurantMenus.Add(restaurantMenuWithItems.RestaurantMenu);
                                    context.SaveChanges();

                                    // Add the menu items inside the menu
                                    foreach (var menuItem in restaurantMenuWithItems.MenuItem)
                                    {
                                        // Find the corresponding menu
                                        var dbRestaurantMenu = (from menu in context.RestaurantMenus
                                                                where menu.Id == restaurantMenuWithItems.RestaurantMenu.Id
                                                                select menu).SingleOrDefault();
                                        // Reset flag
                                        menuItem.Flag = 0;
                                        dbRestaurantMenu.RestaurantMenuItems.Add(menuItem);
                                        context.SaveChanges();
                                    }
                                    break;
                                case Flag.Edit:
                                    // Query for menu with the same ID
                                    var dbMenu = (from menu in context.RestaurantMenus
                                                          where menu.Id == restaurantMenuWithItems.RestaurantMenu.Id
                                                          select menu).SingleOrDefault();
                                    dbMenu.MenuName = restaurantMenuWithItems.RestaurantMenu.MenuName;
                                    dbMenu.IsActive = restaurantMenuWithItems.RestaurantMenu.IsActive;
                                    context.SaveChanges();
                                    break;
                                case Flag.Delete:
                                    // Retrieves the menu from the db
                                    dbMenu = (from menu in context.RestaurantMenus
                                              where menu.Id == restaurantMenuWithItems.RestaurantMenu.Id
                                              select menu).SingleOrDefault();
                                    context.RestaurantMenus.Remove(dbMenu);
                                    // Iterate through that menu's menu items
                                    foreach (var menuItem in restaurantMenuWithItems.MenuItem)
                                    {
                                       var dbMenuItem = (from item in context.RestaurantMenuItems
                                                         where item.Id == menuItem.Id
                                                         select item).SingleOrDefault();
                                        context.RestaurantMenuItems.Remove(dbMenuItem);
                                        context.SaveChanges();
                                    }
                                    break;
                            }
                        }
                        
                        // Update menu items
                        foreach (var restaurantMenuWithItems in restaurantMenuDomains)
                        {
                            foreach (var menuItem in restaurantMenuWithItems.MenuItem)
                            {
                                Flag flag = menuItem.Flag;
                                switch (flag)
                                {
                                    case Flag.Add:
                                        // Find the corresponding menu
                                        var dbRestaurantMenu = (from menu in context.RestaurantMenus
                                                                where menu.Id == restaurantMenuWithItems.RestaurantMenu.Id
                                                                select menu).SingleOrDefault();
                                        // Reset flag
                                        menuItem.Flag = 0;
                                        dbRestaurantMenu.RestaurantMenuItems.Add(menuItem);
                                        context.SaveChanges();
                                        break;
                                    case Flag.Edit:
                                        // Query for menu item with the same ID
                                        var dbMenuItem = (from item in context.RestaurantMenuItems
                                                          where item.Id == menuItem.Id
                                                          select item).SingleOrDefault();
                                        dbMenuItem.ItemName = menuItem.ItemName;
                                        dbMenuItem.ItemPicture = menuItem.ItemPicture;
                                        dbMenuItem.ItemPrice = menuItem.ItemPrice;
                                        dbMenuItem.Tag = menuItem.Tag;
                                        dbMenuItem.Description = menuItem.Description;
                                        dbMenuItem.IsActive = menuItem.IsActive;
                                        context.SaveChanges();
                                        break;
                                    case Flag.Delete:
                                        dbMenuItem = (from item in context.RestaurantMenuItems
                                                      where item.Id == menuItem.Id
                                                      select item).SingleOrDefault();
                                        context.RestaurantMenuItems.Remove(dbMenuItem);
                                        context.SaveChanges();
                                        break;
                                }
                            }
                        }
                        

                        context.SaveChanges();
                        dbContextTransaction.Commit();

                        ResponseDto<bool> responseDto = new ResponseDto<bool>
                        {
                            Data = true,
                            Error = null
                        };
                        return responseDto;
                    }

                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();

                        ResponseDto<bool> responseDto = new ResponseDto<bool>
                        {
                            Data = false,
                            Error = GeneralErrorMessages.GENERAL_ERROR
                        };
                        return responseDto;
                    }
                }
            }
            
        }

        /// <summary>
        /// Stores virtual menu image path in database.
        /// <para>
        /// @author: Angelica Salas Tovar
        /// @update: 04/26/2018
        /// </para>
        /// </summary>
        /// <param name="userProfileDto">The user profile</param>
        /// <param name="menuPath">The virtual menu image path</param>
        /// <param name="menuId">The menu id</param>
        /// <returns></returns>
        public ResponseDto<bool> UploadImage(UserProfileDto userProfileDto, string menuPath, int menuId)
        {
            using (var userContext = new UserContext())
            {
                using (var dbContextTransaction = userContext.Database.BeginTransaction())
                {
                    try
                    {
                        // RestaurantMenuItems
                        var userRestaurantMenuItems = (from restaurantMenuItems in userContext.RestaurantMenuItems
                                                       where restaurantMenuItems.Id == menuId
                                                       select restaurantMenuItems).FirstOrDefault();

                        // To avoid images being stored with the same name and different extensions
                        if (userRestaurantMenuItems.ItemPicture != ImagePaths.DEFAULT_VIRTUAL_MENU_ITEM_PATH)
                        {
                            userRestaurantMenuItems.ItemPicture = ImagePaths.DEFAULT_VIRTUAL_MENU_ITEM_PATH;
                        }

                        // Checks if restaurant menu items result is null, if not then change image paths
                        userRestaurantMenuItems.ItemPicture = menuPath;
                        
                        userContext.SaveChanges();
                        dbContextTransaction.Commit();

                        return new ResponseDto<bool>()
                        {
                            Data = true
                        };
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();

                        return new ResponseDto<bool>()
                        {
                            Data = false,
                            Error = GeneralErrorMessages.GENERAL_ERROR
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Dispose of the context
        /// </summary>
        void IDisposable.Dispose()
        {
            context.Dispose();
        }
    }
}
