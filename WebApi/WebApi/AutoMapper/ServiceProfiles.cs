using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dto;
using WebApi.Model;

namespace WebApi.AutoMapper
{
    public class ServiceProfiles: Profile
    {
        public ServiceProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<Role, RoleDto>();
            CreateMap<Permission, PermissionDto>();
        }
    }
}
