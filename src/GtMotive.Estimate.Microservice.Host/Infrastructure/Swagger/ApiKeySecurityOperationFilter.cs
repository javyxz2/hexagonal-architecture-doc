using System;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace GtMotive.Estimate.Microservice.Host.Infrastructure.Swagger
{
    internal sealed class ApiKeySecurityOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            ArgumentNullException.ThrowIfNull(operation);
            ArgumentNullException.ThrowIfNull(context);

            var controllerAttributes = context.MethodInfo.DeclaringType is null
                ? []
                : context.MethodInfo.DeclaringType
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .ToArray();

            var methodAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .ToArray();

            if (controllerAttributes.Union(methodAttributes).Any())
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });

                operation.Security =
                [
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "ApiKey"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }

                ];
            }
        }
    }
}
