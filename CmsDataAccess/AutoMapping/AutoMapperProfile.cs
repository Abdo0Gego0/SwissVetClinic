using AutoMapper;
using CmsDataAccess.DbModels;
using CmsDataAccess.ModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.AutoMapping
{
    public static class MappingToDto
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg => {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<AutoMapperProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MedicalCenter, MedicalCenterDto>();
            CreateMap<MedicalCenterDto, MedicalCenter>();
            CreateMap<BaseClinic, BaseClinicDto>();

            
        }
    }
}
