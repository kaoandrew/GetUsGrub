﻿using CSULB.GetUsGrub.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSULB.GetUsGrub.BusinessLogic
{
    // TODO: @Jenn Comment this [-Jenn]
    public class SsoTokenPostLogicValidationStrategy
    {
        private readonly SsoToken _ssoToken;

        public SsoTokenPostLogicValidationStrategy(SsoToken ssoToken)
        {
            _ssoToken = ssoToken;
        }

        public ResponseDto<bool> ExecuteStrategy()
        {
            var validationWrappers = new List<IValidationWrapper>()
            {
                new ValidationWrapper<SsoTokenPayload>(_ssoToken.SsoTokenPayload, new SsoTokenPayloadValidator())
            };
            Debug.WriteLine("Validation5");
            foreach (var validationWrapper in validationWrappers)
            {
                var result = validationWrapper.ExecuteValidator();
                Debug.WriteLine(JsonConvert.SerializeObject(result));
                if (!result.Data)
                {
                    result.Error = SsoErrorMessages.INVALID_TOKEN_PAYLOAD;
                    return result;
                }
            }
            Debug.WriteLine("Validation1234");
            return new ResponseDto<bool>()
            {
                Data = true
            };
        }
    }
}