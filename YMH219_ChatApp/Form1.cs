
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMH219_ChatApp.Models;
using YMH219_ChatApp.Networking;

namespace YMH219_ChatApp
{
    public partial class Form1 : Form
    {
        // =========================================
        // NETWORKING
        // =========================================

        private ChatServer? chatServer;
        private ChatClient? chatClient;
        private ChatConnection? chatConnection;
        private CancellationTokenSource? connectionCts;

        private bool connected;
        private bool closing;

        // =========================================
        // COLORS
        // =========================================

        private readonly Color backgroundColor =
            Color.FromArgb(15, 20, 35);

        private readonly Color sidebarColor =
            Color.FromArgb(21, 29, 47);

        private readonly Color panelColor =
            Color.FromArgb(29, 39, 61);

        private readonly Color inputColor =
            Color.FromArgb(39, 51, 76);

        private readonly Color accentColor =
            Color.FromArgb(105, 103, 245);

        private readonly Color accentHover =
            Color.FromArgb(125, 123, 255);

        private readonly Color textColor =
            Color.FromArgb(239, 242, 250);

        private readonly Color mutedColor =
            Color.FromArgb(145, 158, 180);

        private readonly Color successColor =
            Color.FromArgb(55, 190, 135);

        private readonly Color dangerColor =
            Color.FromArgb(220, 85, 100);

        // =========================================
        // CONTROLS
        // =========================================

        private TextBox usernameInput = new();
        private TextBox ipInput = new();
        private TextBox portInput = new();
        private TextBox messageInput = new();

        private Label statusLabel = new();
        private Label headerStatusLabel = new();

        private Button sendButton = new();
        private Button serverButton = new();
        private Button connectButton = new();
        private Button disconnectButton = new();

        private FlowLayoutPanel messagesFlow = new();

        private Panel chatPanel = new();
        private Panel composerPanel = new();

        // =========================================
        // CONSTRUCTOR
        // =========================================

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        // =========================================
        // MAIN INTERFACE
        // =========================================

