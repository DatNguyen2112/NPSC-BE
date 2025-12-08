using NSPC.Business;
using NSPC.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NSPC.Data;
using System.Linq;
using MediatR;
using DocType = NSPC.Common.AttachmentDocTypeConstants;
using EntityType = NSPC.Common.AttachmentEntityTypeConstants;
using AutoMapper;
using NSPC.Data.Data;

namespace NSPC.Api.Controllers
{
    public class AttachmentTemplate
    {
        public string EntityType { get; set; }
        public string DocType { get; set; }
        public string DocTypeName { get; set; }
        public int Order { get; set; }
        public int MaxQuantity { get; set; }
        public int MinQuantity { get; set; }
    }


    /// <summary>
    ///     Core - Upload
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/upload")]
    [ApiExplorerSettings(GroupName = "Admin Upload")]
    public class UploadController : ControllerBase
    {
        private List<AttachmentTemplate> templates;

        private readonly SMDbContext _dbContext;
        private readonly IMediator _mediator;
        public UploadController(SMDbContext dbcontext, IMediator mediator)
        {
            _dbContext = dbcontext;
            _mediator = mediator;

            templates = new List<AttachmentTemplate>();
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

            //Customer
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Customer, DocType = DocType.Customer_Portrait, DocTypeName = "Ảnh chân dung", MaxQuantity = 1, MinQuantity = 1});
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Customer, DocType = DocType.Customer_FrontIdentification, DocTypeName = "Ảnh mặt trước CCCD", MaxQuantity = 1, MinQuantity = 1});
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Customer, DocType = DocType.Customer_BackIdentification, DocTypeName = "Ảnh mặt sau CCCD", MaxQuantity = 1, MinQuantity = 1});
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Customer, DocType = DocType.Customer_Other, DocTypeName = "Ảnh khác", MinQuantity = 0});

            templates.Add(new AttachmentTemplate { EntityType = EntityType.CustomerService, DocType = DocType.Customer_Service_Image, DocTypeName = "Ảnh thiết bị", MinQuantity = 1});
            templates.Add(new AttachmentTemplate { EntityType = EntityType.PaidHistory, DocType = DocType.Customer_Service_Invoice, DocTypeName = "Ảnh giao dịch", MinQuantity = 0});

            //Manh Khanh
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Avatar_Supplies, DocType = DocType.Avatar_Supplies, DocTypeName = "Ảnh đại diện" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Attach, DocType = DocType.Attach, DocTypeName = "Ảnh đính kèm" });
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Avatar, DocType = DocType.Avatar, DocTypeName = "Ảnh đại diện user" });

            //Tenant
            templates.Add(new AttachmentTemplate { EntityType = EntityType.Tenant, DocType = DocType.Logo, DocTypeName = "Logo tenant" });

            templates.Add(new AttachmentTemplate { EntityType = EntityType.Task, DocType = DocType.Task, DocTypeName = "Task" });

        }

        /// <summary>
        /// Tải lên một file
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="file"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        [RequestSizeLimit(40000000)] 
        [HttpPost]
        [Route("blob/{docType}")]
        [ProducesResponseType(typeof(Response<AttachmentDetailViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFormFile(string docType, IFormFile file, IFormCollection formData)
        {
            if (file == null)
                return Helper.TransformData(Helper.CreateBadRequestResponse<AttachmentDetailViewModel>("No file specified."));

            if (string.IsNullOrEmpty(docType)) docType = "general";

            // Get attachment tempalte
            var attTemplate = templates.FirstOrDefault(x => x.DocType == docType);
            if (attTemplate == null)
                return Helper.TransformData(Helper.CreateBadRequestResponse<AttachmentDetailViewModel>("No correct template specified."));

            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            try
            {
                // Fill EntityId if possible
                bool hasEntityId = false;
                Guid entityId = Guid.Empty;
                string entityShortId = string.Empty;
                if (formData.ContainsKey("entityId"))
                {
                    var strEntityId = formData["entityId"].ToString();
                    Guid.TryParse(strEntityId, out entityId);
                    if (entityId != Guid.Empty)
                    {
                        entityShortId = entityId.ToString().Substring(0, 8);
                        hasEntityId = true;
                    }
                }

                // Fill entityDate if possible
                bool hasEntityDate = false;
                string entityDate = string.Empty;
                if (formData.ContainsKey("entityDate"))
                {
                    entityDate = formData["entityDate"].ToString();
                    if (entityDate.Length == 8 && entityDate.Contains("/"))
                        hasEntityDate = true;
                }
                if (!hasEntityDate)
                    entityDate = DateTime.Now.ToString("yy/MM/dd");

                // Incase we have entity id folder path will be
                // public/{entityType}/yy/MM/dd/
                string _fileRootPath = Path.Combine(Utils.GetConfig("StaticFiles:Folder"));
                var newPath = string.Format("temp/{0}/{1}", docType, entityDate);
                if (hasEntityId)
                    newPath = string.Format("public/{0}/{1}/{2}", attTemplate.EntityType, entityDate, entityShortId);

                if (!Directory.Exists(Path.Combine(_fileRootPath, newPath)))
                    Directory.CreateDirectory(Path.Combine(_fileRootPath, newPath));

                var fullfolderPath = Path.Combine(_fileRootPath, newPath);
                var oldFileName = Path.GetFileName(file.FileName);
                var oldFileNameWE = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(oldFileName);
                var newFilePostfix = DateTime.Now.Ticks;
                var newFileName = string.Format("{0}_{1}{2}", createNormalizedFileName(oldFileNameWE, 15), newFilePostfix, ext);
                var refinedOldFileName = string.Format("{0}{1}", createNormalizedFileName(oldFileNameWE, 20), ext);
                if (hasEntityId)
                    newFileName = string.Format("{0}_{1}_{2}_{3}{4}", entityShortId, docType, createNormalizedFileName(oldFileNameWE, 15), newFilePostfix, ext);

                // Generate new file name
                var fullFilePath = combineUnixPath(fullfolderPath, newFileName);
                var relativeFilePath = combineUnixPath(newPath, newFileName);

                // Create new attachment
                var att = new erp_Attachment
                {
                    Id = Guid.NewGuid(),
                    EntityType = attTemplate.EntityType,
                    // EntityId =  // Later update
                    DocType = docType,
                    DocTypeName = attTemplate.DocTypeName,
                    FilePath = relativeFilePath,
                    FileType = FileTypeConstants.GetFileType(ext),
                    FileSize = file.Length,
                    OriginalFileName = refinedOldFileName,
                    StatusCode = FileStatusConstants.New,
                    CreatedByUserId = requestInfo.UserId,
                    CreatedOnDate = DateTime.Now,
                    Description = string.Empty
                };

                if (entityId != Guid.Empty) att.EntityId = entityId;

                if (!System.IO.File.Exists(fullFilePath))
                    System.IO.File.Delete(fullFilePath);

                var relativePath = Path.Combine(newPath, newFileName);
                var uploadResult = new AttachmentDetailViewModel
                {
                    Id = att.Id,
                    FileName = refinedOldFileName,
                    FileUrl = Utils.FetchHost(relativeFilePath),
                    FileSize = file.Length,
                    FileType = att.FileType,
                    DocType = att.DocType,
                    EntityType = att.EntityType,
                    DocTypeName = attTemplate.DocTypeName,
                    FilePath = relativeFilePath,
                    CreatedOnDate = DateTime.Now
                };
                _dbContext.erp_Attachment.Add(att);
                await _dbContext.SaveChangesAsync();

                if (file.Length > 0)
                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                // UpdatePaid to Post
                // Notify other modules
                await _mediator.Publish(new AttachmentUploadedMessage
                {
                    Id = att.Id,
                    FileName = refinedOldFileName,
                    FileUrl = Utils.FetchHost(relativeFilePath),
                    FileSize = file.Length,
                    FileType = att.FileType,
                    DocType = att.DocType,
                    EntityType = att.EntityType,
                    FilePath = relativeFilePath,
                });

                return Helper.TransformData(new Response<AttachmentDetailViewModel>(uploadResult));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return Helper.TransformData(Helper.CreateExceptionResponse<AttachmentDetailViewModel>(ex));
            }
        }

        [HttpGet]
        [Route("{entityType}/attachment-templates")]
        public Response<List<AttachmentTemplate>> GetAttachmentTemplates(string entityType)
        {
            var result = new List<AttachmentTemplate>();

            return Helper.CreateSuccessResponse(templates.Where(x => x.EntityType == entityType).OrderBy(x => x.Order).ToList());
        }

        [HttpGet]
        [Route("attachment-template/by-entity-type/{entityType}")]
        public Response<List<AttachmentTemplate>> GetAttachmentTemplatesByEntity(string entityType)
        {
            var result = new List<AttachmentTemplate>();

            return Helper.CreateSuccessResponse(templates.Where(x => x.EntityType == entityType).OrderBy(x => x.Order).ToList());
        }

        [HttpGet]
        [Route("attachment-template/by-doc-type/{docType}")]
        public Response<List<AttachmentTemplate>> GetAttachmentTemplatesByDocType(string docType)
        {
            var result = new List<AttachmentTemplate>();

            return Helper.CreateSuccessResponse(templates.Where(x => x.DocType == docType).OrderBy(x => x.Order).ToList());
        }

        private string combineUnixPath(string path1, string path2)
        {
            return Path.Combine(path1, path2).Replace("\\", "/");
        }

        public class CropSize
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        public static string createNormalizedFileName(string name, int length)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            name = name.ToLower();
            name = RemoveUnicode(name);
            name = RemoveSpecialChar(name);
            name = name.Replace(' ', '-');

            if (length > 0 && name.Length > length)
                return name.Substring(0, length);
            return name;
        }

        //public static string RemoveMultipleSpace(string input)
        //{
        //    RegexOptions options = RegexOptions.None;
        //    Regex regex = new Regex("[ ]{2,}", options);
        //    return regex.Replace(input, "");
        //}

        public static string RemoveSpecialChar(string input)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[^0-9a-zA-Z]", options);
            return regex.Replace(input, "");
        }
    }
}