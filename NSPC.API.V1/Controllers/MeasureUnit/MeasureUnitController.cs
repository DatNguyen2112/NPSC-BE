using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Common;

namespace NSPC.API.V1.Controllers.MeasureUnit;

/// <summary>
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{api-version:apiVersion}/measure-unit")]
[ApiExplorerSettings(GroupName = "Quản lý đơn vị tính")]
[Authorize]
public class MeasureUnitController : ControllerBase
{
    private readonly IMeasureUnitHandler _measureUnitHandler;

    /// <summary>
    /// </summary>
    /// <param name="measureUnitHandler"></param>
    public MeasureUnitController(IMeasureUnitHandler measureUnitHandler)
    {
        _measureUnitHandler = measureUnitHandler;
    }

    /// <summary>
    /// Tạo đơn vị tính
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Response<MeasureUnitViewModel>>> Create(
        [FromBody] MeasureUnitCreateUpdateModel model)
    {
        var result = await _measureUnitHandler.Create(model);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Cập nhật đơn vị tính
    /// </summary>
    /// <param name="model"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut, Route("{id:guid}")]
    public async Task<ActionResult<Response<MeasureUnitViewModel>>> Update(Guid id,
        [FromBody] MeasureUnitCreateUpdateModel model)
    {
        var result = await _measureUnitHandler.Update(id, model);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Lấy danh sách đơn vị tính
    /// </summary>
    /// <param name="size"></param>
    /// <param name="page"></param>
    /// <param name="filter"></param>
    /// <param name="sort"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Response<Pagination<MeasureUnitViewModel>>>> GetPage(
        [FromQuery] int size = 20,
        [FromQuery] int page = 1,
        [FromQuery] string filter = "{}",
        [FromQuery] string sort = ""
    )
    {
        var filterObject = JsonConvert.DeserializeObject<MeasureUnitQueryModel>(filter);

        sort = string.IsNullOrEmpty(sort) ? "-CreatedOnDate" : sort;
        filterObject.Sort = sort != null ? sort : filterObject.Sort;
        filterObject.Size = size;
        filterObject.Page = page;

        var result = await _measureUnitHandler.GetPage(filterObject);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Lấy chi tiết 1 đơn vị tính
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("{id:guid}")]
    public async Task<ActionResult<Response<MeasureUnitViewModel>>> GetById(Guid id)
    {
        var result = await _measureUnitHandler.GetById(id);
        return Helper.TransformData(result);
    }

    /// <summary>
    /// Xóa 1 đơn vị tính
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete, Route("{id:guid}")]
    public async Task<ActionResult<Response>> Delete(Guid id)
    {
        var result = await _measureUnitHandler.Delete(id);
        return Helper.TransformData(result);
    }
}