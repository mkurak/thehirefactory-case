using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TheHireFactory.ECommerce.Api.Swagger;

public sealed class DefaultResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var pdSchema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository);
        var vpdSchema = context.SchemaGenerator.GenerateSchema(typeof(ValidationProblemDetails), context.SchemaRepository);

        OpenApiMediaType ProblemMedia(string title, int status, string detail, string instance) =>
            new()
            {
                Schema = pdSchema,
                Examples =
                {
                    ["sample"] = new OpenApiExample
                    {
                        Value = new OpenApiObject
                        {
                            ["type"]     = new OpenApiString("https://tools.ietf.org/html/rfc9110#section-15.5"),
                            ["title"]    = new OpenApiString(title),
                            ["status"]   = new OpenApiInteger(status),
                            ["detail"]   = new OpenApiString(detail),
                            ["instance"] = new OpenApiString(instance)
                        }
                    }
                }
            };

        var badRequestMedia = new OpenApiMediaType
        {
            Schema = vpdSchema,
            Examples =
            {
                ["sample"] = new OpenApiExample
                {
                    Value = new OpenApiObject
                    {
                        ["type"]     = new OpenApiString("https://tools.ietf.org/html/rfc9110#section-15.5.1"),
                        ["title"]    = new OpenApiString("One or more validation errors occurred."),
                        ["status"]   = new OpenApiInteger(400),
                        ["detail"]   = new OpenApiString("The request was invalid."),
                        ["instance"] = new OpenApiString("/api/product"),
                        ["errors"]   = new OpenApiObject
                        {
                            ["name"]       = new OpenApiArray { new OpenApiString("Name is required.") },
                            ["price"]      = new OpenApiArray { new OpenApiString("Price must be greater than 0.") },
                            ["categoryId"] = new OpenApiArray { new OpenApiString("Category not found.") },
                        }
                    }
                }
            }
        };

        void AddResponse(string code, string description, OpenApiMediaType media)
        {
            if (!operation.Responses.ContainsKey(code))
            {
                operation.Responses[code] = new OpenApiResponse
                {
                    Description = description,
                    Content = { ["application/problem+json"] = media }
                };
            }
        }

        AddResponse("400", "Bad Request", badRequestMedia);
        AddResponse("404", "Not Found",
            ProblemMedia("Not Found", 404, "The requested resource was not found.", "/api/product/{id}"));
        AddResponse("500", "Server Error",
            ProblemMedia("An unexpected error occurred.", 500, "Unexpected error.", "/api/product"));
    }
}