        private void BuildInterface()
        {
            Text = "Nexus Chat";
            Size = new Size(1250, 800);
            MinimumSize = new Size(950, 760);

            StartPosition = FormStartPosition.CenterScreen;
            BackColor = backgroundColor;
            ForeColor = textColor;
            Font = new Font("Segoe UI", 10);

            Controls.Clear();

            // =====================================
            // LEFT NAVIGATION
            // =====================================

            Panel navigation = new Panel
            {
                Dock = DockStyle.Left,
                Width = 76,
                BackColor = backgroundColor
            };

            Label logo = new Label
            {
                Text = "N",
                Font = new Font("Segoe UI", 23, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = accentColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(48, 48),
                Location = new Point(14, 22),
                Cursor = Cursors.Hand
            };

            RoundControl(logo, 13);

            Label messagesIcon =
                CreateNavigationItem("M", 120, true);

            Label usersIcon =
                CreateNavigationItem("U", 182, false);

            Label settingsIcon =
                CreateNavigationItem("S", 244, false);

            Label profile = new Label
            {
                Text = "YU",
                Size = new Size(44, 44),
                BackColor = panelColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(16, 0),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            RoundControl(profile, 22);

            logo.Click += (s, e) => ShowAboutDialog();
            messagesIcon.Click += (s, e) => messageInput.Focus();
            usersIcon.Click += (s, e) => ShowUsersDialog();
            settingsIcon.Click += (s, e) => ShowSettingsDialog();
            profile.Click += (s, e) => ShowProfileDialog();

            navigation.Resize += (s, e) =>
            {
                profile.Top = navigation.ClientSize.Height - 62;
            };

            navigation.Controls.Add(logo);
            navigation.Controls.Add(messagesIcon);
            navigation.Controls.Add(usersIcon);
            navigation.Controls.Add(settingsIcon);
            navigation.Controls.Add(profile);

            // =====================================
            // CONNECTION SIDEBAR
            // =====================================

            Panel contactsPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 295,
                BackColor = sidebarColor
            };

            Label brandTitle = new Label
            {
                Text = "NEXUS",
                Font = new Font("Segoe UI", 21, FontStyle.Bold),
                ForeColor = textColor,
                Location = new Point(20, 22),
                AutoSize = true
            };

            Label brandSubtitle = new Label
            {
                Text = "CHAT APPLICATION",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = mutedColor,
                Location = new Point(23, 57),
                AutoSize = true
            };

            Panel divider = new Panel
            {
                Location = new Point(20, 91),
                Size = new Size(250, 1),
                BackColor = Color.FromArgb(52, 64, 88)
            };

            Label connectionHeading = new Label
            {
                Text = "Bağlantı Ayarları",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = textColor,
                Location = new Point(20, 115),
                AutoSize = true
            };

            Label connectionDescription = new Label
            {
                Text = "Sunucu bilgilerini girerek bağlantı kurun.",
                Font = new Font("Segoe UI", 8),
                ForeColor = mutedColor,
                Location = new Point(21, 145),
                AutoSize = true
            };

            // Connection card

            Panel connectionCard = new Panel
            {
                Location = new Point(15, 185),
                Size = new Size(265, 405),
                BackColor = panelColor
            };

            RoundControl(connectionCard, 14);

            Label usernameLabel =
                CreateSmallLabel("Kullanıcı Adı", 18);

            usernameInput =
                CreateTextBox("Yasemin", 42, 225);

            Label ipLabel =
                CreateSmallLabel("IP Adresi", 100);

            ipInput =
                CreateTextBox("127.0.0.1", 124, 225);

            Label portLabel =
                CreateSmallLabel("Port Numarası", 182);

            portInput =
                CreateTextBox("5055", 206, 225);

            serverButton = CreateButton(
                "Sunucuyu Başlat",
                accentColor,
                255,
                225,
                39
            );

            connectButton = CreateButton(
                "Bağlan",
                successColor,
                302,
                225,
                39
            );

            disconnectButton = CreateButton(
                "Bağlantıyı Kes",
                dangerColor,
                349,
                225,
                39
            );

            disconnectButton.Enabled = false;

            serverButton.Click += async (s, e) =>
                await StartServerAsync();

            connectButton.Click += async (s, e) =>
                await ConnectToServerAsync();

            disconnectButton.Click += (s, e) =>
                DisconnectFromServer();

            connectionCard.Controls.Add(usernameLabel);
            connectionCard.Controls.Add(usernameInput);
            connectionCard.Controls.Add(ipLabel);
            connectionCard.Controls.Add(ipInput);
            connectionCard.Controls.Add(portLabel);
            connectionCard.Controls.Add(portInput);
            connectionCard.Controls.Add(serverButton);
            connectionCard.Controls.Add(connectButton);
            connectionCard.Controls.Add(disconnectButton);

            // Status card

            Panel statusCard = new Panel
            {
                Location = new Point(15, 0),
                Size = new Size(265, 88),
                BackColor = panelColor,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            RoundControl(statusCard, 12);

            Label statusHeading = new Label
            {
                Text = "BAĞLANTI DURUMU",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = mutedColor,
                Location = new Point(15, 13),
                AutoSize = true
            };

            statusLabel = new Label
            {
                Text = "● Bağlantı Yok",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = dangerColor,
                Location = new Point(15, 43),
                AutoSize = true
            };

            statusCard.Controls.Add(statusHeading);
            statusCard.Controls.Add(statusLabel);

            contactsPanel.Resize += (s, e) =>
            {
                statusCard.Top =
                    contactsPanel.ClientSize.Height -
                    statusCard.Height - 20;
            };

            contactsPanel.Controls.Add(brandTitle);
            contactsPanel.Controls.Add(brandSubtitle);
            contactsPanel.Controls.Add(divider);
            contactsPanel.Controls.Add(connectionHeading);
            contactsPanel.Controls.Add(connectionDescription);
            contactsPanel.Controls.Add(connectionCard);
            contactsPanel.Controls.Add(statusCard);

            // =====================================
            // MAIN CHAT PANEL
            // =====================================

            chatPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = backgroundColor
            };

            // =====================================
            // CHAT HEADER
            // =====================================

            Panel chatHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = sidebarColor
            };

            Label chatTitle = new Label
            {
                Text = "Doğrudan Mesajlaşma",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = textColor,
                Location = new Point(25, 17),
                AutoSize = true
            };

            Label chatSubtitle = new Label
            {
                Text = "TCP tabanlı güvenilir bağlantı",
                Font = new Font("Segoe UI", 9),
                ForeColor = mutedColor,
                Location = new Point(27, 49),
                AutoSize = true
            };

            Panel headerStatusCard = new Panel
            {
                Size = new Size(145, 38),
                BackColor = panelColor,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            RoundControl(headerStatusCard, 12);

            headerStatusLabel = new Label
            {
                Text = "● Offline",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = dangerColor,
                AutoSize = true,
                Location = new Point(14, 11)
            };

            headerStatusCard.Controls.Add(headerStatusLabel);

            chatHeader.Controls.Add(chatTitle);
            chatHeader.Controls.Add(chatSubtitle);
            chatHeader.Controls.Add(headerStatusCard);

            chatHeader.Resize += (s, e) =>
            {
                headerStatusCard.Left =
                    chatHeader.ClientSize.Width -
                    headerStatusCard.Width - 25;

                headerStatusCard.Top = 26;
            };

            // =====================================
            // MESSAGE AREA
            // =====================================

            Panel messagesPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = backgroundColor,
                Padding = new Padding(18, 15, 18, 10)
            };

            messagesFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = backgroundColor,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(5)
            };

