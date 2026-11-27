
using System.Net.Sockets;

namespace YMH219_ChatApp.Networking
{
    public sealed class ChatClient : IDisposable
    {
        private TcpClient? client;
        private bool disposed;

        public bool IsConnected =>
            client?.Connected == true;

        public async Task<ChatConnection> ConnectAsync(
            string ipAddress,
            int port,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (client != null)
                throw new InvalidOperationException(
                    "Bağlantı nesnesi zaten kullanılıyor.");

            TcpClient newClient = new();

            try
            {
                await newClient.ConnectAsync(
                    ipAddress,
                    port,
                    cancellationToken);

                newClient.NoDelay = true;

                client = newClient;

                return new ChatConnection(newClient);
            }
            catch
            {
                newClient.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            client?.Close();
            client = null;
            disposed = true;
        }
    }
}
