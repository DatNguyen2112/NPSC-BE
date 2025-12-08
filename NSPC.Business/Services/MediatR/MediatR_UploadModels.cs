using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSPC.Common;
using NSPC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NSPC.Business
{
    /// <summary>
    /// Message thông báo khi có attachment được upload vào hệ thống
    /// </summary>
    public class AttachmentUploadedMessage: AttachmentDetailViewModel, INotification
    {
    }
}
