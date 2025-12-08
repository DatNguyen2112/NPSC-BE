using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenData.Common.Constants;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.PhongBan;
using NSPC.Data.Data.Entity.WorkingDay;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.WorkingDay
{
    public class WorkingDayHandler : IWorkingDayHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public WorkingDayHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response> BootstrapWorkingDays(int year)
        {
            try
            {
                if (year - DateTime.Now.Year < 0)
                    return new ResponseError(HttpStatusCode.BadRequest, "Can't generate working days for previous years than present");
                if (_dbContext.mk_WorkingDay.Any(x => x.Year == year))
                    return new ResponseError(HttpStatusCode.BadRequest, string.Format("Working days for {0} is already created", year));
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var lunardate = new ChineseLunisolarCalendar();
                var time = new DateTime(year, 1, 1);
                int numberOfMonth;
                if (lunardate.IsLeapYear(time.Year) == true)
                    numberOfMonth = 13;
                else numberOfMonth = 12;

                int numberOfMonthPreviousYear;
                if (lunardate.IsLeapYear(lunardate.GetYear(time)) == true)
                    numberOfMonthPreviousYear = 13;
                else numberOfMonthPreviousYear = 12;

                bool isBeforeMarch = false;
                if (lunardate.IsLeapMonth(time.Year, 1) || lunardate.IsLeapMonth(time.Year, 2) || lunardate.IsLeapMonth(time.Year, 3))
                    isBeforeMarch = true;

                while (time < (new DateTime(year, 1, 1)).AddYears(1))
                {
                    var result = new mk_WorkingDay();
                    result.Id = new Guid();
                    result.Day = time.Day;
                    result.Month = time.Month;
                    result.Year = year;
                    if ((time.Day == 1 && time.Month == 1))
                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Tết Dương lịch";
                    }
                    else if ((time.Day == 30 && time.Month == 4))
                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Giải phóng miền Nam";
                    }
                    else if ((time.Day == 1 && time.Month == 5))
                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Quốc tế Lao Động";
                    }
                    else if ((time.Day == 2 && time.Month == 9))
                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Quốc Khánh";
                    }
                    else if ((lunardate.GetDayOfMonth(time) == 10 && lunardate.GetMonth(time) == 3 && numberOfMonth == 12))
                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Giỗ tổ Hùng Vương";
                    }
                    else if ((lunardate.GetDayOfMonth(time) == 10 && lunardate.GetMonth(time) == 3 && numberOfMonth == 13 && !isBeforeMarch))
                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Giỗ tổ Hùng Vương";
                    }
                    else if ((lunardate.GetDayOfMonth(time) == 10 && lunardate.GetMonth(time) == 4 && numberOfMonth == 13 && isBeforeMarch))
                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Giỗ tổ Hùng Vương";
                    }
                    else if ((lunardate.GetDayOfMonth(time) == 1 || lunardate.GetDayOfMonth(time) == 2
                            || lunardate.GetDayOfMonth(time) == 3 || lunardate.GetDayOfMonth(time) == 4
                            || lunardate.GetDayOfMonth(time) == 5) && lunardate.GetMonth(time) == 1)

                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Tết Nguyên Đán";
                    }
                    else if ((lunardate.GetDayOfMonth(time) == 29 && lunardate.GetMonth(time) == numberOfMonthPreviousYear)
                        || (lunardate.GetDayOfMonth(time) == 30 && lunardate.GetMonth(time) == numberOfMonthPreviousYear))
                    {
                        result.Type = TypeOfDayConstant.HOLIDAY;
                        result.OriginalType = TypeOfDayConstant.HOLIDAY;
                        result.Note = "Tết Nguyên Đán";
                    }
                    else if (time.DayOfWeek == DayOfWeek.Sunday || time.DayOfWeek == DayOfWeek.Saturday)
                    {
                        result.Type = TypeOfDayConstant.DAYOFF;
                        result.OriginalType = TypeOfDayConstant.DAYOFF;
                    }
                    else
                    {
                        result.Type = TypeOfDayConstant.WORKING;
                        result.OriginalType = TypeOfDayConstant.WORKING;
                    }
                    result.CreatedByUserId = currentUser.UserId;
                    result.CreatedByUserName = currentUser.FullName;
                    _dbContext.mk_WorkingDay.Add(result);
                    time = time.AddDays(1);
                }

                await _dbContext.SaveChangesAsync();

                return new Response(HttpStatusCode.OK, "Thành công!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: {@param}");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response<List<TotalWorkingDayViewModel>>> GetTotalWorkingDay(int year)
        {
            try
            {
                var result = new List<TotalWorkingDayViewModel>();
                for (int i = 1; i <= 12; i++ )
                {
                    var totalWorkingDayOnMonth = await GetWorkingDays(year, i, null);
                    var item = new TotalWorkingDayViewModel
                    {
                        id = Guid.NewGuid(),
                        Month = i,
                        Year = year,
                        TotalWorkingDay = totalWorkingDayOnMonth.Data.Count(x => x.Type == TypeOfDayConstant.WORKING)
                    };
                    result.Add(item);
                }
                return new Response<List<TotalWorkingDayViewModel>>(result);
            }
            catch(Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: {@param}");
                return Helper.CreateExceptionResponse<List<TotalWorkingDayViewModel>>(ex);
            }
        }

        public async Task<Response<List<WorkingDayViewModel>>> GetWorkingDays(int year, int? month, int? day)
        {
            try
            {
                var result = new List<WorkingDayViewModel>();

                var query = _dbContext.mk_WorkingDay
                    .Where(t => t.Year == year);

                if (month.HasValue)
                    query = query.Where(x => x.Month == month.Value);

                if (day.HasValue)
                    query = query.Where(x => x.Day == day.Value);

                query = query.OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day);

                var days = await query.ToListAsync();

                var lunardate = new ChineseLunisolarCalendar();

                var daysOfWeekMapping = new Dictionary<DayOfWeek, string>
                    {
                        { DayOfWeek.Monday, "T2" },
                        { DayOfWeek.Tuesday, "T3" },
                        { DayOfWeek.Wednesday, "T4" },
                        { DayOfWeek.Thursday, "T5" },
                        { DayOfWeek.Friday, "T6" },
                        { DayOfWeek.Saturday, "T7" },
                        { DayOfWeek.Sunday, "CN" }
                    };

                foreach (var d in days)
                {
                    var dayModel = new WorkingDayViewModel
                    {
                        Day = d.Day,
                        Month = d.Month,
                        Year = d.Year,
                        Type = d.Type,
                        Note = d.Note,
                        IsOverride = d.IsOverride,
                        LastModifiedByUserName = d.LastModifiedByUserName,
                        LastModifiedOnDate = d.LastModifiedOnDate,
                    };
                    dayModel.Date = new DateTime(d.Year, d.Month, d.Day);
                    dayModel.LunarDay = lunardate.GetDayOfMonth(dayModel.Date);
                    dayModel.LunarMonth = lunardate.GetMonth(dayModel.Date);
                    dayModel.LunarYear = lunardate.GetYear(dayModel.Date);
                    //dayModel.DayOfWeek = dayModel.Date.DayOfWeek.ToString();
                    dayModel.DayOfWeek = daysOfWeekMapping[dayModel.Date.DayOfWeek];

                    result.Add(dayModel);
                }

                return new Response<List<WorkingDayViewModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: {@param}");
                return Helper.CreateExceptionResponse<List<WorkingDayViewModel>>(ex);
            }
        }

        public async Task<Response> UpdateWorkingDay(int year, int month, int day, WorkingDayUpdateModel model)
        {
            try
            {
                var workingDay = await _dbContext.mk_WorkingDay
                    .Where(c => c.Day == day && c.Month == month && c.Year == year).FirstOrDefaultAsync();
                if (workingDay != null)
                {
                    workingDay.Type = model.Type;
                    workingDay.Note = model.Note;
                    workingDay.IsOverride = true;

                    var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                    workingDay.LastModifiedByUserId = currentUser.UserId;
                    workingDay.LastModifiedOnDate = DateTime.Now;
                    workingDay.LastModifiedByUserName = currentUser.FullName;

                    await _dbContext.SaveChangesAsync();

                    return new Response(HttpStatusCode.OK, "Chỉnh sửa thành công");
                }
                return new Response(HttpStatusCode.NotFound, "Chỉnh sửa thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: {@param}");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }
    }
}
