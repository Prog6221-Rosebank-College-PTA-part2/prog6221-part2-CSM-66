using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatBOT.ChatBot
{
    public partial class MainForm : Form
    {
        private ChatbotEngine _engine = new ChatbotEngine();
        private bool _nameEntered = false;

        //Controls
        private Panel pnlHeader = new Panel();
        private Label lblAsciiArt = new Label();
        private Label lblSubtitle = new Label();

        private Panel pnlMemory = new Panel();
        private Label lblMemoryInfo = new Label();

        private RichTextBox rtbChat = new RichTextBox();

        private Panel pnlInput = new Panel();
        private TextBox txtInput = new TextBox();
        private Button btnSend = new Button();

        private FlowLayoutPanel pnlTopics = new FlowLayoutPanel();
        private Label lblTopicsHeader = new Label();

        public MainForm()
        {
            InitializeComponent();
            BuildUI();
            PlayGreeting();
            ShowWelcomePrompt();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "CyberSecurity Awareness Bot";
            this.Size = new Size(900, 720);
            this.MinimumSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(13, 17, 23);
            this.ForeColor = Color.White;
            this.Font = new Font("Consolas", 9.5f);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void BuildUI()
        {
            //HEADER PANEL 
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 200;
            pnlHeader.BackColor = Color.FromArgb(20, 26, 34);
            pnlHeader.Padding = new Padding(10, 6, 10, 6);

            lblAsciiArt.AutoSize = false;
            lblAsciiArt.Dock = DockStyle.Top;
            lblAsciiArt.Height = 100;
            lblAsciiArt.Font = new Font("Consolas", 5f, FontStyle.Bold);
            lblAsciiArt.ForeColor = Color.FromArgb(0, 200, 120);
            lblAsciiArt.TextAlign = ContentAlignment.MiddleLeft;
            lblAsciiArt.Text = @"
_________        ___.                 _________                          .__  __              _____                                                             __________        __   
\_   ___ \___.__.\_ |__   ___________/   _____/ ____   ____  __ _________|__|/  |_ ___.__.   /  _  \__  _  _______ _______   ____   ____   ____   ______ ______ \______   \ _____/  |_ 
/    \  \<   |  | | __ \_/ __ \_  __ \_____  \_/ __ \_/ ___\|  |  \_  __ \  \   __<   |  |  /  /_\  \ \/ \/ /\__  \\_  __ \_/ __ \ /    \_/ __ \ /  ___//  ___/  |    |  _//  _ \   __\
\     \___\___  | | \_\ \  ___/|  | \/        \  ___/\  \___|  |  /|  | \/  ||  |  \___  | /    |    \     /  / __ \|  | \/\  ___/|   |  \  ___/ \___ \ \___ \   |    |   (  <_> )  |  
 \______  / ____| |___  /\___  >__| /_______  /\___  >\___  >____/ |__|  |__||__|  / ____| \____|__  /\/\_/  (____  /__|    \___  >___|  /\___  >____  >____  >  |______  /\____/|__|  
        \/\/          \/     \/             \/     \/     \/                       \/              \/             \/            \/     \/     \/     \/     \/          \/             
";


            lblSubtitle.AutoSize = false;
            lblSubtitle.Dock = DockStyle.Top;
            lblSubtitle.Height = 26;
            lblSubtitle.Font = new Font("Consolas", 9f);
            lblSubtitle.ForeColor = Color.FromArgb(120, 180, 160);
            lblSubtitle.TextAlign = ContentAlignment.MiddleLeft;
            lblSubtitle.Padding = new Padding(10, 0, 0, 0);
            lblSubtitle.Text = "Your personal guide to staying safe online";

            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblAsciiArt);

            //MEMORY INFO PANEL 
            pnlMemory.Dock = DockStyle.Top;
            pnlMemory.Height = 28;
            pnlMemory.BackColor = Color.FromArgb(15, 40, 30);
            pnlMemory.Padding = new Padding(10, 4, 10, 4);

            lblMemoryInfo.Dock = DockStyle.Fill;
            lblMemoryInfo.Font = new Font("Consolas", 8.5f);
            lblMemoryInfo.ForeColor = Color.FromArgb(80, 200, 140);
            lblMemoryInfo.TextAlign = ContentAlignment.MiddleLeft;
            lblMemoryInfo.Text = "Memory: No user info stored yet.";
            pnlMemory.Controls.Add(lblMemoryInfo);

            //TOPIC QUICK-BUTTONS
            Panel pnlTopicContainer = new Panel();
            pnlTopicContainer.Dock = DockStyle.Bottom;
            pnlTopicContainer.Height = 70;
            pnlTopicContainer.BackColor = Color.FromArgb(16, 22, 30);
            pnlTopicContainer.Padding = new Padding(8, 4, 8, 4);

            lblTopicsHeader.AutoSize = true;
            lblTopicsHeader.Font = new Font("Consolas", 8f);
            lblTopicsHeader.ForeColor = Color.FromArgb(100, 160, 130);
            lblTopicsHeader.Text = "Quick topics:";
            lblTopicsHeader.Location = new Point(8, 6);

            pnlTopics.Location = new Point(8, 22);
            pnlTopics.AutoSize = true;
            pnlTopics.FlowDirection = FlowDirection.LeftToRight;
            pnlTopics.WrapContents = false;
            pnlTopics.AutoScroll = false;

            string[] topics = { "Password", "Phishing", "Scam", "Privacy", "Malware", "Safe Browsing", "VPN", "Two Factor" };
            Color[] btnColors = {
                Color.FromArgb(0, 120, 80),
                Color.FromArgb(180, 80, 0),
                Color.FromArgb(140, 20, 20),
                Color.FromArgb(0, 80, 160),
                Color.FromArgb(100, 0, 140),
                Color.FromArgb(0, 100, 120),
                Color.FromArgb(60, 100, 0),
                Color.FromArgb(20, 60, 140)
            };

            for (int i = 0; i < topics.Length; i++)
            {
                string topic = topics[i];
                Button btn = new Button
                {
                    Text = topic,
                    Size = new Size(110, 36),
                    Margin = new Padding(3, 2, 3, 2),
                    BackColor = btnColors[i % btnColors.Length],
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Consolas", 7.5f),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => { txtInput.Text = $"Tell me about {topic.ToLower()}"; SendMessage(); };
                pnlTopics.Controls.Add(btn);
            }

            pnlTopicContainer.Controls.Add(pnlTopics);
            pnlTopicContainer.Controls.Add(lblTopicsHeader);

            //INPUT PANEL
            pnlInput.Dock = DockStyle.Bottom;
            pnlInput.Height = 52;
            pnlInput.BackColor = Color.FromArgb(20, 26, 34);
            pnlInput.Padding = new Padding(10, 8, 10, 8);

            txtInput.Dock = DockStyle.Fill;
            txtInput.BackColor = Color.FromArgb(30, 38, 50);
            txtInput.ForeColor = Color.White;
            txtInput.BorderStyle = BorderStyle.None;
            txtInput.Font = new Font("Consolas", 11f);
            txtInput.KeyDown += TxtInput_KeyDown;
            txtInput.GotFocus += (s, e) => { if (txtInput.Text == "Type your message here...") { txtInput.Text = ""; txtInput.ForeColor = Color.White; } };
            txtInput.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtInput.Text)) { txtInput.Text = "Type your message here..."; txtInput.ForeColor = Color.Gray; } };
            txtInput.Text = "Type your message here...";
            txtInput.ForeColor = Color.Gray;

            btnSend.Dock = DockStyle.Right;
            btnSend.Width = 90;
            btnSend.Text = "SEND ▶";
            btnSend.BackColor = Color.FromArgb(0, 180, 100);
            btnSend.ForeColor = Color.Black;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Font = new Font("Consolas", 9f, FontStyle.Bold);
            btnSend.Cursor = Cursors.Hand;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += (s, e) => SendMessage();

            pnlInput.Controls.Add(txtInput);
            pnlInput.Controls.Add(btnSend);

            //CHAT DISPLAY 
            rtbChat.Dock = DockStyle.Fill;
            rtbChat.BackColor = Color.FromArgb(13, 17, 23);
            rtbChat.ForeColor = Color.White;
            rtbChat.ReadOnly = true;
            rtbChat.BorderStyle = BorderStyle.None;
            rtbChat.Font = new Font("Consolas", 10.5f);
            rtbChat.Padding = new Padding(10);
            rtbChat.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbChat.WordWrap = true;

            //COMPOSE FORM
            this.Controls.Add(rtbChat);
            this.Controls.Add(pnlInput);
            this.Controls.Add(pnlTopicContainer);
            this.Controls.Add(pnlMemory);
            this.Controls.Add(pnlHeader);
        }

        //Greeting audio
        private void PlayGreeting()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                if (File.Exists(filePath))
                {
                    SoundPlayer player = new SoundPlayer(filePath);
                    player.Play();
                }
            }
            catch { /* Silently continue if audio fails */ }
        }

        //Initial prompt
        private void ShowWelcomePrompt()
        {
            AppendLine("CyberSecurity Awareness Bot", Color.FromArgb(0, 180, 100));
            AppendLine("");
            AppendBotMessage("Hello! Before we begin, please tell me your name.");
        }

        //Handle enter key
        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        //Core send logic
        private void SendMessage()
        {
            string input = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input) || input == "Type your message here...") return;

            txtInput.Text = "";
            txtInput.ForeColor = Color.White;

            // Name entry stage
            if (!_nameEntered)
            {
                _engine.SetUserName(input);
                _nameEntered = true;
                AppendUserMessage(input);
                AppendLine("");
                AppendBotMessage($"Hello, {input}!Welcome to your CyberSecurity Awareness Bot.");
                AppendBotMessage("You can ask me about cybersecurity topics or click a quick topic button below.");
                AppendBotMessage("Type 'exit' at any time to quit.");
                AppendLine("");
                UpdateMemoryBar();
                return;
            }

            AppendUserMessage(input);

            // Process through engine
            var (response, type) = _engine.ProcessInput(input);

            AppendLine("");
            switch (type)
            {
                case ResponseType.Exit:
                    AppendBotMessage(response, Color.FromArgb(0, 220, 120));
                    AppendLine("");
                    txtInput.Enabled = false;
                    btnSend.Enabled = false;
                    break;
                case ResponseType.Sentiment:
                    AppendBotMessage(response, Color.FromArgb(255, 200, 80));
                    break;
                case ResponseType.Memory:
                    AppendBotMessage(response, Color.FromArgb(100, 180, 255));
                    break;
                case ResponseType.Error:
                    AppendBotMessage(response, Color.FromArgb(255, 100, 100));
                    break;
                default:
                    AppendBotMessage(response);
                    break;
            }
            AppendLine("");
            UpdateMemoryBar();
        }

        //Append helpers 
        private void AppendUserMessage(string text)
        {
            string name = string.IsNullOrEmpty(_engine.GetUserName()) ? "You" : _engine.GetUserName();
            AppendColoured($"[{name}]: ", Color.FromArgb(100, 200, 255));
            AppendColoured(text + "\n", Color.FromArgb(100, 200, 255));
        }

        private void AppendBotMessage(string text, Color? colour = null)
        {
            AppendColoured("[Bot]: ", Color.FromArgb(0, 220, 100));
            AppendColoured(text + "\n", colour ?? Color.FromArgb(0, 220, 100));
        }

        private void AppendLine(string text = "", Color? colour = null)
        {
            AppendColoured(text + "\n", colour ?? Color.FromArgb(60, 70, 80));
        }

        private void AppendColoured(string text, Color colour)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = colour;
            rtbChat.AppendText(text);
            rtbChat.SelectionColor = rtbChat.ForeColor;
            rtbChat.ScrollToCaret();
        }

        // Update memory status bar
        private void UpdateMemoryBar()
        {
            var mem = _engine.GetMemory();
            string info = $"Memory:  Name: {(string.IsNullOrEmpty(mem.Name) ? "—" : mem.Name)}";
            if (!string.IsNullOrEmpty(mem.FavouriteTopic))
                info += $"  |  Favourite topic: {mem.FavouriteTopic}";
            if (!string.IsNullOrEmpty(mem.LastTopic))
                info += $"  |  Last topic: {mem.LastTopic}";
            lblMemoryInfo.Text = info;
        }
    }
}
