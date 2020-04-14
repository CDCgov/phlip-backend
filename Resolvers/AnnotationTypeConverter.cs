using System;
using System.Collections.Generic;
using AutoMapper;
using Esquire.Models;
using Newtonsoft.Json;

namespace Esquire.Resolvers
{
    public class AnnotationTypeConverter : ITypeConverter<string, List<Annotation>>
    {
        public List<Annotation> Convert(string source, List<Annotation> destination, ResolutionContext context)
        {
            List<Annotation> annotations = null;
            try
            {
                if (!string.IsNullOrEmpty(source))
                {
                    var cleansedSource = source.Replace("'", "\\'");
                    Console.WriteLine(cleansedSource);
                    annotations = JsonConvert.DeserializeObject<List<Annotation>>(cleansedSource);
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }         
     return annotations;
        }
    }
}

