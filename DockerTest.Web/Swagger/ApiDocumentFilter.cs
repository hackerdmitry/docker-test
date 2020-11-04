using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DockerTest.Web.Swagger
{
    public class ApiDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // foreach (var description in context.ApiDescriptions)
            // {
            //     var parameters = description.ParameterDescriptions
            //        .Where(x => x.ModelMetadata is DefaultModelMetadata)
            //        .ToArray();
            //
            //     foreach (var parameterDescription in parameters)
            //     {
            //         var remove = ((DefaultModelMetadata) parameterDescription.ModelMetadata).Attributes.Attributes
            //            .Any(x => x is JsonIgnoreAttribute);
            //
            //         if (remove)
            //         {
            //             var path = swaggerDoc.Paths["/" + description.RelativePath];
            //             if (path.Operations.TryGetValue(OperationType.Get, out var operation))
            //             {
            //                 var parameter = operation.Parameters
            //                   ?.FirstOrDefault(x => x.Name.Equals(parameterDescription.Name));
            //                 if (parameter != null)
            //                 {
            //                     operation.Parameters.Remove(parameter);
            //                 }
            //             }
            //         }
            //     }
            // }
        }
    }
}