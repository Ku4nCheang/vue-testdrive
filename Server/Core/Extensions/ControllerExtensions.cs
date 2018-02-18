using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using netcore.Core.ErrorDescribers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace netcore.Core.Extensions
{

    public static class ControllerExtensions 
    {
        public static JsonResult JsonSuccess(this Controller ctrl, Object result = null)
        {
            return ctrl.Json(new {}).Success(result);
        }

        public static JsonResult JsonNotContent(this Controller ctrl)
        {
            return ctrl.Json(new {}).Success(null, (int)HttpStatusCode.NoContent);
        }

        public static JsonResult JsonCreated(this Controller ctrl, Object result )
        {
            return ctrl.Json(new {}).Success(result, (int)HttpStatusCode.Created);
        }

        public static JsonResult JsonAccepted(this Controller ctrl, Object result )
        {
            return ctrl.Json(new {}).Success(result, (int)HttpStatusCode.Accepted);
        }

        public static JsonResult JsonBadRequest(this Controller ctrl, Error error = null)
        {
            error = (error != null)? error: new Error();
            return ctrl.Json(new {}).BadRequest(error.Code, error.Description);
        }

        public static JsonResult JsonError(this Controller ctrl, Error error = null)
        {
            error = (error != null)? error: new Error();
            return ctrl.Json(new {}).Error(error.Code, error.Description);
        }

        public static JsonResult JsonUnauthorized(this Controller ctrl, Error error = null)
        {
            error = (error != null)? error: new Error();
            return ctrl.Json(new{}).Unauthorized(error.Code, error.Description);
        }

        public static JsonResult JsonNotFound(this Controller ctrl, Error error = null)
        {
            error = (error != null)? error: new Error();
            return ctrl.Json(new{}).NotFound(error.Code, error.Description);
        }

        public static JsonResult JsonInvalidModelState(this Controller ctrl, ModelStateDictionary modelState, Error error = null )
        {
            error = (error != null)? error: new Error();
            return ctrl.Json(new{}).InvalidModelState(modelState, error.Code, error.Description);
        }
    }

    public class ModelFieldError
    {
        public string FieldKey { get; set; }
        public string ErrorMessage { get; set; }

        public ModelFieldError (string fieldKey, string errorMessage) {
            FieldKey = fieldKey;
            ErrorMessage = errorMessage;
        }
    }

    public static class ModelStateExtensions
    {
        public static IEnumerable<ModelFieldError> AllErrors(this ModelStateDictionary modelState)
        {
            var result = from ms in modelState
                        where ms.Value.Errors.Any()
                            let fieldKey = ms.Key
                            let errors = ms.Value.Errors
                            from error in errors
                            select new ModelFieldError(fieldKey, error.ErrorMessage);

            return result;
        }
    }

    public static class JsonResultExtensions
    {
        public static JsonResult Success(this JsonResult response, object result, int statusCode = (int) HttpStatusCode.OK)
        {
            response.StatusCode = statusCode;
            response.Value = new {
                Success = true,
                Result = result
            };

            return response;
        }

        public static JsonResult Error(this JsonResult response, string errorCode = "", string errorMessage = "")
        {
            response.StatusCode = (int) HttpStatusCode.OK;
            response.Value = new {
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage 
            };

            return response;
        }

        public static JsonResult BadRequest(this JsonResult response, string errorCode = "", string errorMessage = "")
        {
            response.StatusCode = (int) HttpStatusCode.BadRequest;
            response.Value = new {
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage 
            };

            return response;
        }

        public static JsonResult Unauthorized(this JsonResult response, string errorCode = "", string errorMessage = "")
        {
            response.StatusCode = (int) HttpStatusCode.Unauthorized;
            response.Value = new {
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage 
            };

            return response;
        }

        public static JsonResult NotFound(this JsonResult response, string errorCode = "", string errorMessage = "")
        {
            response.StatusCode = (int) HttpStatusCode.NotFound;
            
            response.Value = new {
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage 
            };

            return response;
        }

        public static JsonResult InvalidModelState(this JsonResult response, ModelStateDictionary modelState, string errorCode = "", string errorMessage = "")
        {
            response.StatusCode = (int) HttpStatusCode.OK;
            response.Value = new {
                Success = false,
                ErrorCode = "",
                ErrorMessage = errorMessage,
                ModelErrors = modelState.AllErrors().ToList()
            };
            return response;
        }
    }
}