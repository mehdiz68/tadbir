using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CoreLib.Infrastructure.ModelBinder
{
    public class CorrectArabianLetter : FilterAttribute, IActionFilter
    {
        private readonly string[] _parameterName;

        public CorrectArabianLetter(string[] parameterName)
        {
            _parameterName = parameterName;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            for (int i = 0; i < _parameterName.Length; i++)
            {
                var valueProviderResult = filterContext.Controller.ValueProvider.GetValue(_parameterName[i]);

                if (valueProviderResult != null)
                {
                    var finalstring = CommonFunctions.CorrectArabianLetter(valueProviderResult.AttemptedValue.ToString());
                    filterContext.ActionParameters[_parameterName[i]] = finalstring;
                }
            }
           
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
