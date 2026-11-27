
using System.Net.Sockets;
using YMH219_ChatApp.Models;

namespace YMH219_ChatApp.Networking
{
    public sealed class ChatConnection : IDisposable
    {
        private readonly TcpClient client;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;
        private readonly SemaphoreSlim sendLock = new(1, 1);

        private bool disposed;

        public bool IsConnected => client.Connected;

        public ChatConnection(TcpClient tcpClient)
        {
            client = tcpClient;

            NetworkStream stream = client.GetStream();

            reader = ChatProtocol.CreateReader(stream);
            writer = ChatProtocol.CreateWriter(stream);
        }

        public async Task SendAsync(
            ChatMessage message,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            string json = ChatProtocol.Serialize(message);

            await sendLock.WaitAsync(cancellationToken);

            try
            {
                ObjectDisposedException.ThrowIf(disposed, this);

                await writer.WriteLineAsync(json.AsMemory(), cancellationToken);
                await writer.FlushAsync(cancellationToken);
            }
            finally
            {
                sendLock.Release();
            }
        }

        public async Task<ChatMessage?> ReceiveAsync(
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            string? json = await reader.ReadLineAsync(cancellationToken);

            if (json == null)
                return null;

            return ChatProtocol.Deserialize(json);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            reader.Dispose();
            writer.Dispose();
            client.Close();

            sendLock.Dispose();
        }
    }
}

