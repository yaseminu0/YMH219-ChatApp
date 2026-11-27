
namespace YMH219_ChatApp.Models
{
    public sealed record ChatMessage
    {
        public Guid Id { get; init; }

        public string Sender { get; init; } = string.Empty;

        public DateTimeOffset TimestampUtc { get; init; }

        public string Text { get; init; } = string.Empty;

        public static ChatMessage Create(string sender, string text)
        {
            if (string.IsNullOrWhiteSpace(sender))
                throw new ArgumentException("Kullanıcı adı boş olamaz.");

            if (sender.Length > 40)
                throw new ArgumentException("Kullanıcı adı en fazla 40 karakter olabilir.");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Mesaj boş olamaz.");

            if (text.Length > 2000)
                throw new ArgumentException("Mesaj en fazla 2000 karakter olabilir.");

            return new ChatMessage
            {
                Id = Guid.NewGuid(),
                Sender = sender.Trim(),
                TimestampUtc = DateTimeOffset.UtcNow,
                Text = text
            };
        }
    }
}
