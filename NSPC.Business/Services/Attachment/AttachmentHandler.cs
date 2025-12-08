using AutoMapper;
using NSPC.Data;
using NSPC.Common;
using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Net;
using NSPC.Data.Data;
using DocType = NSPC.Common.AttachmentDocTypeConstants;
using EntityType = NSPC.Common.AttachmentEntityTypeConstants;
namespace NSPC.Business
{
    public class AttachmentHandler : IAttachmentHandler
    {
        private readonly SMDbContext _dbContext;
        private List<AttachmentTemplate> templates;
        public AttachmentHandler(SMDbContext dbContext)
        {
            _dbContext = dbContext;
            templates = new List<AttachmentTemplate>();
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Post, DocType = DocType.Post_Cover, DocTypeName = "Ảnh đính kèm" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Post, DocType = DocType.Post_Thumbnail, DocTypeName = "Ảnh đại diện" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Order, DocType = "order_image", DocTypeName = "Ảnh đính kèm" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Order, DocType = "order_thumb", DocTypeName = "Ảnh đại diện" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.MedicalRecord, DocType = DocType.Medrec_Img_Before, DocTypeName = "Ảnh trước khi tiến hành", MinQuantity = 0, MaxQuantity = 0 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.MedicalRecord, DocType = DocType.Medrec_Film_Before, DocTypeName = "Phim trước khi tiến hành", MinQuantity = 0, MaxQuantity = 0 });

