using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Microsoft.EntityFrameworkCore;
using NSPC.Data;
using NSPC.Data.Data;

namespace NSPC.Business
{
    public class RightCollection
    {
        private readonly IMapper _mapper;
        public HashSet<RightViewModel> Collection;
        public static RightCollection Instance { get; } = new RightCollection();

        private MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddCollectionMappers();
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
            cfg.CreateMap<IdmRole, RoleModel>();
        });

        protected RightCollection()
        {
            _mapper = new Mapper(config);
            LoadToHashSet();
        }

        public void LoadToHashSet()
        {
            Collection = new HashSet<RightViewModel>();

            using (var _dbContext = new SMDbContext())
            {
                var rights = _dbContext.IdmRight.AsNoTracking().ToList();
                var mappedRights = _mapper.Map<List<RightViewModel>>(rights);
                foreach (var right in mappedRights)
                {
                    Collection.Add(right);
                }
            }
        }

        public RightViewModel GetById(Guid id)
        {
            var result = Collection.FirstOrDefault(x => x.Id == id);
            return result;
        }
    }
}

