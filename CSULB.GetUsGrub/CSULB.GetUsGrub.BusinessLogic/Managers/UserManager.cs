﻿using CSULB.GetUsGrub.DataAccess;
using CSULB.GetUsGrub.Models;
using CSULB.GetUsGrub.UserAccessControl;
using System.Collections.Generic;
using System.Linq;

namespace CSULB.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// The <c>UserManager</c> class.
    /// Contains all methods for performing the create, get, update, delete actions for a user.
    /// <para>
    /// @author: Angelica, Jennifer Nguyen
    /// @updated: 03/12/2018
    /// </para>
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// The CreateIndividualUser method.
        /// Contains business logic to create an individual user.
        /// <para>
        /// @author: Jennifer Nguyen
        /// @updated: 03/13/2018
        /// </para>
        /// </summary>
        /// <param name="registerUserDto"></param>
        /// <returns>ResponseDto</returns>
        public ResponseDto<RegisterUserDto> CreateIndividualUser(RegisterUserDto registerUserDto)
        {
            var createIndividualPreLogicValidationStrategy = new CreateIndividualPreLogicValidationStrategy(registerUserDto);
            var securityQuestionMapper = new SecurityQuestionMapper();
            var userProfileMapper = new UserProfileMapper();
            var securityAnswerSalts = new List<SecurityAnswerSalt>();
            var saltGenerator = new SaltGenerator();
            var payloadHasher = new PayloadHasher();
            var claimsFactory = new ClaimsFactory();

            // Validate data transfer object
            var result = createIndividualPreLogicValidationStrategy.ExecuteStrategy();
            if (result.Error != null)
            {
                return new ResponseDto<RegisterUserDto>
                {
                    Data = registerUserDto,
                    Error = result.Error
                };
            }

            // Map data transfer object to domain models
            var userAccount = new UserAccount(registerUserDto.UserAccountDto);
            var securityQuestions = registerUserDto.SecurityQuestionDtos
                                        .Select(securityQuestionDto => securityQuestionMapper.MapDtoToModel(securityQuestionDto))
                                        .ToList();
            var userProfile = userProfileMapper.MapDtoToModel(registerUserDto.UserProfileDto);

            // Hash password
            var passwordSalt = new PasswordSalt(saltGenerator.GenerateSalt(128));
            userAccount.Password = payloadHasher.Sha256HashWithSalt(passwordSalt.Salt, userAccount.Password);

            // Hash security answers
            for (var i = 0; i < securityQuestions.Count; i++)
            {
                securityAnswerSalts.Add(new SecurityAnswerSalt { Salt = saltGenerator.GenerateSalt(128) });
                securityQuestions[i].Answer = payloadHasher.Sha256HashWithSalt(securityAnswerSalts[i].Salt, securityQuestions[i].Answer);
            }

            // Set claims
            var claims = new UserClaims
            {
                Claims = claimsFactory.CreateIndividualClaims()
            };

            // Set UserAccount to active
            userAccount.IsActive = true;

            // Set UserAccount is not first time user
            userAccount.IsFirstTimeUser = false;

            // Validate domain models
            var createIndividualPostLogicValdiationStrategy = new CreateIndividualPostLogicValidationStrategy(userAccount, securityQuestions, securityAnswerSalts, passwordSalt, claims, userProfile);
            var validateResult = createIndividualPostLogicValdiationStrategy.ExecuteStrategy();
            if (!validateResult)
            {
                return new ResponseDto<RegisterUserDto>
                {
                    Data = registerUserDto,
                    Error = "Something went wrong. Please try again later."
                };
            }

            // Store user in database
            using (var userGateway = new UserGateway())
            {
                var gatewayResult = userGateway.StoreIndividualUser(userAccount, passwordSalt, securityQuestions, securityAnswerSalts, claims, userProfile);
                if (gatewayResult.Data == false)
                {
                    return new ResponseDto<RegisterUserDto>()
                    {
                        Data = registerUserDto,
                        Error = gatewayResult.Error
                    };
                }
            }

            return new ResponseDto<RegisterUserDto>
            {
                Data = registerUserDto
            };
        }

        /// <summary>
        /// The CreateRestaurantUser method.
        /// Contains business logic for creating a restaurant user.
        /// <para>
        /// @author: Jennifer Nguyen
        /// @updated: 03/13/2018
        /// </para>
        /// </summary>
        /// <param name="registerRestaurantDto"></param>
        /// <returns>ResponseDto</returns>
        public ResponseDto<RegisterRestaurantDto> CreateRestaurantUser(RegisterRestaurantDto registerRestaurantDto)
        {
            var createRestaurantPreLogicValidationStrategy = new CreateRestaurantPreLogicValidationStrategy(registerRestaurantDto);
            var securityQuestionMapper = new SecurityQuestionMapper();
            var userProfileMapper = new UserProfileMapper();
            var restaurantProfileMapper = new RestaurantProfileMapper();
            var securityAnswerSalts = new List<SecurityAnswerSalt>();
            var saltGenerator = new SaltGenerator();
            var payloadHasher = new PayloadHasher();
            var claimsFactory = new ClaimsFactory();

            // Validate data transfer object
            var restaurantResult = createRestaurantPreLogicValidationStrategy.ExecuteStrategy();
            if (restaurantResult.Error != null)
            {
                return new ResponseDto<RegisterRestaurantDto>
                {
                    Data = registerRestaurantDto,
                    Error = restaurantResult.Error
                };
            }

            // Map data transfer object to domain models
            var userAccount = new UserAccount(registerRestaurantDto.UserAccountDto);
            var securityQuestions = registerRestaurantDto.SecurityQuestionDtos
                                        .Select(securityQuestionDto => securityQuestionMapper.MapDtoToModel(securityQuestionDto))
                                        .ToList();
            var userProfile = userProfileMapper.MapDtoToModel(registerRestaurantDto.UserProfileDto);
            var restaurantProfile = restaurantProfileMapper.MapDtoToModel(registerRestaurantDto.RestaurantProfileDto);

            // TODO: @Brian Need Google Map integration for following [-Jenn]
            // Get longitude and latitude given Address model to Google Map service
            // Set longitude and latitude

            // Hash password
            var passwordSalt = new PasswordSalt(saltGenerator.GenerateSalt(128));
            userAccount.Password = payloadHasher.Sha256HashWithSalt(passwordSalt.Salt, userAccount.Password);

            // Hash security answers
            for (var i = 0; i < securityQuestions.Count; i++)
            {
                securityAnswerSalts.Add(new SecurityAnswerSalt { Salt = saltGenerator.GenerateSalt(128) });
                securityQuestions[i].Answer = payloadHasher.Sha256HashWithSalt(securityAnswerSalts[i].Salt, securityQuestions[i].Answer);
            }

            // Set claims to be stored in UserClaims table
            var claims = new UserClaims
            {
                Claims = claimsFactory.CreateRestaurantClaims()
            };

            // Set UserAccount to active
            userAccount.IsActive = true;

            // Set UserAccount is not first time user
            userAccount.IsFirstTimeUser = false;

            // Validate domain models
            var createRestaurantPostLogicValdiationStrategy = new CreateRestaurantPostLogicValidationStrategy(userAccount, securityQuestions, securityAnswerSalts, passwordSalt, claims, userProfile, restaurantProfile);
            var validateResult = createRestaurantPostLogicValdiationStrategy.ExecuteStrategy();
            if (!validateResult)
            {
                return new ResponseDto<RegisterRestaurantDto>
                {
                    Data = registerRestaurantDto,
                    Error = "Something went wrong. Please try again later."
                };
            }

            // Store user in database
            using (var userGateway = new UserGateway())
            {
                var gatewayResult = userGateway.StoreRestaurantUser(userAccount, passwordSalt, securityQuestions, securityAnswerSalts, claims, userProfile, restaurantProfile);
                if (gatewayResult.Data == false)
                {
                    return new ResponseDto<RegisterRestaurantDto>()
                    {
                        Data = registerRestaurantDto,
                        Error = gatewayResult.Error
                    };
                }
            }

            return new ResponseDto<RegisterRestaurantDto>
            {
                Data = registerRestaurantDto
            };
        }

        /// <summary>
        /// The CreateIndividualUser method.
        /// Contains business logic to create an individual user.
        /// <para>
        /// @author: Jennifer Nguyen
        /// @updated: 03/13/2018
        /// </para>
        /// </summary>
        /// <param name="registerUserDto"></param>
        /// <returns>ResponseDto</returns>
        public ResponseDto<RegisterUserDto> CreateAdmin(RegisterUserDto registerUserDto)
        {
            var createIndividualPreLogicValidationStrategy = new CreateIndividualPreLogicValidationStrategy(registerUserDto);
            var securityQuestionMapper = new SecurityQuestionMapper();
            var userProfileMapper = new UserProfileMapper();
            var securityAnswerSalts = new List<SecurityAnswerSalt>();
            var saltGenerator = new SaltGenerator();
            var payloadHasher = new PayloadHasher();
            var claimsFactory = new ClaimsFactory();

            // Validate data transfer object
            var result = createIndividualPreLogicValidationStrategy.ExecuteStrategy();
            if (result.Error != null)
            {
                return new ResponseDto<RegisterUserDto>
                {
                    Data = registerUserDto,
                    Error = result.Error
                };
            }

            // Map data transfer object to domain models
            var userAccount = new UserAccount(registerUserDto.UserAccountDto);
            var securityQuestions = registerUserDto.SecurityQuestionDtos
                                        .Select(securityQuestionDto => securityQuestionMapper.MapDtoToModel(securityQuestionDto))
                                        .ToList();
            var userProfile = userProfileMapper.MapDtoToModel(registerUserDto.UserProfileDto);

            // Hash password
            var passwordSalt = new PasswordSalt(saltGenerator.GenerateSalt(128));
            userAccount.Password = payloadHasher.Sha256HashWithSalt(passwordSalt.Salt, userAccount.Password);

            // Hash security answers
            for (var i = 0; i < securityQuestions.Count; i++)
            {
                securityAnswerSalts.Add(new SecurityAnswerSalt { Salt = saltGenerator.GenerateSalt(128) });
                securityQuestions[i].Answer = payloadHasher.Sha256HashWithSalt(securityAnswerSalts[i].Salt, securityQuestions[i].Answer);
            }

            // Set claims
            var claims = new UserClaims
            {
                Claims = claimsFactory.CreateAdminClaims()//Gives admin claims
            };

            // Set UserAccount to active
            userAccount.IsActive = true;

            // Set UserAccount is not first time user
            userAccount.IsFirstTimeUser = false;

            // Validate domain models
            var createIndividualPostLogicValdiationStrategy = new CreateIndividualPostLogicValidationStrategy(userAccount, securityQuestions, securityAnswerSalts, passwordSalt, claims, userProfile);
            var validateResult = createIndividualPostLogicValdiationStrategy.ExecuteStrategy();
            if (!validateResult)
            {
                return new ResponseDto<RegisterUserDto>
                {
                    Data = registerUserDto,
                    Error = "Something went wrong. Please try again later."
                };
            }

            // Store user in database
            using (var userGateway = new UserGateway())
            {
                var gatewayResult = userGateway.StoreIndividualUser(userAccount, passwordSalt, securityQuestions, securityAnswerSalts, claims, userProfile);
                if (gatewayResult.Data == false)
                {
                    return new ResponseDto<RegisterUserDto>()
                    {
                        Data = registerUserDto,
                        Error = gatewayResult.Error
                    };
                }
            }
            return new ResponseDto<RegisterUserDto>
            {
                Data = registerUserDto
            };
        }

        /// <summary>
        /// DeactivateUser deactivates the user when given a username.
        /// @author: Angelica Salas Tovar
        /// @update: 03/20/2018
        /// </summary>
        /// <param name="username">The user that will be deactivated.</param>
        /// <returns>Response Dto</returns>
        public ResponseDto<bool> DeactivateUser(string username)
        {
            using (var gateway = new UserGateway())
            {
                var gatewayResult = gateway.DeactivateUser(username);

                if (gatewayResult.Data == false)//If gateway has failed return false
                {
                    return new ResponseDto<bool>()
                    {
                        Data = false,
                        Error = gatewayResult.Error
                    };
                }
                return new ResponseDto<bool>//If gateway was successful return true
                {
                    Data = true
                };
            }
        }

        /// <summary>
        /// ReactivateUser reactivates the user when given a username.
        /// @author: Angelica Salas Tovar
        /// @update: 03/20/2018
        /// </summary>
        /// <param name="username">The user that will be reactivated.</param>
        /// <returns>Response Dto</returns>
        public ResponseDto<string> ReactivateUser(string username)
            {
                using (var gateway = new UserGateway())
                {
                    var gatewayResult = gateway.ReactivateUser(username);
                if (gatewayResult.Data == false)
                {
                    return new ResponseDto<string>()
                    {
                        Data = username,
                        Error = gatewayResult.Error
                    };
                }
                return new ResponseDto<string>
                {
                    Data = "Success!"
                };
            }
            }

        /// <summary>
        /// DeleteUser deletes the user when given a username.
        /// @author: Angelica Salas Tovar
        /// @update: 03/20/2018
        /// </summary>
        /// <param name="username">The user that will be deleted.</param>
        /// <returns>Response Dto</returns>
        public ResponseDto<string> DeleteUser(string username)
            {
                using (var gateway = new UserGateway())
                {
                    var gatewayResult = gateway.DeleteUser(username);
                if (gatewayResult.Data == false)
                {
                    return new ResponseDto<string>()
                    {
                        Data = username,
                        Error = gatewayResult.Error
                    };
                }
                return new ResponseDto<string>
                {
                    Data = "Success!"
                };
            }
            }
        /// <summary>
        /// EditUser edits the user when given a.
        /// @author: Angelica Salas Tovar
        /// @update: 03/20/2018
        /// </summary>
        /// <param name="username">The user that will be deactivated.</param>
        /// <returns>Response Dto</returns>
        public ResponseDto<EditUserDto> edituser(EditUserDto user)//@TODO:Change name later
        {
            using (var gateway = new UserGateway())
            {
                var gatewayresult = gateway.EditUser(user);
                if (gatewayresult.Data == false)
                {
                    return new ResponseDto<EditUserDto>()
                    {
                        Data = user,
                        Error = gatewayresult.Error
                    };
                }
                return new ResponseDto<EditUserDto>
                {
                    Data = user
                };
            }
        }
    }
}