            // Upload for farmer_profile
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Farmer, DocType = DocType.Farmer_Avatar, DocTypeName = "Ảnh đại diện" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Farmer, DocType = DocType.Farmer_Medical_Degree, DocTypeName = "Ảnh bằng cấp" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Farmer, DocType = DocType.Farmer_NationalId, DocTypeName = "Ảnh CCCD" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Farmer, DocType = DocType.Farmer_Previous_Customer, DocTypeName = "Ảnh kỷ niệm với khách hàng" });


            templates.Add(new AttachmentTemplate { EntityType = EntityType.TreatmentDiary, DocType = DocType.Tredia_Img_Before, DocTypeName = "Ảnh trước khi tiến hành", MinQuantity = 1, MaxQuantity = 0 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.TreatmentDiary, DocType = DocType.Tredia_Film_Before, DocTypeName = "Phim trước khi tiến hành", MinQuantity = 1, MaxQuantity = 0 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.TreatmentDiary, DocType = DocType.Tredia_Img_After, DocTypeName = "Ảnh sau khi tiến hành", MinQuantity = 1, MaxQuantity = 0 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.TreatmentDiary, DocType = DocType.Tredia_Film_After, DocTypeName = "Phim sau khi tiến hành", MinQuantity = 1, MaxQuantity = 0 });
            // Clinic
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Clinic, DocType = DocType.Clinic_Avatar, DocTypeName = "Ảnh đại diện phòng khám", MinQuantity = 1, MaxQuantity = 1 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Clinic, DocType = DocType.Clinic_Supervisor_Avatar, DocTypeName = "Ảnh đại diện người đứng đầu", MinQuantity = 1, MaxQuantity = 1 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Clinic, DocType = DocType.Clinic_Supervisor_Degree, DocTypeName = "Ảnh bằng cấp người đứng đầu", MinQuantity = 0, MaxQuantity = 0 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Clinic, DocType = DocType.Clinic_Supervisor_NationalId, DocTypeName = "Ảnh CCCD người đứng đầu", MinQuantity = 1, MaxQuantity = 4 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Clinic, DocType = DocType.Clinic_License, DocTypeName = "Ảnh chứng chỉ phòng khám", MinQuantity = 0, MaxQuantity = 0 });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Clinic, DocType = DocType.Clinic_Equipment, DocTypeName = "Ảnh trang thiết bị phòng khám", MinQuantity = 0, MaxQuantity = 0 });

            // Orderer Claim
            templates.Add(new AttachmentTemplate { EntityType = EntityType.OrdererClaim, DocType = DocType.OrdererClaim, DocTypeName = "Ảnh khiếu nại" });

            // Withdrawal
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Wallet, DocType = DocType.WalletWithdrawal, DocTypeName = "Ảnh chụp hóa đơn chuyển khoản" });

            // Data
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Data, DocType = DocType.DataImage, DocTypeName = "Ảnh số 1" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Data, DocType = DocType.DataImage1, DocTypeName = "Ảnh số 2" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Data, DocType = DocType.DataImage2, DocTypeName = "Ảnh số 3" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Data, DocType = DocType.DataImage3, DocTypeName = "Ảnh số 4" });

        }

        public Response<string> MoveEntityAttachment(string docType, string entityType, Guid entityId, string oldPath, DateTime? entityDate)
        {
            if (string.IsNullOrEmpty(oldPath))
                return Helper.CreateBadRequestResponse<string>("No file specified.");

            if (string.IsNullOrEmpty(docType)) docType = "general";

            // Call service
            try
            {
                // Fill EntityId if possible
                var entityShortId = entityId.ToString().Substring(0, 8);

                // Fill entityDate if possible
                string entityDateString = string.Empty;
                if (entityDate.HasValue)
                    entityDateString = entityDate.Value.ToString("yy/MM/dd");
                else
                    entityDateString = DateTime.Now.ToString("yy/MM/dd");

                // Incase we have entity id folder path will be
                // public/{entityType}/yy/MM/dd/
                string _fileRootPath = Path.Combine(Utils.GetConfig("StaticFiles:Folder"));
                var oldFileName = Path.GetFileName(oldPath);

                var newPath = string.Format("public/{0}/{1}/{2}", entityType, entityDateString, entityShortId);
                
                if (oldPath.StartsWith(newPath))
                    return Helper.CreateBadRequestResponse<string>("File is already moved");

                if (!Directory.Exists(Path.Combine(_fileRootPath, newPath)))
                    Directory.CreateDirectory(Path.Combine(_fileRootPath, newPath));

                newPath = string.Format("{0}/{1}", newPath, oldFileName);

                // Move file to new path
                File.Move(Path.Combine(_fileRootPath,oldPath),Path.Combine(_fileRootPath,newPath));
                return Helper.CreateSuccessResponse<string>(newPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return Helper.CreateExceptionResponse<string>(ex);
            }
        }

        public async Task CleanupOldAttachments(Guid entityId, List<Guid> attachmentListId)
        {
            try
            {
                // Process attachments
                if (attachmentListId.Count > 0)
                {
                    var deletedAttachments = await _dbContext.erp_Attachment.Where(x => x.EntityId == entityId && !attachmentListId.Contains(x.Id)).ToListAsync();
                    if (deletedAttachments != null && deletedAttachments.Count() > 0)
                        foreach (var deletedAttachment in deletedAttachments)
                        {
                            deletedAttachment.EntityId = null;
                            deletedAttachment.IsDelete = true;
                        }
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var deletedAttachments = await _dbContext.erp_Attachment.Where(x => x.EntityId == entityId).ToListAsync();
                    if (deletedAttachments != null && deletedAttachments.Count() > 0)
                        foreach (var deletedAttachment in deletedAttachments)
                        {
                            deletedAttachment.EntityId = null;
                            deletedAttachment.IsDelete = true;
                        }
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }


        }

        public async Task<Response> ValidateAttachment(string entityType, List<Guid> ids) 
        {
            try
            {
                var listTemplate = templates.Where(x => x.EntityType == entityType).ToList();
                var attachments = await _dbContext.erp_Attachment.Where(x => ids.Contains(x.Id)).ToListAsync();
                foreach(var template in listTemplate)
                {
                    if (!attachments.Any(x => x.DocType == template.DocType))
                        return Helper.CreateBadRequestResponse(string.Format("Vui lòng chọn {0}", template.DocTypeName));
                }
                return Helper.CreateSuccessResponse("OK");
            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse("Không Valid được ảnh");
                throw;
            }
        }

        public async Task<Response> RemoveOldAttachment()
        {
            try
            {
                var filePath = Utils.GetConfig("StaticFiles:Folder");
                var attachments = await _dbContext.erp_Attachment.Where(x => x.EntityId == null).ToListAsync();
                
                foreach (var attach in attachments)
                {
                    var fullPath = Path.Combine(filePath, attach.FilePath);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                    Log.Information(fullPath);
                    _dbContext.Remove(attach);
                }
                 
                 await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("OK");
            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse("Lỗi");
                throw;
            }
        }

    }
}