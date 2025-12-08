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

namespace NSPC.Api.Controllers
{
    /// <summary>
    ///     Core - Upload
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/core/nodes")]
    [ApiExplorerSettings(GroupName = "Admin Upload")]
    public class MdmNodeUploadController : ControllerBase
    {
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly List<CropSize> _cropSize;
        private readonly IParameterHandler _parameterHandler;

        public MdmNodeUploadController(IHostEnvironment env, IConfiguration configuration, IParameterHandler parameterHandler)
        {
            _env = env;
            _configuration = configuration;
            _cropSize = _configuration.GetSection("ItemMediaSize").Get<List<CropSize>>();
            _parameterHandler = parameterHandler;
        }

        /// <summary>
        /// Tải lên một file dưới dạng Base64
        /// </summary>
        /// <param name="model">Thông tin upload</param>
        /// <param name="destinationPhysicalPath">Đường dẫn vật lý (bắt buộc)</param>
        /// <param name="isResize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [Authorize, HttpPost]
        [Route("upload/physical/base64")]
        [ProducesResponseType(typeof(Response<FileUploadResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFileBase64Physical([FromBody] Base64FileData model,
        string destinationPhysicalPath = null, bool isResize = true, int width = 1200, int height = 1200)
        {
            if (string.IsNullOrEmpty(destinationPhysicalPath))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "destinationPhysicalPath cannot leave blank"));
            if (destinationPhysicalPath.Contains("."))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "Malformed destinationPhysicalPath"));

            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            try
            {
                string rootPath = Path.Combine(Utils.GetConfig("StaticFiles:Folder"));

                var isExistDirectory = Directory.Exists(Path.Combine(rootPath, destinationPhysicalPath));
                if (!isExistDirectory) Directory.CreateDirectory(Path.Combine(rootPath, destinationPhysicalPath));

                var fullfolderPath = Path.Combine(rootPath, destinationPhysicalPath);

                // get data image from client
                var base64 = model.FileData;
                if (base64.IndexOf(',') >= 0) base64 = base64.Substring(base64.IndexOf(',') + 1);
                var bytes = Convert.FromBase64String(base64);
                var filename = model.Name;

                var filePrefix = DateTime.Now.ToString("HHmm");
                var filePostFix = DateTime.Now.ToString("yyMMdd");

                var newFileName = Path.GetFileNameWithoutExtension(filename) + "_" + filePrefix + "_" +
                                  filePostFix + Path.GetExtension(filename);

                if (newFileName.Length > 255)
                {
                    var withoutExtName = Path.GetFileNameWithoutExtension(filename);
                    var extName = Path.GetExtension(filename);
                    var trimmed = withoutExtName.Substring(0,
                        withoutExtName.Length - (255 - filePrefix.Length - filePostFix.Length - extName.Length));
                    newFileName = trimmed + "_" + filePrefix + "_" + filePostFix + extName;
                }

                var fullFilePath = Path.Combine(fullfolderPath, newFileName);

                if (!System.IO.File.Exists(fullFilePath))
                    System.IO.File.Delete(fullFilePath);

                using (var imageFile = new FileStream(fullFilePath, FileMode.Create))
                {
                    await imageFile.WriteAsync(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                var fileInfo = new FileInfo(fullFilePath);
                var relativePath = Path.Combine(destinationPhysicalPath, newFileName);
                if (isResize)
                {
                    await Utils.ThumbnailImageAsync(fullFilePath, fullfolderPath, width, height);
                }

                if (isResize)
                {
                    await Utils.ThumbnailImageAsync(fullFilePath, fullFilePath, width, height);
                }

                return Helper.TransformData(new Response<FileUploadResult>(new FileUploadResult
                {
                    PhysicalPath = Path.Combine(relativePath).Replace("\\", "/"),
                    PhysicalName = newFileName,
                    Name = filename,
                    Size = fileInfo.Length,
                    Extension = fileInfo.Extension
                }));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return Helper.TransformData(new ResponseError(HttpStatusCode.InternalServerError,
                    "Có lỗi trong quá trình xử lý: " + ex.Message));
            }
        }

        /// <summary>
        /// Tải lên một file
        /// </summary>
        /// <param name="file">File upoad</param>
        /// <param name="destinationPhysicalPath">Đường dẫn vật lý (tùy chọn)</param>
        /// <param name="isResize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("upload/physical/blob")]
        [ProducesResponseType(typeof(Response<FileUploadResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFileBlobPhysical(IFormFile file, int width = 1200, int height = 1200,
        string destinationPhysicalPath = "",
        bool isResize = true)
        {
            //if (string.IsNullOrEmpty(destinationPhysicalPath))
            //    return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "destinationPhysicalPath cannot leave blank"));
            if (destinationPhysicalPath.Contains("."))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "Malformed destinationPhysicalPath"));

            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            try
            {
                string rootPath = Path.Combine(Utils.GetConfig("StaticFiles:Folder"));

                var isExistDirectory = Directory.Exists(Path.Combine(rootPath, destinationPhysicalPath));
                if (!isExistDirectory) Directory.CreateDirectory(Path.Combine(rootPath, destinationPhysicalPath));

                var fullfolderPath = Path.Combine(rootPath, destinationPhysicalPath);

                var filename = file.FileName ?? "DCS";
                filename = filename.Trim().Trim('"').Replace("&", "");

                // Generate new file name
                var filePrefix = DateTime.Now.ToString("HHmm");
                var filePostFix = DateTime.Now.ToString("yyMMdd");

                var newFileName = Path.GetFileNameWithoutExtension(filename) + "_" + filePrefix + "_" +
                                                  filePostFix + Path.GetExtension(filename);

                if (newFileName.Length > 255)
                {
                    var withoutExtName = Path.GetFileNameWithoutExtension(filename);
                    var extName = Path.GetExtension(filename);
                    var trimmed = withoutExtName.Substring(0,
                        withoutExtName.Length - (255 - filePrefix.Length - filePostFix.Length - extName.Length));
                    newFileName = trimmed + "_" + filePrefix + "_" + filePostFix + Path.GetExtension(filename);
                }

                var fullFilePath = Path.Combine(fullfolderPath, newFileName);

                if (!System.IO.File.Exists(fullFilePath))
                    System.IO.File.Delete(fullFilePath);

                if (file.Length > 0)
                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                var relativePath = Path.Combine(destinationPhysicalPath, newFileName);
                var uploadResult = new FileUploadResult
                {
                    PhysicalPath = Path.Combine(relativePath).Replace("\\", "/"),
                    Name = filename,
                    PhysicalName = newFileName,
                    Size = file.Length,
                    Extension = Path.GetExtension(file.FileName)
                };

                if (file.Length > 0)
                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                if (isResize)
                {
                    await Utils.ThumbnailImageAsync(fullFilePath, fullFilePath, width, height);
                }

                return Helper.TransformData(new Response<FileUploadResult>(uploadResult));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return Helper.TransformData(new ResponseError(HttpStatusCode.InternalServerError,
                    "Có lỗi trong quá trình xử lý: " + ex.Message));
            }
        }

        #region Upload Image

        /// <summary>
        ///     Tải lên nhiều file
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="destinationPhysicalPath">Đường dẫn vật lý (tùy chọn)</param>
        /// <param name="isResize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("upload/physical/blob/many")]
        [ProducesResponseType(typeof(Response<List<FileUploadResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFileBlobManyPhysical([FromForm] IFormCollection formData,
         string destinationPhysicalPath = null,
        bool isResize = true, int width = 1200, int height = 1200)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            if (string.IsNullOrEmpty(destinationPhysicalPath))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "destinationPhysicalPath cannot leave blank"));
            if (destinationPhysicalPath.Contains("."))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "Malformed destinationPhysicalPath"));

            // Call service
            try
            {
                #region Lấy thông tin vùng lưu trữ vật lý

                string partitionPhysicalPath = Path.Combine(Utils.GetConfig("StaticFiles:Folder"));

                #endregion Lấy thông tin vùng lưu trữ vật lý

                #region Xác định thư mục đích

                // Get root path
                var rootPath = partitionPhysicalPath;
                Log.Debug("Upload root path : " + rootPath);

                if (string.IsNullOrEmpty(destinationPhysicalPath))
                {
                    var paramResponse = await _parameterHandler.FindByNameAsync("DefaultAlias");
                    if (paramResponse.Code != HttpStatusCode.OK)
                        return null;
                    var param = paramResponse as Response<ParameterModel>;

                    destinationPhysicalPath = param.Data.Value;

                    destinationPhysicalPath = destinationPhysicalPath + "\\" +
                                              DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") +
                                              "\\" +
                                              DateTime.Now.ToString("dd") + "\\";
                    bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                    if (!isWindows)
                    {
                        destinationPhysicalPath = destinationPhysicalPath.Replace("\\", "/");
                    }
                }

                var sourcefolder = destinationPhysicalPath;

                // Create folder path
                if (!Directory.Exists(Path.Combine(rootPath, sourcefolder)))
                    Directory.CreateDirectory(Path.Combine(rootPath, sourcefolder));

                #endregion Xác định thư mục đích

                #region Do Upload

                var listUploadResult = new List<FileUploadResult>();
                foreach (var file in formData.Files)
                {
                    #region Tạo tên file từ đích

                    var filename = file.FileName ?? "NoName";
                    filename = filename.Trim('"').Replace("&", "and");

                    // Generate new file name
                    var filePrefix = DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm") +
                                     DateTime.Now.ToString("ss") +
                                     new Random(DateTime.Now.Millisecond).Next(10, 99);

                    var filePostFix = DateTime.Now.ToString("yyMMdd");

                    var newFileName = Path.GetFileNameWithoutExtension(filename) + "_" + filePrefix + "_" +
                                      filePostFix + Path.GetExtension(filename);
                    // Check length

                    if (newFileName.Length > 255)
                    {
                        var withoutExtName = Path.GetFileNameWithoutExtension(filename);
                        var extName = Path.GetExtension(filename);
                        var trimmed = withoutExtName.Substring(0,
                            withoutExtName.Length - (255 - filePrefix.Length - filePostFix.Length - extName.Length));

                        newFileName = trimmed + "_" + filePrefix + "_" + filePostFix + Path.GetExtension(filename);
                    }

                    #endregion Tạo tên file từ đích

                    var fullPath = Path.Combine(sourcefolder, newFileName);
                    listUploadResult.Add(new FileUploadResult
                    {
                        PhysicalPath = Path.Combine(sourcefolder, newFileName).Replace("\\", "/"),
                        Name = filename,
                        PhysicalName = newFileName,
                        Size = file.Length,
                        Extension = Path.GetExtension(file.FileName)
                    });
                    if (file.Length > 0)
                        using (var stream = new FileStream(Path.Combine(rootPath, fullPath), FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                    if (isResize)
                    {
                        await Utils.ThumbnailImageAsync(Path.Combine(rootPath, fullPath), Path.Combine(rootPath, fullPath), width, height);
                    }
                }

                #endregion Do Upload

                return Helper.TransformData(new Response<List<FileUploadResult>>(listUploadResult));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return Helper.TransformData(new ResponseError(HttpStatusCode.InternalServerError,
                    "Có lỗi trong quá trình xử lý: " + ex.Message));
            }
        }

        /// <summary>
        ///     Tải lên nhiều file dưới dạng Base64
        /// </summary>
        /// <param name="model">Thông tin upload</param>
        /// <param name="destinationPhysicalPath">Đường dẫn vật lý (tùy chọn)</param>
        /// <param name="isResize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("upload/physical/base64/many")]
        [ProducesResponseType(typeof(Response<List<FileUploadResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFileBase64ManyPhysical([FromBody] List<Base64FileData> model,
         string destinationPhysicalPath = null,
        bool isResize = true, int width = 1200, int height = 1200)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            if (string.IsNullOrEmpty(destinationPhysicalPath))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "destinationPhysicalPath cannot leave blank"));
            if (destinationPhysicalPath.Contains("."))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "Malformed destinationPhysicalPath"));


            // Call service
            try
            {
                #region Lấy thông tin vùng lưu trữ vật lý

                string partitionPhysicalPath;
                partitionPhysicalPath = Path.Combine(Utils.GetConfig("StaticFiles:Folder"));

                #endregion Lấy thông tin vùng lưu trữ vật lý

                #region Xác định thư mục đích

                // Get root path
                var rootPath = partitionPhysicalPath;
                Log.Debug("Upload root path : " + rootPath);
                // Append destinationPhysicalPath
                if (string.IsNullOrEmpty(destinationPhysicalPath))
                {
                    var paramResponse = await _parameterHandler.FindByNameAsync("DefaultAlias");
                    if (paramResponse.Code != HttpStatusCode.OK)
                        return null;
                    var param = paramResponse as Response<ParameterModel>;

                    destinationPhysicalPath = param.Data.Value;

                    destinationPhysicalPath = destinationPhysicalPath + "\\" +
                                              DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") +
                                              "\\" +
                                              DateTime.Now.ToString("dd") + "\\";
                    bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                    if (!isWindows)
                    {
                        destinationPhysicalPath = destinationPhysicalPath.Replace("\\", "/");
                    }
                }

                Log.Debug("DestinationPhysicalPath : " + destinationPhysicalPath);
                var sourcefolder = destinationPhysicalPath;
                Log.Debug("Upload source folder : " + sourcefolder);
                // Create folder path
                var isExistDirectory = Directory.Exists(Path.Combine(rootPath, sourcefolder));
                if (!isExistDirectory) Directory.CreateDirectory(Path.Combine(rootPath, sourcefolder));
                var fullfolderPath = Path.Combine(rootPath, sourcefolder);

                #endregion Xác định thư mục đích

                #region Lưu file upload vật lý

                var listUploadResult = new List<FileUploadResult>();
                foreach (var fileData in model)
                {
                    #region Create File profile in file source path

                    // get data image from client
                    var base64 = fileData.FileData;
                    if (base64.IndexOf(',') >= 0) base64 = base64.Substring(base64.IndexOf(',') + 1);
                    var bytes = Convert.FromBase64String(base64);
                    var filename = fileData.Name;

                    #region makefileName

                    var filePrefix = DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm") +
                                     DateTime.Now.ToString("ss") +
                                     new Random(DateTime.Now.Millisecond).Next(10, 99);
                    var filePostFix = DateTime.Now.ToString("yy-MM-dd");
                    var newFileName = Path.GetFileNameWithoutExtension(filename) + "_" + filePrefix + "_" +
                                      filePostFix + Path.GetExtension(filename);
                    Log.Debug("New file name : " + newFileName);
                    if (newFileName.Length > 255)
                    {
                        var withoutExtName = Path.GetFileNameWithoutExtension(filename);
                        var extName = Path.GetExtension(filename);
                        var trimmed = withoutExtName.Substring(0,
                            withoutExtName.Length - (255 - filePrefix.Length - filePostFix.Length - extName.Length));
                        newFileName = trimmed + "_" + filePrefix + "_" + filePostFix + Path.GetExtension(filename);
                    }

                    #endregion makefileName

                    var fileSavePath = Path.Combine(fullfolderPath, newFileName);
                    using (var imageFile = new FileStream(fileSavePath, FileMode.Create))
                    {
                        await imageFile.WriteAsync(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    var fileInfo = new FileInfo(fileSavePath);

                    listUploadResult.Add(new FileUploadResult
                    {
                        PhysicalPath = Path.Combine(sourcefolder, fileInfo.Name).Replace("\\", "/"),
                        PhysicalName = fileInfo.Name,
                        Name = fileData.Name,
                        Size = fileInfo.Length,
                        Extension = fileInfo.Extension
                    });

                    if (isResize)
                    {
                        await Utils.ThumbnailImageAsync(Path.Combine(rootPath, fileSavePath), Path.Combine(rootPath, fileSavePath), width, height);
                    }

                    #endregion Create File profile in file source path
                }

                #endregion Lưu file upload vật lý

                return Helper.TransformData(new Response<List<FileUploadResult>>(listUploadResult));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return Helper.TransformData(new ResponseError(HttpStatusCode.InternalServerError,
                    "Có lỗi trong quá trình xử lý: " + ex.Message));
            }
        }

        /// <summary>
        ///     Tải lên một file
        /// </summary>
        /// <param name="file">File upoad</param>
        /// <param name="destinationPhysicalPath">Đường dẫn vật lý (tùy chọn)</param>
        [Authorize, HttpPost]
        [Route("upload/image/blob")]
        [ProducesResponseType(typeof(Response<List<FileUploadResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadImage(IFormFile file,
        string destinationPhysicalPath = null)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            if (string.IsNullOrEmpty(destinationPhysicalPath))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "destinationPhysicalPath cannot leave blank"));
            if (destinationPhysicalPath.Contains("."))
                return Helper.TransformData(new Response(HttpStatusCode.BadRequest, "Malformed destinationPhysicalPath"));


            // Call service
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Helper.TransformData(new ResponseError(HttpStatusCode.Forbidden, "Không tìm thấy file gửi lên!"));
                }

                var result = await Upload(file, destinationPhysicalPath);

                return Helper.TransformData(new Response<List<FileUploadResult>>(result));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return Helper.TransformData(new ResponseError(HttpStatusCode.InternalServerError,
                    "Có lỗi trong quá trình xử lý: " + ex.Message));
            }
        }