            messagesFlow.SizeChanged +=
                (s, e) => ResizeMessageRows();

            messagesPanel.Controls.Add(messagesFlow);

            // =====================================
            // MESSAGE COMPOSER
            // =====================================

            composerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 115,
                BackColor = sidebarColor,
                Padding = new Padding(20)
            };

            Panel inputContainer = new Panel
            {
                Location = new Point(20, 17),
                Height = 68,
                BackColor = inputColor,
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            RoundControl(inputContainer, 13);

            messageInput = new TextBox
            {
                Multiline = true,
                AcceptsReturn = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.None,
                BackColor = inputColor,
                ForeColor = textColor,
                Font = new Font("Segoe UI", 11),
                Location = new Point(14, 13),
                Size = new Size(400, 43)
            };

            sendButton = new Button
            {
                Text = "Gönder  ➤",
                Size = new Size(115, 42),
                BackColor = accentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.FlatAppearance.MouseOverBackColor = accentHover;

            RoundControl(sendButton, 10);

            inputContainer.Controls.Add(messageInput);
            inputContainer.Controls.Add(sendButton);

            inputContainer.Resize += (s, e) =>
            {
                sendButton.Left =
                    inputContainer.ClientSize.Width -
                    sendButton.Width - 10;

                sendButton.Top = 13;

                messageInput.Width =
                    Math.Max(100, sendButton.Left - 25);

                messageInput.Height =
                    inputContainer.ClientSize.Height - 24;
            };

            composerPanel.Resize += (s, e) =>
            {
                inputContainer.Width =
                    composerPanel.ClientSize.Width - 40;
            };

            messageInput.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    e.SuppressKeyPress = true;
                    await SendMessageAsync();
                }
            };

            sendButton.Click += async (s, e) =>
                await SendMessageAsync();

            Label composerHint = new Label
            {
                Text = "Enter: Gönder     Shift + Enter: Yeni Satır",
                ForeColor = mutedColor,
                Font = new Font("Segoe UI", 8),
                Location = new Point(22, 87),
                AutoSize = true
            };

            composerPanel.Controls.Add(inputContainer);
            composerPanel.Controls.Add(composerHint);

            chatPanel.Controls.Add(messagesPanel);
            chatPanel.Controls.Add(composerPanel);
            chatPanel.Controls.Add(chatHeader);

            Controls.Add(chatPanel);
            Controls.Add(contactsPanel);
            Controls.Add(navigation);

