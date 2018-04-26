using CSULB.GetUsGrub.DataAccess;
using CSULB.GetUsGrub.Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Web;

namespace CSULB.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// Performs business logic and executes requests regarding user profiles
    /// 
    /// @author: Andrew Kao
    /// @updated: 4/28/18
    /// </summary>
    public class UserProfileManager : IProfileManager<UserProfileDto>
    {
        public ResponseDto<UserProfileDto> GetProfile(string token)
        {
            // Retrieve userID from db
            var userGateway = new UserGateway();

            var tokenService = new TokenService();

            var userAccountResponseDto = userGateway.GetUserByUsername(tokenService.GetTokenUsername(token));
            // Retrieve profile from database
            var profileGateway = new UserProfileGateway();

						var userProfileResponseDto = profileGateway.GetUserProfileById(userAccountResponseDto.Data.Id); // TODO: @Andrew, you had this and it doesnt exist anymore... profileGateway.GetUserProfileById(userAccountResponseDto.Data.Id); [-Angelica]

            return userProfileResponseDto;
        }

        public ResponseDto<bool> EditProfile(UserProfileDto userProfileDto, string token)
        {
            var editUserProfilePreLogicValidationStrategy = new EditUserProfilePreLogicValidationStrategy(userProfileDto);

            var result = editUserProfilePreLogicValidationStrategy.ExecuteStrategy();

            if (result.Error != null)
            {
                return new ResponseDto<bool>
                {
                    Data = true,
                    Error = "Something went wrong. Please try again later."
                };
            }

            // Extract DTO contents and map DTO to domain model
            var tokenService = new TokenService();
            var userProfileDomain = new UserProfile(userProfileDto.DisplayName, userProfileDto.DisplayPicture);

            // Retrieve userID from db
            var userGateway = new UserGateway();

            var userAccountResponseDto = userGateway.GetUserByUsername(tokenService.GetTokenUsername(token));

            // Execute update of database
            var profileGateway = new UserProfileGateway();

            var responseDtoFromGateway = profileGateway.EditUserProfileById(userAccountResponseDto.Data.Id, userProfileDomain);

            return responseDtoFromGateway;
        }

        //ImageUploadManager
        // TODO: @Angelica Add image profile upload here
        public ResponseDto<bool> ProfileImageUpload(HttpPostedFile image,  string username)
        {
            var user = new UserProfileDto() { Username = username};
  
            var ImageUploadValidationStrategy = new ImageUploadValidationStrategy(user, image);
            var result = ImageUploadValidationStrategy.ExecuteStrategy();

            if (result.Data == false)
            {
                return new ResponseDto<bool>()
                {
                    Data = false,
                    Error = result.Error
                };
            }

            var fileExtension = Path.GetExtension(image.FileName);
            var newImagename = username + fileExtension;

            // Saving Virtual Path
            var virtualPath = ImagePaths.VIRTUAL_PROFILE_IMAGE_PATH + newImagename;

            // Storing Virtual Path
            user.DisplayPicture = virtualPath;

            // Save Rooted Path
            string rootedPath = ImagePaths.PHSYICAL_PROFILE_IMAGE_PATH + newImagename;

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

                // Save the image to the path
                image.SaveAs(rootedPath); //savePath + newImagename
                
                return new ResponseDto<bool>
                {
                    Data = true
                };
            }
        }

    }
}