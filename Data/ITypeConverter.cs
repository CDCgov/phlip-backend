using System.Collections.Generic;
using AutoMapper;

namespace Esquire.Data
{
    public interface ITypeConverter<in TSource, TDestination>
    {
        List<TDestination> Convert(TSource source, List<TDestination> destination, ResolutionContext context);
    }
}