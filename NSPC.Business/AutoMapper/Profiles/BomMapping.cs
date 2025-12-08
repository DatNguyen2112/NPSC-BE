using AutoMapper;
using NSPC.Business.Services.Bom;
using NSPC.Data.Data.Entity.Bom;
using NSPC.Data.Data.Entity.VatTu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.AutoMapper.Profiles
{
    public class BomMapping : Profile
    {
        public BomMapping()
        {
            // Entity to ViewModel mapping configuration
            CreateMap<sm_Bom, BomViewModel>();
            CreateMap<sm_Materials, MaterialViewModel>();
            CreateMap<sm_Product, ProductViewModelInBom>();

            // ViewModel to Entity mapping configuration
            CreateMap<BomCreateUpdateModel, sm_Bom>();
            CreateMap<MaterialCreateUpdateModel, sm_Materials>();
        }
    }
}
