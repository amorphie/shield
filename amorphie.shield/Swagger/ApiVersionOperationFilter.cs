using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace amorphie.shield.Swagger;

public class ApiVersionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        operation.Parameters ??= new List<OpenApiParameter>();

        var apiVersionMetadata = actionMetadata
            .Any(metadataItem => metadataItem is ApiVersionMetadata);
        if (apiVersionMetadata)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Api-Version",
                In = ParameterLocation.Header,
                Description = "API Version header value",
                Schema = new OpenApiSchema
                {
                    Type = "String",
                    Default = new OpenApiString("1.0")
                }
            });
        }
    }
}
