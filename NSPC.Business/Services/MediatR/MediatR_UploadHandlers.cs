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
    /* Logic */
    /* Khi có attachment được upload vào post thì thực hiện cập nhật:
     * - LastModified của Post
     * - Attachments của Post
     * - Các field Url của Post với từng loại DocType tương ứng, ví dụ: post_thumb thì cập nhật vào ThumbnailUrl
     */
    /// <summary>
    /// Xử lý các message thông báo khi có attachment upload vào Post
    /// </summary>
    
    
}
