using NSPC.Common;

namespace NSPC.Business;

public interface IMeasureUnitHandler
{
    Task<Response<MeasureUnitViewModel>> Create(MeasureUnitCreateUpdateModel model);
    Task<Response<MeasureUnitViewModel>> Update(Guid id, MeasureUnitCreateUpdateModel model);
    Task<Response<Pagination<MeasureUnitViewModel>>> GetPage(MeasureUnitQueryModel query);
    Task<Response<MeasureUnitViewModel>> GetById(Guid id);
    Task<Response> Delete(Guid id);
}