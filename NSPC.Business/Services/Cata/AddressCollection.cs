using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;

namespace NSPC.Business.Services.Cata
{
    public class AddressCollection
    {
        public List<ProvinceListViewModel> AllProvince;
        public List<CommuneListViewModel> AllCommune;
        public List<DistrictListViewModel> AllDistrict;
        public static AddressCollection Instance { get; } = new AddressCollection();

        private MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddCollectionMappers();
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
            cfg.CreateMap<cata_Province, ProvinceListViewModel>();
            cfg.CreateMap<cata_Commune, CommuneListViewModel>();
            cfg.CreateMap<cata_District, DistrictListViewModel>();
        });

        IMapper mapper;


         protected AddressCollection()
        {
            mapper = new Mapper(config);
            if (AllProvince == null || AllProvince.Count() == 0 || AllCommune == null || AllCommune.Count() == 0 ||
                AllDistrict == null || AllDistrict.Count() == 0)
            {
                LoadData();
            }
        }
        
        public void LoadData()
        {
            using (var _dataContext = new SMDbContext())
            {
                AllProvince = new List<ProvinceListViewModel>();
                AllCommune = new List<CommuneListViewModel>();
                AllDistrict = new List<DistrictListViewModel>();

                var resultProvince = _dataContext.cata_Province.ToList();
                var resultCommunee = _dataContext.cata_Commune.ToList();
                var resultDistricte = _dataContext.cata_District.ToList();

                AllProvince = mapper.Map<List<cata_Province>, List<ProvinceListViewModel>>(resultProvince);
                AllCommune = mapper.Map<List<cata_Commune>, List<CommuneListViewModel>>(resultCommunee);
                AllDistrict = mapper.Map<List<cata_District>, List<DistrictListViewModel>>(resultDistricte);
            }
        }

        public ProvinceListViewModel FetchProvince(int provinceCode)
        {
            if (provinceCode == 0)
                return null;
            var result = AllProvince.Where(x => x.ProvinceCode == provinceCode).FirstOrDefault();

            return result;
        }

        public List<ProvinceListViewModel> FetchProvinceListCode(List<int> provinceListCode)
        {
            var result = new List<ProvinceListViewModel>();
            if (provinceListCode == null) 
                return null;

            foreach (var code in provinceListCode)
            {
                var model = AllProvince.Where(x => x.ProvinceCode == code).FirstOrDefault();

                if (model != null)
                    result.Add(model);
            }

            return result;
        }

        public CommuneListViewModel FetchCommune(int communeCode)
        {
            if (communeCode == 0)
                return null;
            var result = AllCommune.Where(x => x.CommuneCode == communeCode).FirstOrDefault();

            return result;
        }

        public List<CommuneListViewModel> FetchCommuneListCode(List<int> communeListCode)
        {
            var result = new List<CommuneListViewModel>();
            if (communeListCode == null)
                return null;

            foreach (var code in communeListCode)
            {
                var model = AllCommune.Where(x => x.CommuneCode == code).FirstOrDefault();

                if (model != null)
                    result.Add(model);
            }

            return result;
        }

        public DistrictListViewModel FetchDistrict(int districtCode)
        {
            if (districtCode == 0)
                return null;
            var result = AllDistrict.Where(x => x.DistrictCode == districtCode).FirstOrDefault();

            return result;
        }

        public List<DistrictListViewModel> FetchDistrictListCode(List<int> districtListCode)
        {
            var result = new List<DistrictListViewModel>();
            if (districtListCode == null)
                return null;

            foreach (var code in districtListCode)
            {
                var model = AllDistrict.Where(x => x.DistrictCode == code).FirstOrDefault();

                if (model != null)
                    result.Add(model);
            }

            return result;
        }
    }
}
