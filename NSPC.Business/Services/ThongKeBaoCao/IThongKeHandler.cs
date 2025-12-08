using NSPC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IThongKeHandler
    {
        Task<Response<Pagination<ThongKeTonKhoViewModel>>> GetPage(ThongKeTonKhoQueryModel query);
    }
}
