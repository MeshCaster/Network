// MeshNetwork.Application/Mappings/MappingProfile.cs
using AutoMapper;
using Core.Application.DTOs.Domain;
using Core.Domain.Entities;
using NetTopologySuite.Geometries;

namespace Core.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Node mappings
        CreateMap<Node, NodeDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new LocationDto
            {
                Latitude = src.Location.Y,
                Longitude = src.Location.X,
                Altitude = null
            }))
            .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.IsOnline()))
            .ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.Username : null));
        
        CreateMap<LocationDto, Point>()
            .ConvertUsing(src => NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326).CreatePoint(new Coordinate(src.Longitude, src.Latitude)));
        
        // NodeConnection mappings
        CreateMap<NodeConnection, NodeConnectionDto>()
            .ForMember(dest => dest.SourceNodeName, opt => opt.MapFrom(src => src.SourceNode.Name))
            .ForMember(dest => dest.TargetNodeName, opt => opt.MapFrom(src => src.TargetNode.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsHealthy, opt => opt.MapFrom(src => src.IsHealthy()));
        
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.NodeCount, opt => opt.MapFrom(src => src.Nodes.Count));
        
        // Route mappings
        CreateMap<Route, RouteDto>()
            .ForMember(dest => dest.Preference, opt => opt.MapFrom(src => src.Preference.ToString()))
            .ForMember(dest => dest.PathNodeNames, opt => opt.Ignore()); // Populated manually
        
        // NetworkMetric mappings
        CreateMap<NetworkMetric, NetworkMetricDto>();
        
        // NetworkHealth mappings
        CreateMap<NetworkHealth, NetworkHealthDto>()
            .ForMember(dest => dest.HealthStatus, opt => opt.Ignore()); // Calculated in handler
        
        // RelayTransaction mappings
        CreateMap<RelayTransaction, RelayTransactionDto>()
            .ForMember(dest => dest.NodeName, opt => opt.MapFrom(src => src.Node.Name));
    }
}