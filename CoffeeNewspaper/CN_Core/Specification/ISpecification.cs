using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CN_Core.Specification
{
    // https://github.com/dotnet-architecture/eShopOnWeb
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
    }
}