﻿using CSULB.GetUsGrub.BusinessLogic;
using CSULB.GetUsGrub.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CSULB.GetUsGrub
{
    public class AuthenticationFilter :  AuthorizationFilterAttribute
    {
        private readonly bool _isActive = true;

        public AuthenticationFilter() { }

        public AuthenticationFilter(bool isActive)
        {
            _isActive = isActive;
        }

        public override void OnAuthorization(HttpActionContext filterContext)
        {
            // If not active, then skip this authentication filter
            if (!_isActive) return;

            if (!IsUserAuthorized(filterContext))
            {
                AuthenticationTokenManager tokenManager = new AuthenticationTokenManager();
                AuthenticationToken authenticationToken;
                TokenService tokenService = new TokenService();

                // Extracting the tokenString from the Header
                var tokenString = tokenService.ExtractToken(request);

                // Checking if there is an empty or a null value to the token
                if (string.IsNullOrEmpty(tokenString))
                {
                    // This is done incase the request does not require authentication
                    return Task.Run(() => SendAsync(request, cancellationToken)).Result;
                }


                // Extract username from  the token
                var username = tokenService.GetTokenUsername(tokenString);

                // Checking if the Username is empty or null
                if (string.IsNullOrEmpty(username))
                {
                    return UserNotAuthenticated();
                }

                using (AuthenticationGateway gateway = new AuthenticationGateway())
                {
                    // Getting the Authentication Token Associated with the username
                    var gatewayResult = gateway.GetAuthenticationToken(username);

                    // Checking if there was an error Generated in the gateway if the string is not the same and its experation time has to be later than now
                    if (gatewayResult.Error != null || gatewayResult.Data.TokenString != tokenString ||
                        gatewayResult.Data.ExpiresOn.CompareTo(DateTime.Now) > 0)
                    {
                        return UserNotAuthenticated();
                    }

                    authenticationToken = gatewayResult.Data;
                }


                var tokenPrincible = tokenManager.GetTokenPrincipal(authenticationToken, out validatedToken);

                Thread.CurrentPrincipal = tokenPrincible;

                return Task.Run(() => SendAsync(request, cancellationToken)).Result;
            }
            catch (Exception)
            {
                return UserNotAuthenticated();
            }
        }

        /// <summary>
        /// 
        /// This method would throw an un authorized message if any expeption is thrown
        /// 
        /// </summary>
        /// <returns>
        /// Task with the response of 401 that the user is unauthenticated
        /// </returns>
        private HttpResponseMessage UserNotAuthenticated()
        {
            // Setting the message code to be a 401 
            var response = new HttpResponseMessage() { StatusCode = HttpStatusCode.Unauthorized };

            return response;
        }
    }
}