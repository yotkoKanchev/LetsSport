namespace LetsSport.Services.Mapping
{
    using System;

    public static class ObjectMappingExtensions
    {
        public static TDest To<TDest>(this object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return AutoMapperConfig.MapperInstance.Map<TDest>(source);
        }

        public static TDest To<TSource, TDest>(this TSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return AutoMapperConfig.MapperInstance.Map<TSource, TDest>(source);
        }

        public static TDest From<TSource, TDest>(TSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return AutoMapperConfig.MapperInstance.Map<TSource, TDest>(source);
        }
    }
}