        private async Task<List<FileUploadResult>> Upload(IFormFile file, string destinationPhysicalPath)
        {
            var result = new List<FileUploadResult>();


            #region Lấy thông tin vùng lưu trữ vật lý

            string partitionPhysicalPath;
            partitionPhysicalPath = Path.Combine(Utils.GetConfig("StaticFiles:Folder"));

            #endregion Lấy thông tin vùng lưu trữ vật lý

            #region Xác định thư mục đích

            // Get root path
            var rootPath = partitionPhysicalPath;
            Log.Debug("Upload root path : " + rootPath);
            // Append destinationPhysicalPath
            if (string.IsNullOrEmpty(destinationPhysicalPath))
            {
                var paramResponse = await _parameterHandler.FindByNameAsync("DefaultAlias");
                if (paramResponse.Code != HttpStatusCode.OK)
                    return null;
                var param = paramResponse as Response<ParameterModel>;

                destinationPhysicalPath = param.Data.Value;

                destinationPhysicalPath = destinationPhysicalPath + "\\" +
                          DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") +
                          "\\" +
                          DateTime.Now.ToString("dd") + "\\";
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (!isWindows)
                {
                    destinationPhysicalPath = destinationPhysicalPath.Replace("\\", "/");
                }
            }

            Log.Debug("DestinationPhysicalPath : " + destinationPhysicalPath);
            var sourcefolder = destinationPhysicalPath;
            Log.Debug("Upload source folder : " + sourcefolder);
            // Create folder path
            var isExistDirectory = Directory.Exists(Path.Combine(rootPath, sourcefolder));
            if (!isExistDirectory) Directory.CreateDirectory(Path.Combine(rootPath, sourcefolder));

            #endregion Xác định thư mục đích

            #region Do Upload

            #region Tạo tên file từ đích

            var filename = file.FileName ?? "NoName";
            filename = filename.Trim('"').Replace("&", "and");

            // Generate new file name
            var filePrefix = DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm") +
                             DateTime.Now.ToString("ss") +
                             new Random(DateTime.Now.Millisecond).Next(10, 99);

            var filePostFix = DateTime.Now.ToString("yy-MM-dd");

            var newFileName = Path.GetFileNameWithoutExtension(filename) + "_" + filePrefix + "_" +
                              filePostFix + Path.GetExtension(filename);
            // Check length

            if (newFileName.Length > 255)
            {
                var withoutExtName = Path.GetFileNameWithoutExtension(filename);
                var extName = Path.GetExtension(filename);
                var trimmed = withoutExtName.Substring(0,
                    withoutExtName.Length - (255 - filePrefix.Length - filePostFix.Length - extName.Length));

                newFileName = trimmed + "_" + filePrefix + "_" + filePostFix + Path.GetExtension(filename);
            }

            #endregion Tạo tên file từ đích

            //Resize Image
            var listImage = new List<Image>();

            if (file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    using (var img = Image.FromStream(memoryStream))
                    {
                        foreach (var item in _cropSize)
                        {
                            var imageInstance = Utils.ResizeImage(img, new Size(item.Width, item.Height));
                            listImage.Add(imageInstance);
                        }
                    }

                    var fileNameWOE = Path.GetFileNameWithoutExtension(newFileName);
                    var extName = Path.GetExtension(filename);

                    foreach (var item in listImage)
                    {
                        var cropSize = item.Width.ToString() + "x" + item.Height.ToString();
                        var fileCropName = string.Concat(fileNameWOE, "_", cropSize, extName);
                        var savedPath = Path.Combine(sourcefolder, fileCropName);

                        var uploadResult = new FileUploadResult
                        {
                            PhysicalPath = Path.Combine(savedPath).Replace("\\", "/"),
                            Name = fileCropName,
                            PhysicalName = fileCropName,
                            Size = file.Length,
                            Extension = Path.GetExtension(file.FileName),
                            RootPath = rootPath,
                            FullPath = savedPath,
                            CropSize = cropSize
                        };

                        if (file.Length > 0)
                            using (var stream = new FileStream(Path.Combine(rootPath, uploadResult.FullPath), FileMode.Create))
                            {
                                item.Save(stream, ImageFormat.Jpeg);
                            }

                        result.Add(uploadResult);
                    }
                }
            }

            #endregion Do Upload

            return result;
        }

        #endregion Upload Image

        public class CropSize
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}