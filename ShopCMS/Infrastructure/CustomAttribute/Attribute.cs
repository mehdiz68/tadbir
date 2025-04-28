using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace ahmadi.Infrastructure.CustomAttribute
{
    public class MyRemoteAttribute : RemoteAttribute
    {
        public MyRemoteAttribute(string routeName)
            : base(routeName)
        {
        }

        public MyRemoteAttribute(string action, string controller)
            : base(action, controller)
        {
            this.RouteData["area"] = "";
        }

        public MyRemoteAttribute(string action, string controller,
            string areaName)
            : base(action, controller, areaName)
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            // Get the controller using reflection  
            Type controller = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(type => type.Name.ToLower() == string.Format("{0}Controller",
                    this.RouteData["controller"].ToString()).ToLower());
            if (controller != null)
            {
                // Get the action method that has validation logic  
                MethodInfo action = controller.GetMethods()
                    .FirstOrDefault(method => method.Name.ToLower() ==
                        this.RouteData["action"].ToString().ToLower());
                if (action != null)
                {
                    // Create an instance of the controller class  
                    object instance = Activator.CreateInstance(controller);
                    // Invoke the action method that has validation logic  
                    object response = action.Invoke(instance, new object[] { value });
                    if (response is JsonResult)
                    {
                        object jsonData = ((JsonResult)response).Data;
                        if (jsonData is bool)
                        {
                            return (bool)jsonData ? ValidationResult.Success :
                                new ValidationResult(this.ErrorMessage);
                        }
                    }
                }
            }
            return new ValidationResult(base.ErrorMessageString);
        }
    }
}
