namespace LetsSport.Services.Mapping
{
    using System;

    public static class ObjectMappingExtensions
    {
        public static TDest To<TSource, TDest>(this TSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return AutoMapperConfig.MapperInstance.Map<TSource, TDest>(source);
        }
    }
}
