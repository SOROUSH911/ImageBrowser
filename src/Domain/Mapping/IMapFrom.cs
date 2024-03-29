using AutoMapper;

namespace CleanArchitectureTemplate.Domain.Mapping
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
