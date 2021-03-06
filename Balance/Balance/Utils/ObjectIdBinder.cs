﻿using System;
using System.Web.Mvc;
using MongoDB.Bson;

namespace Balance.Utils
{
    public class ObjectIdBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };

            object actualValue = null;
            try
            {
                actualValue = ObjectId.Parse(valueResult.AttemptedValue);
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return actualValue;
        }
    }
}
