using System.Collections.Generic;
using AutoMapper;
using Esquire.Models;
using Newtonsoft.Json;

namespace Esquire.Resolvers
{
    public class AnnotationTypeToStrConverter : ITypeConverter<List<Annotation>, string>
    {
        public string Convert(List<Annotation> source, string destination, ResolutionContext context)
        {
            var annotations = JsonConvert.SerializeObject(source);
 
     return annotations;
        }
    }
}

