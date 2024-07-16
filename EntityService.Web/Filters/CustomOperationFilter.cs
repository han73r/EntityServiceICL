using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using EntityService.Application.Models;
using Microsoft.OpenApi.Any;

namespace EntityService.Web.Filters {
    public class CustomOperationFilter : IOperationFilter {
        public void Apply(OpenApiOperation operation, OperationFilterContext context) {
            if (context.ApiDescription.HttpMethod == "POST" && context.ApiDescription.RelativePath == "/api/Entity/insert") {
                operation.Description = "Adding a new entity to the system";
                operation.RequestBody = new OpenApiRequestBody {
                    Content = new Dictionary<string, OpenApiMediaType> {
                        ["application/json"] = new OpenApiMediaType {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(EntityDto), context.SchemaRepository)
                        }
                    }
                };
                operation.Responses.Add("200", new OpenApiResponse { Description = "Entity saved successfully" });
                operation.Responses.Add("400", new OpenApiResponse { Description = "Invalid entity data" });
                operation.Responses.Add("500", new OpenApiResponse { Description = "Internal server error" });
            } else if (context.ApiDescription.HttpMethod == "GET" && context.ApiDescription.RelativePath == "/api/Entity/get/{id}") {
                operation.Description = "Retrieve entity by Id";
                operation.Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "id",
                        In = ParameterLocation.Path,
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uuid",
                            Example = new OpenApiString("cfaa0d3f-7fea-4423-9f69-ebff826e2f89")
                        }
                    }
                };
                operation.Responses.Add("200", new OpenApiResponse { Description = "Entity retrieved successfully" });
                operation.Responses.Add("400", new OpenApiResponse { Description = "Invalid Id format" });
                operation.Responses.Add("404", new OpenApiResponse { Description = "Entity not found" });
                operation.Responses.Add("500", new OpenApiResponse { Description = "Internal server error" });
            }
        }
    }
}