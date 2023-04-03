using Microsoft.OpenApi.Models;
using PayOut_Aulac_FPT.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PayOut_Aulac_FPT.Swagger.Operation
{
    public class RequiredOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //check have atrribute RequiredCustom then add required
            foreach (var parameter in operation.Parameters)
            {
                //get parameterinfos
                var parameterInfos = context.MethodInfo.GetParameters();
                foreach (var item in parameterInfos)
                {
                    //get property have name is parameter.Name
                    var property = item.ParameterType.GetProperty(parameter.Name);
                    if (property != null)
                    {
                        //check have attribute RequiredCustom
                        var attribute = property.GetCustomAttributes(false).OfType<RequiredCustomAttribute>().FirstOrDefault();
                        if (attribute != null)
                        {
                            //add required
                            parameter.Required = true;
                        }
                    }
                }

            }

            var requestBody = operation.RequestBody;
            if (requestBody != null)
            {
                foreach (var item in requestBody.Content)
                {
                    var schema = item.Value.Schema;
                    if (schema != null)
                    {
                        //get parameter from context
                        var parameters = context.ApiDescription.ParameterDescriptions;
                        //find parameter have attribute RequiredCustom
                        foreach (var parameter in parameters)
                        {
                            //check have attribute RequiredCustom
                            var attribute = parameter.CustomAttributes().OfType<RequiredCustomAttribute>().FirstOrDefault();
                            if (attribute != null)
                            {
                                //add required
                                schema.Required.Add(parameter.Name);
                            }
                        }
                    }
                }
            }

        }
    }
}
