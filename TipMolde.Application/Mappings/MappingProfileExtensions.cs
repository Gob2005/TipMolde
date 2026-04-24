using System.Linq.Expressions;
using AutoMapper;

namespace TipMolde.Application.Mappings
{
    internal static class MappingProfileExtensions
    {
        public static IMappingExpression<TSource, TDestination> MapTrimmedRequired<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, string?>> destinationMember,
            Func<TSource, string> sourceGetter)
        {
            return mapping.ForMember(destinationMember, opt => opt.MapFrom(src => sourceGetter(src).Trim()));
        }

        public static IMappingExpression<TSource, TDestination> MapTrimmedOptional<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, string?>> destinationMember,
            Func<TSource, string?> sourceGetter)
        {
            return mapping.ForMember(destinationMember, opt => opt.MapFrom(src => NormalizeOptionalString(sourceGetter(src))));
        }

        public static IMappingExpression<TSource, TDestination> MapTrimmedIfProvided<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, string?>> destinationMember,
            Func<TSource, string?> sourceGetter)
        {
            return mapping.ForMember(destinationMember, opt =>
            {
                opt.Condition(src => !string.IsNullOrWhiteSpace(sourceGetter(src)));
                opt.MapFrom(src => sourceGetter(src)!.Trim());
            });
        }

        public static IMappingExpression<TSource, TDestination> MapValueIfProvided<TSource, TDestination, TValue>(
            this IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, TValue>> destinationMember,
            Func<TSource, TValue?> sourceGetter)
            where TValue : struct
        {
            return mapping.ForMember(destinationMember, opt =>
            {
                opt.Condition(src => sourceGetter(src).HasValue);
                opt.MapFrom(src => sourceGetter(src).GetValueOrDefault());
            });
        }

        public static IMappingExpression<TSource, TDestination> MapValueIfProvided<TSource, TDestination, TValue>(
            this IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, TValue?>> destinationMember,
            Func<TSource, TValue?> sourceGetter)
            where TValue : struct
        {
            return mapping.ForMember(destinationMember, opt =>
            {
                opt.Condition(src => sourceGetter(src).HasValue);
                opt.MapFrom(src => sourceGetter(src));
            });
        }

        public static string? NormalizeOptionalString(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public static TProperty? GetOptionalValue<TSource, TProperty>(
            TSource? source,
            Func<TSource, TProperty?> selector)
            where TSource : class
        {
            return source is null ? default : selector(source);
        }
    }
}