            AddSystemMessage(
                "Nexus Chat'e hoş geldiniz. " +
                "Bağlantı kurmak için sol paneli kullanabilirsiniz."
            );
        }

        // =========================================
        // ROUND CORNERS
        // =========================================

        private void RoundControl(Control control, int radius)
        {
            void ApplyRoundedRegion()
            {
                if (control.Width <= 0 || control.Height <= 0)
                    return;

                using GraphicsPath path = new GraphicsPath();

                int diameter = radius * 2;

                path.AddArc(0, 0, diameter, diameter, 180, 90);
                path.AddArc(
                    control.Width - diameter,
                    0,
                    diameter,
                    diameter,
                    270,
                    90
                );
                path.AddArc(
                    control.Width - diameter,
                    control.Height - diameter,
                    diameter,
                    diameter,
                    0,
                    90
                );
                path.AddArc(
                    0,
                    control.Height - diameter,
                    diameter,
                    diameter,
                    90,
                    90
                );

                path.CloseFigure();

                Region? oldRegion = control.Region;
                control.Region = new Region(path);
                oldRegion?.Dispose();
            }

            control.Resize += (s, e) => ApplyRoundedRegion();
            ApplyRoundedRegion();
        }

        // =========================================
        // UI HELPERS
        // =========================================

        private Label CreateNavigationItem(
            string text,
            int top,
            bool selected)
        {
            Label item = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = selected ? Color.White : mutedColor,
                BackColor = selected ? panelColor : backgroundColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(48, 48),
                Location = new Point(14, top),
                Cursor = Cursors.Hand
            };

            RoundControl(item, 12);

            item.MouseEnter += (s, e) =>
            {
                item.BackColor = panelColor;
                item.ForeColor = Color.White;
            };

            item.MouseLeave += (s, e) =>
            {
                item.BackColor = selected
                    ? panelColor
                    : backgroundColor;

                item.ForeColor = selected
                    ? Color.White
                    : mutedColor;
            };

            return item;
        }

        private Label CreateSmallLabel(string text, int top)
        {
            return new Label
            {
                Text = text,
                ForeColor = mutedColor,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(17, top)
            };
        }

        private TextBox CreateTextBox(
            string value,
            int top,
            int width)
        {
            return new TextBox
            {
                Text = value,
                Location = new Point(17, top),
                Size = new Size(width, 30),
                BackColor = inputColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
        }

        private Button CreateButton(
            string text,
            Color color,
            int top,
            int width,
            int height)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(17, top),
                Size = new Size(width, height),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor =
                ControlPaint.Light(color, 0.12f);

            RoundControl(button, 9);

            return button;
        }

        // =========================================
        // TCP SERVER
        // =========================================

        private async Task StartServerAsync()
        {
            if (chatServer != null || connected || chatClient != null)
            {
                AddSystemMessage("Sunucu veya bağlantı zaten aktif.");
                return;
            }

            if (!int.TryParse(portInput.Text, out int port) ||
                port < 1 || port > 65535)
            {
                MessageBox.Show(
                    "Lütfen geçerli bir port numarası girin.",
                    "Geçersiz Port",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            ChatServer newServer = new ChatServer();
            CancellationTokenSource newCts =
                new CancellationTokenSource();

            chatServer = newServer;
            connectionCts = newCts;

            serverButton.Enabled = false;
            connectButton.Enabled = false;

            try
            {
                AddSystemMessage(
                    "Sunucu başlatıldı. Kullanıcı bağlantısı bekleniyor..."
                );

                UpdateStatus("● Kullanıcı Bekleniyor", Color.Gold);

                ChatConnection acceptedConnection =
                    await newServer.StartAsync(port, newCts.Token);

                if (closing ||
                    newCts.IsCancellationRequested ||
                    !ReferenceEquals(chatServer, newServer))
                {
                    acceptedConnection.Dispose();
                    return;
                }

                chatConnection = acceptedConnection;
                connected = true;

                UpdateStatus("● Bağlandı", successColor);

                disconnectButton.Enabled = true;

                AddSystemMessage("Bir kullanıcı bağlandı.");

                _ = ReceiveMessagesAsync(
                    acceptedConnection,
                    newCts.Token
                );
            }
            catch (Exception ex)
            {
                if (!newCts.IsCancellationRequested && !closing)
                {
                    AddSystemMessage("Sunucu hatası: " + ex.Message);
                    UpdateStatus("● Bağlantı Yok", dangerColor);
                }

                if (ReferenceEquals(chatServer, newServer))
                {
                    chatServer = null;
                    connectionCts = null;

                    newServer.Dispose();
                    newCts.Dispose();

                    serverButton.Enabled = true;
                    connectButton.Enabled = true;
                }
            }
        }

        // =========================================
        // TCP CLIENT
        // =========================================

        private async Task ConnectToServerAsync()
        {
            if (connected ||
                chatClient != null ||
                chatServer != null)
            {
                AddSystemMessage(
                    "Zaten bir bağlantı işlemi devam ediyor."
                );
                return;
            }

            if (!int.TryParse(portInput.Text, out int port) ||
                port < 1 || port > 65535)
            {
                MessageBox.Show(
                    "Lütfen geçerli bir port numarası girin.",
                    "Geçersiz Port",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (!IPAddress.TryParse(
                ipInput.Text.Trim(),
                out IPAddress? address))
            {
                MessageBox.Show(
                    "Lütfen geçerli bir IP adresi girin.",
                    "Geçersiz IP",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            ChatClient newClient = new ChatClient();
            CancellationTokenSource newCts =
                new CancellationTokenSource();

            chatClient = newClient;
            connectionCts = newCts;

            connectButton.Enabled = false;
            serverButton.Enabled = false;

            try
            {
                AddSystemMessage("Sunucuya bağlanılıyor...");
                UpdateStatus("● Bağlanıyor", Color.Gold);

                ChatConnection newConnection =
                    await newClient.ConnectAsync(
                        address.ToString(),
                        port,
                        newCts.Token
                    );

                if (closing ||
                    newCts.IsCancellationRequested ||
                    !ReferenceEquals(chatClient, newClient))
                {
                    newConnection.Dispose();
                    return;
                }

                chatConnection = newConnection;
                connected = true;

                UpdateStatus("● Bağlandı", successColor);

                disconnectButton.Enabled = true;

                AddSystemMessage("Sunucuya başarıyla bağlanıldı.");

                _ = ReceiveMessagesAsync(
                    newConnection,
                    newCts.Token
                );
            }
            catch (Exception ex)
            {
                if (!newCts.IsCancellationRequested && !closing)
                {
                    AddSystemMessage("Bağlantı hatası: " + ex.Message);
                    UpdateStatus("● Bağlantı Yok", dangerColor);
                }

                if (ReferenceEquals(chatClient, newClient))
                {
                    chatClient = null;
                    connectionCts = null;

                    newClient.Dispose();
                    newCts.Dispose();

                    serverButton.Enabled = true;
                    connectButton.Enabled = true;
                }
            }
        }

        // =========================================
        // RECEIVE MESSAGES
        // =========================================

        private async Task ReceiveMessagesAsync(
            ChatConnection currentConnection,
            CancellationToken cancellationToken)
        {
            try
            {
                while (!closing &&
                       !cancellationToken.IsCancellationRequested)
                {
                    ChatMessage? message =
                        await currentConnection.ReceiveAsync(
                            cancellationToken
                        );

                    if (message == null)
                        break;

                    AddIncomingMessage(message);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal connection cancellation.
            }
            catch (ObjectDisposedException)
            {
                // Connection already disposed.
            }
            catch (Exception ex)
            {
                if (!closing &&
                    !cancellationToken.IsCancellationRequested)
                {
                    AddSystemMessage(
                        "Mesaj alma hatası: " + ex.Message
                    );
                }
            }
            finally
            {
                if (ReferenceEquals(
                    chatConnection,
                    currentConnection))
                {
                    CleanupConnection(currentConnection);
                }
            }
        }

        // =========================================
        // SEND MESSAGE
        // =========================================

        private async Task SendMessageAsync()
        {
            string messageText = messageInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(messageText))
                return;

            if (!connected || chatConnection == null)
            {
                MessageBox.Show(
                    "Lütfen önce bir sunucuya bağlanın.",
                    "Bağlantı Yok",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string username = usernameInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Lütfen kullanıcı adınızı girin.");
                return;
            }

            try
            {
                ChatMessage chatMessage =
                    ChatMessage.Create(username, messageText);

                ChatConnection currentConnection = chatConnection;

                await currentConnection.SendAsync(chatMessage);

                if (!ReferenceEquals(
                    chatConnection,
                    currentConnection))
                {
                    return;
                }

                AddChatMessage(chatMessage, true);

                messageInput.Clear();
                messageInput.Focus();
            }
            catch (Exception ex)
            {
                AddSystemMessage(
                    "Mesaj gönderme hatası: " + ex.Message
                );
            }
        }

        // =========================================
        // CONNECTION CLEANUP
        // =========================================

        private void CleanupConnection(
            ChatConnection currentConnection)
        {
            if (!ReferenceEquals(
                chatConnection,
                currentConnection))
            {
                return;
            }

            chatConnection = null;
            connected = false;

            CancellationTokenSource? oldCts = connectionCts;

            ChatServer? oldServer = chatServer;
            ChatClient? oldClient = chatClient;

            connectionCts = null;
            chatServer = null;
            chatClient = null;

            currentConnection.Dispose();

            oldServer?.Dispose();
            oldClient?.Dispose();
            oldCts?.Dispose();

            if (!closing && !IsDisposed)
            {
                UpdateStatus("● Bağlantı Yok", dangerColor);

                AddSystemMessage("Bağlantı kapatıldı.");

                serverButton.Enabled = true;
                connectButton.Enabled = true;
                disconnectButton.Enabled = false;
            }
        }

        // =========================================
        // INCOMING MESSAGE
        // =========================================

        private void AddIncomingMessage(ChatMessage message)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action(() => AddIncomingMessage(message))
                );
                return;
            }

            AddChatMessage(message, false);
        }

        // =========================================
        // DISPLAY CHAT MESSAGE
        // =========================================

        private void AddChatMessage(
            ChatMessage message,
            bool ownMessage)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action(() =>
                        AddChatMessage(message, ownMessage))
                );
                return;
            }

            int availableWidth =
                Math.Max(250, messagesFlow.ClientSize.Width - 55);

            Panel row = new Panel
            {
                Width = availableWidth,
                Height = 85,
                BackColor = backgroundColor,
                Margin = new Padding(0, 0, 0, 13),
                Tag = ownMessage
            };

            Label senderLabel = new Label
            {
                Text = ownMessage ? "Sen" : message.Sender,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = ownMessage ? accentHover : mutedColor,
                AutoSize = true,
                Location = new Point(12, 0)
            };

            Label timeLabel = new Label
            {
                Text = message.TimestampUtc
                    .ToLocalTime()
                    .ToString("HH:mm"),
                Font = new Font("Segoe UI", 8),
                ForeColor = mutedColor,
                AutoSize = true,
                Location = new Point(100, 1)
            };

            Panel bubble = new Panel
            {
                BackColor = ownMessage
                    ? accentColor
                    : panelColor,
                Location = new Point(10, 24),
                Padding = new Padding(14, 9, 14, 9),
                AutoSize = true,
                MaximumSize = new Size(
                    Math.Max(200, availableWidth - 100),
                    0
                )
            };

            RoundControl(bubble, 12);

            Label messageLabel = new Label
            {
                Text = message.Text,
                AutoSize = true,
                MaximumSize = new Size(
                    Math.Max(170, availableWidth - 130),
                    0
                ),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.Transparent
            };

            bubble.Controls.Add(messageLabel);

            bubble.Size = new Size(
                messageLabel.Width + bubble.Padding.Horizontal,
                messageLabel.Height + bubble.Padding.Vertical
            );

            row.Controls.Add(senderLabel);
            row.Controls.Add(timeLabel);
            row.Controls.Add(bubble);

            row.Height = bubble.Height + 32;

            if (ownMessage)
            {
                senderLabel.Left =
                    Math.Max(10, row.Width - senderLabel.Width - 15);

                timeLabel.Left =
                    Math.Max(10, senderLabel.Left - timeLabel.Width - 10);

                bubble.Left =
                    Math.Max(10, row.Width - bubble.Width - 10);
            }
            else
            {
                senderLabel.Left = 10;
                timeLabel.Left = senderLabel.Right + 12;
                bubble.Left = 10;
            }

            messagesFlow.Controls.Add(row);
            messagesFlow.ScrollControlIntoView(row);
        }

        // =========================================
        // SYSTEM MESSAGE
        // =========================================

        private void AddSystemMessage(string message)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action(() => AddSystemMessage(message))
                );
                return;
            }

            Label label = new Label
            {
                Text = "—  " + message + "  —",
                AutoSize = true,
                ForeColor = mutedColor,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                Margin = new Padding(5, 8, 5, 15)
            };

            messagesFlow.Controls.Add(label);
            messagesFlow.ScrollControlIntoView(label);
        }

        // =========================================
        // RESIZE MESSAGE ROWS
        // =========================================

        private void ResizeMessageRows()
        {
            if (messagesFlow.IsDisposed)
                return;

            int availableWidth =
                Math.Max(250, messagesFlow.ClientSize.Width - 55);

            foreach (Control control in messagesFlow.Controls)
            {
                if (control is not Panel row ||
                    row.Controls.Count < 3)
                {
                    continue;
                }

                Panel? bubble = null;
                Label? senderLabel = null;
                Label? timeLabel = null;

                foreach (Control child in row.Controls)
                {
                    if (child is Panel panel)
                        bubble = panel;
                    else if (child is Label label)
                    {
                        if (senderLabel == null)
                            senderLabel = label;
                        else
                            timeLabel = label;
                    }
                }

                if (bubble == null ||
                    senderLabel == null ||
                    timeLabel == null)
                {
                    continue;
                }

                row.Width = availableWidth;

                Label? messageLabel = null;

                foreach (Control child in bubble.Controls)
                {
                    if (child is Label label)
                    {
                        messageLabel = label;
                        break;
                    }
                }

                if (messageLabel == null)
                    continue;

                messageLabel.MaximumSize = new Size(
                    Math.Max(170, availableWidth - 130),
                    0
                );

                bubble.Size = new Size(
                    messageLabel.Width + bubble.Padding.Horizontal,
                    messageLabel.Height + bubble.Padding.Vertical
                );

                bool ownMessage =
                    row.Tag is bool value && value;

                if (ownMessage)
                {
                    senderLabel.Left =
                        Math.Max(10, row.Width - senderLabel.Width - 15);

                    timeLabel.Left =
                        Math.Max(10, senderLabel.Left - timeLabel.Width - 10);

                    bubble.Left =
                        Math.Max(10, row.Width - bubble.Width - 10);
                }
                else
                {
                    senderLabel.Left = 10;
                    timeLabel.Left = senderLabel.Right + 12;
                    bubble.Left = 10;
                }

                bubble.Top = 24;
                row.Height = bubble.Height + 32;
            }
        }

        // =========================================
        // UPDATE STATUS
        // =========================================

        private void UpdateStatus(string text, Color color)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action(() => UpdateStatus(text, color))
                );
                return;
            }

            statusLabel.Text = text;
            statusLabel.ForeColor = color;

            headerStatusLabel.Text = connected
                ? "● Online"
                : text.Contains("Bekleniyor")
                    ? "● Bekleniyor"
                    : "● Offline";

            headerStatusLabel.ForeColor = color;
        }

        // =========================================
        // DISCONNECT
        // =========================================

        private void DisconnectFromServer()
        {
            connected = false;

            ChatConnection? oldConnection = chatConnection;
            ChatServer? oldServer = chatServer;
            ChatClient? oldClient = chatClient;

            CancellationTokenSource? oldCts = connectionCts;

            chatConnection = null;
            chatServer = null;
            chatClient = null;
            connectionCts = null;

            try
            {
                oldCts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed.
            }

            oldConnection?.Dispose();
            oldServer?.Dispose();
            oldClient?.Dispose();
            oldCts?.Dispose();

            serverButton.Enabled = true;
            connectButton.Enabled = true;
            disconnectButton.Enabled = false;

            UpdateStatus("● Bağlantı Yok", dangerColor);

            AddSystemMessage("Bağlantıyı kestiniz.");
        }

        // =========================================
        // ABOUT DIALOG
        // =========================================

        private void ShowAboutDialog()
        {
            Form dialog = CreateDialog(
                "Nexus Chat Hakkında",
                430,
                310
            );

            Label logo = new Label
            {
                Text = "N",
                Size = new Size(65, 65),
                Location = new Point(175, 25),
                BackColor = accentColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            RoundControl(logo, 15);

            Label title = CreateDialogLabel(
                "NEXUS CHAT",
                130,
                110,
                19,
                true
            );

            Label description = CreateDialogLabel(
                "TCP tabanlı masaüstü mesajlaşma uygulaması.\n\n" +
                "C# ve Windows Forms ile geliştirilmiştir.\n\n" +
                "YMH 219 - Nesne Tabanlı Programlama",
                45,
                155
            );

            dialog.Controls.Add(logo);
            dialog.Controls.Add(title);
            dialog.Controls.Add(description);

            dialog.ShowDialog(this);
        }

        // =========================================
        // USERS DIALOG
        // =========================================

        private void ShowUsersDialog()
        {
            Form dialog = CreateDialog(
                "Kullanıcılar",
                420,
                300
            );

            Label title = CreateDialogLabel(
                "Bağlantı Bilgileri",
                25,
                25,
                16,
                true
            );

            Label localUser = CreateDialogLabel(
                "Yerel Kullanıcı: " + usernameInput.Text,
                25,
                90
            );

            Label connection = CreateDialogLabel(
                connected
                    ? "● Karşı Kullanıcı: Bağlandı"
                    : "● Karşı Kullanıcı: Bağlı Değil",
                25,
                130
            );

            connection.ForeColor =
                connected ? successColor : dangerColor;

            Label description = CreateDialogLabel(
                "Nexus Chat şu anda tek bir doğrudan\n" +
                "TCP bağlantısını desteklemektedir.",
                25,
                185
            );

            dialog.Controls.Add(title);
            dialog.Controls.Add(localUser);
            dialog.Controls.Add(connection);
            dialog.Controls.Add(description);

            dialog.ShowDialog(this);
        }

        // =========================================
        // PROFILE DIALOG
        // =========================================

        private void ShowProfileDialog()
        {
            Form dialog = CreateDialog(
                "Profilim",
                410,
                290
            );

            Label title = CreateDialogLabel(
                "Profili Düzenle",
                25,
                25,
                16,
                true
            );

            Label nameLabel = CreateDialogLabel(
                "Kullanıcı Adı",
                25,
                90
            );

            TextBox nameInput = new TextBox
            {
                Text = usernameInput.Text,
                Location = new Point(25, 120),
                Width = 340,
                BackColor = inputColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };

            Button save = CreateDialogButton(
                "Profili Kaydet",
                235,
                185
            );

            save.Click += (s, e) =>
            {
                string newName = nameInput.Text.Trim();

                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show(
                        "Kullanıcı adı boş bırakılamaz."
                    );
                    return;
                }

                if (newName.Length > 40)
                {
                    MessageBox.Show(
                        "Kullanıcı adı en fazla 40 karakter olabilir."
                    );
                    return;
                }

                usernameInput.Text = newName;

                AddSystemMessage(
                    "Profil güncellendi: " + newName
                );

                dialog.Close();
            };

            dialog.Controls.Add(title);
            dialog.Controls.Add(nameLabel);
            dialog.Controls.Add(nameInput);
            dialog.Controls.Add(save);

            dialog.ShowDialog(this);
        }

        // =========================================
        // SETTINGS DIALOG
        // =========================================

        private void ShowSettingsDialog()
        {
            Form dialog = CreateDialog(
                "Ayarlar",
                430,
                410
            );

            Label title = CreateDialogLabel(
                "Bağlantı Ayarları",
                25,
                25,
                16,
                true
            );

            Label ipLabel = CreateDialogLabel(
                "Sunucu IP Adresi",
                25,
                90
            );

            TextBox ip = new TextBox
            {
                Text = ipInput.Text,
                Location = new Point(25, 120),
                Width = 345,
                BackColor = inputColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label portLabel = CreateDialogLabel(
                "Port Numarası",
                25,
                175
            );

            TextBox port = new TextBox
            {
                Text = portInput.Text,
                Location = new Point(25, 205),
                Width = 345,
                BackColor = inputColor,
                ForeColor = textColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label note = CreateDialogLabel(
                connected || chatServer != null || chatClient != null
                    ? "Yeni ayarları uygulamadan önce bağlantıyı kesin."
                    : "Değişiklikler bağlantı alanlarına uygulanır.",
                25,
                255
            );

            note.ForeColor = mutedColor;

            Button save = CreateDialogButton(
                "Ayarları Kaydet",
                240,
                305
            );

            save.Click += (s, e) =>
            {
                if (connected ||
                    chatServer != null ||
                    chatClient != null)
                {
                    MessageBox.Show(
                        "Önce bağlantı işlemini kesin."
                    );
                    return;
                }

                if (!IPAddress.TryParse(ip.Text.Trim(), out _))
                {
                    MessageBox.Show(
                        "Lütfen geçerli bir IP adresi girin."
                    );
                    return;
                }

                if (!int.TryParse(
                    port.Text,
                    out int portNumber) ||
                    portNumber < 1 ||
                    portNumber > 65535)
                {
                    MessageBox.Show(
                        "Lütfen geçerli bir port numarası girin."
                    );
                    return;
                }

                ipInput.Text = ip.Text.Trim();
                portInput.Text = portNumber.ToString();

                AddSystemMessage(
                    "Bağlantı ayarları güncellendi."
                );

                dialog.Close();
            };

            dialog.Controls.Add(title);
            dialog.Controls.Add(ipLabel);
            dialog.Controls.Add(ip);
            dialog.Controls.Add(portLabel);
            dialog.Controls.Add(port);
            dialog.Controls.Add(note);
            dialog.Controls.Add(save);

            dialog.ShowDialog(this);
        }

        // =========================================
        // DIALOG HELPERS
        // =========================================

        private Form CreateDialog(
            string title,
            int width,
            int height)
        {
            return new Form
            {
                Text = title,
                Size = new Size(width, height),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = sidebarColor,
                ForeColor = textColor,
                Font = new Font("Segoe UI", 10)
            };
        }

        private Label CreateDialogLabel(
            string text,
            int left,
            int top,
            int fontSize = 10,
            bool bold = false)
        {
            return new Label
            {
                Text = text,
                ForeColor = textColor,
                Location = new Point(left, top),
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    fontSize,
                    bold ? FontStyle.Bold : FontStyle.Regular
                )
            };
        }

        private Button CreateDialogButton(
            string text,
            int left,
            int top)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(130, 40),
                BackColor = accentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = accentHover;

            RoundControl(button, 9);

            return button;
        }

        // =========================================
        // FORM CLOSING
        // =========================================

        protected override void OnFormClosing(
            FormClosingEventArgs e)
        {
            closing = true;
            connected = false;

            ChatConnection? oldConnection = chatConnection;
            ChatServer? oldServer = chatServer;
            ChatClient? oldClient = chatClient;

            CancellationTokenSource? oldCts = connectionCts;

            chatConnection = null;
            chatServer = null;
            chatClient = null;
            connectionCts = null;

            try
            {
                oldCts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed.
            }

            oldConnection?.Dispose();
            oldServer?.Dispose();
            oldClient?.Dispose();
            oldCts?.Dispose();

            base.OnFormClosing(e);
        }
    }
}
