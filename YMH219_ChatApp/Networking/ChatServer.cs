
using System.Net;
using System.Net.Sockets;

namespace YMH219_ChatApp.Networking
{
    public sealed class ChatServer : IDisposable
    {
        private TcpListener? listener;
        private bool disposed;

        public bool IsRunning { get; private set; }

        public async Task<ChatConnection> StartAsync(
            int port,
            CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (IsRunning)
                throw new InvalidOperationException(
                    "Sunucu zaten çalışıyor.");

            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port));

            listener = new TcpListener(IPAddress.Any, port);

            listener.Start();
            IsRunning = true;

            try
            {
                TcpClient client =
                    await listener.AcceptTcpClientAsync(cancellationToken);

                client.NoDelay = true;

                return new ChatConnection(client);
            }
            catch
            {
                Stop();
                throw;
            }
        }

        public void Stop()
        {
            if (listener == null)
                return;

            listener.Stop();
            listener = null;
            IsRunning = false;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            Stop();
            disposed = true;
        }
    }
}
