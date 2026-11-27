
using System.Text;
using System.Text.Json;
using YMH219_ChatApp.Models;

namespace YMH219_ChatApp.Networking
{
    public static class ChatProtocol
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(ChatMessage message)
        {
            return JsonSerializer.Serialize(message, JsonOptions);
        }

        public static ChatMessage Deserialize(string json)
        {
            ChatMessage? message =
                JsonSerializer.Deserialize<ChatMessage>(json, JsonOptions);

            if (message == null)
                throw new InvalidDataException("Geçersiz mesaj verisi.");

            if (message.Id == Guid.Empty)
                throw new InvalidDataException("Mesaj kimliği geçersiz.");

            if (string.IsNullOrWhiteSpace(message.Sender))
                throw new InvalidDataException("Gönderen bilgisi eksik.");

            if (message.Text == null || message.Text.Length > 2000)
                throw new InvalidDataException("Mesaj içeriği geçersiz.");

            return message;
        }

        public static StreamReader CreateReader(Stream stream)
        {
            return new StreamReader(
                stream,
                new UTF8Encoding(false),
                false,
                1024,
                true
            );
        }

        public static StreamWriter CreateWriter(Stream stream)
        {
            return new StreamWriter(
                stream,
                new UTF8Encoding(false),
                1024,
                true)
            {
                AutoFlush = true,
                NewLine = "\n"
            };
        }
    }
}
