using System.ComponentModel.DataAnnotations;
using System.Reflection;
using GClaims.Core.Extensions;
using GClaims.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GClaims.Core.FIlters;

public class ModelStateValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        if (context.ActionDescriptor is not ControllerActionDescriptor descriptor)
        {
            return;
        }

        var parameters = descriptor.MethodInfo.GetParameters();

        foreach (var parameter in parameters)
        {
            if (!context.ActionArguments.ContainsKey(parameter.Name))
            {
                continue;
            }

            var argument = context.ActionArguments[parameter.Name];

            if (TypeHelper.IsNullable(parameter.ParameterType))
            {
                context.ModelState.MarkFieldValid(parameter.Name);
            }
            else if (TypeHelper.IsEnumerable(parameter.ParameterType, out var itemType))
            {
                if (itemType.IsNotNull())
                {
                    ValidateAttributes(parameter, argument, context.ModelState);
                }
                else
                {
                    context.ModelState.ClearValidationState(parameter.Name);
                    context.ModelState.MarkFieldValid(parameter.Name);
                }
            }
            else if (context.ModelState.GetValidationState(parameter.Name) == ModelValidationState.Invalid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
            else if (context.ModelState.GetValidationState(parameter.Name) == ModelValidationState.Valid)
            {
            }
            else
            {
                if (context.ModelState.All(state => state.Value.ValidationState != ModelValidationState.Invalid))
                {
                    continue;
                }

                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }
        }
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private void ValidateAttributes(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
    {
        var validationAttributes = parameter.CustomAttributes;

        foreach (var attributeData in validationAttributes)
        {
            var attributeInstance = parameter.GetCustomAttribute(attributeData.AttributeType);

            if (attributeInstance is not ValidationAttribute validationAttribute)
            {
                continue;
            }

            var isValid = validationAttribute.IsValid(argument);
            if (isValid)
            {
                continue;
            }

            if (parameter.Name != null)
            {
                modelState.AddModelError(parameter.Name, validationAttribute.FormatErrorMessage(parameter.Name));
            }
        }
    }
}