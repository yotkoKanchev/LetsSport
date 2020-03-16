namespace LetsSport.Services.Mapping
{
    using System;

    public static class ObjectMappingExtensions
    {
        public static TDestination To<TDestination>(this object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return AutoMapperConfig.MapperInstance.Map<TDestination>(source);
        }

        public static TDestination To<TSource, TDestination>(this TSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return AutoMapperConfig.MapperInstance.Map<TSource, TDestination>(source);
        }

        public static TDestination From<TSource, TDestination>(TSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return AutoMapperConfig.MapperInstance.Map<TSource, TDestination>(source);
        }
    }
}
