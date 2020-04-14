//using System.Collections.Generic;
//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Microsoft.OpenApi.Models;

//namespace esquire_backend.Security
//{
//    public class SecurityRequirementsDocumentFilter : IDocumentFilter
//    {
//        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
//        {
//            swaggerDoc.Security = new List<IDictionary<string, IEnumerable<string>>>
//            {
//                new Dictionary<string, IEnumerable<string>>
//                {
//                    { "Bearer", new string[]{ } }
//                }
//            };
//        }
//    }
//}