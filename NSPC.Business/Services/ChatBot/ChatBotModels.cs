using System.Text.Json.Serialization;

namespace NSPC.Business.Services.ChatBot;

public class ChatBotPromptModel
{
    public string Prompt { get; set; }
    public Guid? CustomerId { get; set; }
}

public class ChatGptResponse
{
    public class ResponseUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptPokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = null;

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class ErrorResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string ErrMessage { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, dynamic> Metadata { get; set; } = null;
    }

    public class NonStreamingChoice
    {
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = null;

        [JsonPropertyName("native_finish_reason")]
        public string NativeFinishReason { get; set; } = null;

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("error")]
        public ErrorResponse Error { get; set; } = null;
    }

    public class StreamingChoice
    {
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = null;

        [JsonPropertyName("native_finish_reason")]
        public string NativeFinishReason { get; set; } = null;

        [JsonPropertyName("delta")]
        public Message Delta { get; set; }

        [JsonPropertyName("error")]
        public ErrorResponse Error { get; set; } = null;
    }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("choices")]
    public List<StreamingChoice> Choices { get; set; } = new();

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("system_fingerprint")]
    public string SystemFingerprint { get; set; }

    [JsonPropertyName("usage")]
    public ResponseUsage Usage { get; set; } = null;
}