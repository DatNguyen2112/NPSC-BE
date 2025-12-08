using NSPC.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IAttachmentHandler
    {
        Response<string> MoveEntityAttachment(string docType, string entityType, Guid entityId, string oldPath, DateTime? entityDate);
        Task CleanupOldAttachments(Guid entityId, List<Guid> attachmentListId);
        Task<Response> ValidateAttachment(string entityType, List<Guid> ids);
        Task<Response> RemoveOldAttachment();
    }
}