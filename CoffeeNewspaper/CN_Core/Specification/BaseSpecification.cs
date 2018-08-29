using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CN_Core.Specification
{
    // https://github.com/dotnet-architecture/eShopOnWeb
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        // string-based includes allow for including children of children, e.g. Basket.Items.Product
        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
    }
}