using AutoMapper;
using System;
using System.Linq;
using System.Reflection;


namespace CleanArchitectureTemplate.Domain.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
            // converts
            //CreateMap<BlogPostTag, string>().ConvertUsing(bpt => bpt.Tag.Name);
            //CreateMap<Author, string>().ConvertUsing(bpt => bpt.User.Name);
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var methodInfo = type.GetMethod("Mapping")
                    ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                methodInfo?.Invoke(instance, new object[] { this });

            }
        }
    }
}
