using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSPC.Common;
using NSPC.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class PushNotificationHandler : IPushNotificationHandler
    {
        private readonly string appSrc;

        public PushNotificationHandler()
        {
            appSrc = "Uberental";
        }

        /*public async Task<Response> PushNotificationByDeviceId(PushMessageModel model)
        {
            using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
            {
                var deviceRepo = unitOfWork.GetRepository<CommUserDevice>();
                var getDeviceTokenByUser = deviceRepo.GetAll().Where(x => x.DeviceToken.Equals(model.DeviceToken)).FirstOrDefault();
                if (getDeviceTokenByUser != null)
                {
                    var userMessageData = await CreateMessage(model) as ResponseObject<UserMessageModel>;
                    if (userMessageData.Code == Code.Success)
                    {
                        var pushNotificationData = await PushNotification(userMessageData.Data, model.DeviceToken);
                        if (pushNotificationData.Code == Code.Success)
                            return new Response(Code.Success, "PUSH_SUCCESS");
                    }
                    return new Response(Code.ServerError, "MESSAGE_CREATE_ERROR");
                }
                return new Response(Code.ServerError, "INVALID_DEVICE_TOKEN");
            }
*/


        /*public async Task<Response> PushNotificationByUser(Guid userId, PushMessageModel model)
        {
            using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
            {
                var deviceRepo = unitOfWork.GetRepository<CommUserDevice>();
                var getDeviceTokenByUser = deviceRepo.GetAll().Where(sp => sp.UserId == userId).ToList();
                if (getDeviceTokenByUser != null)
                {
                    var userMessageData = await CreateMessage(model) as ResponseObject<UserMessageModel>;

                    if (userMessageData.Code == Code.Success)
                    {
                        var totalPushSuccess = 0;

                        foreach (var userDevice in getDeviceTokenByUser)
                        {
                            model.DeviceToken = userDevice.DeviceToken;
                            var pushNotificationData = await PushNotification(userMessageData.Data, model.DeviceToken);
                            if (pushNotificationData.Code == Code.Success) totalPushSuccess += 1;
                        }
                        return new Response(Code.Success, "PUSH_SUCCESS_" + totalPushSuccess + "/" + getDeviceTokenByUser.Count() + "_DEVICES");
                    }
                    return new Response(Code.ServerError, "MESSAGE_CREATE_ERROR");
                }
                return new Response(Code.ServerError, "NO_DEVICES_ARE_REGISTERED_BY_THE_USER");
            }
        }*/

        public async Task<Response> PushNotification(UserMessageModel model,List<idm_User> listReceiveUser)
        {
            var respCode = await SendOTT(appSrc, model.Title, model.Content, model.Payload, listReceiveUser);

            if (respCode == Config.CODE_ERR)
            {
                return new Response(HttpStatusCode.Forbidden, Enum.GetName(typeof(PushNotificationStatus), PushNotificationStatus.Fail));
            }
            return new Response(HttpStatusCode.OK, "PUSH_SUCCESS");
        }

        /// <summary>
        /// Push Notify using admin sdk
        /// </summary>
        /// <param name="AppSrc"></param>
        /// <param name="deviceId"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="payloadData"></param>
        /// <returns></returns>
        public async Task<string> SendOTT(string AppSrc, string title, string content, string payloadData, List<idm_User> listReceiveUser)
        {
            try
            {
                var filepath = string.Format(Directory.GetCurrentDirectory() + "/Resources/FCM/{0}.json", AppSrc);
                if (!File.Exists(filepath))
                {
                    Log.Error(filepath + "is not exist");
                    return Config.CODE_ERR;
                }
                FirebaseApp firebaseApp = FirebaseApp.GetInstance(AppSrc);
                if (firebaseApp == null)
                {
                    firebaseApp = FirebaseApp.Create(new AppOptions()
                    {
                        //Credential = GoogleCredential.GetApplicationDefault(),
                        Credential = GoogleCredential.FromFile(filepath)
                    }, AppSrc);
                }
                Log.Information(firebaseApp.Name); // "[DEFAULT]"

                // Retrieve services by passing the defaultApp variable...
                var defaultAuth = FirebaseAuth.GetAuth(firebaseApp);

                // ... or use the equivalent shorthand notation
                //defaultAuth = FirebaseAuth.DefaultInstance;
                Dictionary<string, string> extrasData = new Dictionary<string, string>();
                if (payloadData != null)
                {
                    try
                    {
                        var jsonObj = JObject.Parse(payloadData);
                        extrasData = jsonObj.ToObject<Dictionary<string, string>>();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, string.Empty);
                    }
                }

                var listMessage = new List<Message>();
                // See documentation on defining a message payload.
                foreach (var user in listReceiveUser)
                {
                    var message = new Message()
                    {
                        Data = extrasData,
                        Notification = new Notification()
                        {
                            Title = title,
                            Body = content,
                        },
                        Apns = new ApnsConfig()
                        {
                            Aps = new Aps()
                            {
                                Badge = 1,
                                Sound = "default"
                            },
                        },
                        Android = new AndroidConfig()
                        {
                            Notification = new AndroidNotification()
                            {
                                Sound = "default"
                            }
                        },                       
                        Token = user.DeviceToken,
                    };

                    // Send a message to the device corresponding to the provided
                    // registration token.
                    listMessage.Add(message);
                }
                var a = await FirebaseMessaging.GetMessaging(firebaseApp).SendAllAsync(listMessage);
                if(a.FailureCount > 0)
                {
                    foreach(var item in a.Responses)
                    {
                        Log.Error(item.Exception, "\n"+item.Exception.MessagingErrorCode.ToString() +" "+ item.Exception.ErrorCode);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Config.CODE_ERR;
            }
        }
    }
}