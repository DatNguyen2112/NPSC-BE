using NSPC.Common;

namespace NSPC.Business.Services.ChatBot;

public interface IChatBotHandler
{
    Task<Response> GetResponseFromBot(ChatBotPromptModel model, Stream outputStream);
}