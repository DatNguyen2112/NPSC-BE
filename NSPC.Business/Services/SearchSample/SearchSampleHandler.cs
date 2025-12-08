using AutoMapper;
using FileManagement.Data;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//using NSPC.Business.Services.QuanLyKho;
using NSPC.Common;
using NSPC.Data.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class SearchSampleHandler : ISearchSampleHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public SearchSampleHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<SearchSampleModel>> Create(SearchSampleCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.fm_Search_Sample.Any(x => x.Title == model.Title))
                    return Helper.CreateBadRequestResponse<SearchSampleModel>(string.Format("Tên {0} đã tồn tại", model.Title));
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var searchSample = new fm_Search_Sample
                {
                    Title = model.Title,
                    QueryJsonString = model.QueryJsonString,
                    CreatedByUserName = currentUser.UserName,
                    CreatedByUserId = currentUser.UserId,
                    CreatedOnDate = DateTime.Now,
                };

                _dbContext.Add(searchSample);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<SearchSampleModel>(searchSample), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SearchSampleModel>(ex);
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var searchSample = await _dbContext.fm_Search_Sample.FirstOrDefaultAsync(x => x.Id == id);
                if (searchSample == null)
                    return Helper.CreateNotFoundResponse("Không tìm thấy bản ghi");

                _dbContext.Remove(searchSample);

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SearchSampleModel>(ex);
            }
        }

        public async Task<Response<SearchSampleModel>> GetById(Guid id)
        {
            try
            {

                var searchSample = await _dbContext.fm_Search_Sample.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (searchSample == null)
                    return Helper.CreateNotFoundResponse<SearchSampleModel>("Không tìm thấy bản ghi");

                var result = _mapper.Map<SearchSampleModel>(searchSample);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SearchSampleModel>(ex);
            }
        }

        public async Task<Response<Pagination<SearchSampleModel>>> GetPage(SearchSampleQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var data = await _dbContext.fm_Search_Sample.AsNoTracking().Where(predicate).GetPageAsync(query);

                var result = _mapper.Map<Pagination<SearchSampleModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<SearchSampleModel>>(ex);
            }
        }

        public async Task<Response<SearchSampleModel>> Update(Guid id, SearchSampleCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.fm_Search_Sample.Any(x => x.Title == model.Title && x.Id != id))
                    return Helper.CreateBadRequestResponse<SearchSampleModel>(string.Format("Tên {0} đã tồn tại", model.Title));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var searchSample = await _dbContext.fm_Search_Sample.FirstOrDefaultAsync(x => x.Id == id);
                if (searchSample == null)
                    return Helper.CreateNotFoundResponse<SearchSampleModel>("Không tìm thấy bản ghi");

                searchSample.Title = model.Title;
                searchSample.QueryJsonString = model.QueryJsonString;
                searchSample.LastModifiedByUserId = currentUser.UserId;
                searchSample.LastModifiedByUserName = currentUser.UserName;
                searchSample.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<SearchSampleModel>(searchSample), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<SearchSampleModel>(ex);
            }
        }

        private Expression<Func<fm_Search_Sample, bool>> BuildQuery(SearchSampleQueryModel query)
        {
            var predicate = PredicateBuilder.New<fm_Search_Sample>(true);

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(x => x.Title.ToLower().Contains(query.FullTextSearch.ToLower()));

            return predicate;
        }
    }
}
