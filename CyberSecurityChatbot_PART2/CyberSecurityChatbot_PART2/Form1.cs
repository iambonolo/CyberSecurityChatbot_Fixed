using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace CyberSecurityChatbot_PART2
{//start of namespace

    public partial class Form1 : Form
    {//start of class

        //========================================
        // FIELD DECLARATIONS
        //========================================

        // Stores the user's name
        private string userName = "";

        // Stores the user's favourite cybersecurity topic
        private string favouriteTopic = "";

        // Stores the last topic discussed for follow up questions
        private string lastTopic = "";

        // Random number generator for selecting random responses
        private Random random = new Random();

        // Tracks last response index per topic to avoid repeats
        private Dictionary<string, int> lastResponseIndex = new Dictionary<string, int>();

        // Delegate declaration for processing responses
        private delegate string ResponseDelegate(string input);

        // Colours used throughout the UI
        private Color purpleDark = Color.FromArgb(88, 44, 131);
        private Color purpleLight = Color.FromArgb(108, 64, 171);
        private Color sidebarColor = Color.FromArgb(68, 34, 111);
        private Color chatBackground = Color.FromArgb(245, 245, 250);

        //========================================
        // DICTIONARY — Keyword Responses
        // Uses generic collection as required
        //========================================
        private Dictionary<string, string[]> keywordResponses = new Dictionary<string, string[]>()
        {//start of keywordResponses dictionary

            { "password", new string[] {
                "Use strong passwords with a mix of letters, numbers and symbols!",
                "Never reuse passwords across different accounts.",
                "Consider using a password manager to keep your passwords safe.",
                "Avoid using personal information like birthdays in your passwords."
            }},
            { "phishing", new string[] {
                "Be cautious of emails asking for personal information.",
                "Always verify the sender's email address before clicking links.",
                "Legitimate organisations will never ask for your password via email.",
                "Look out for urgent language in emails — it is a common phishing tactic."
            }},
            { "scam", new string[] {
                "If something sounds too good to be true, it probably is!",
                "Never send money to someone you have not met in person.",
                "Be wary of unsolicited phone calls asking for personal details.",
                "Report scams to the South African Police Service cybercrime unit."
            }},
            { "privacy", new string[] {
                "Review your social media privacy settings regularly.",
                "Be careful about what personal information you share online.",
                "Use a VPN when connecting to public WiFi networks.",
                "Read privacy policies before signing up for new services."
            }},
            { "malware", new string[] {
                "Install reputable antivirus software and keep it updated.",
                "Never download software from untrusted sources.",
                "Be careful when clicking on links in emails or messages.",
                "Regularly scan your computer for malware."
            }},
            { "safe browsing", new string[] {
                "Always look for HTTPS before entering personal info.",
                "Keep your browser updated to the latest version.",
                "Use a reputable ad blocker to avoid malicious advertisements.",
                "Clear your browser history and cookies regularly."
            }}

        };//end of keywordResponses dictionary

        //========================================
        // DICTIONARY — Sentiment Responses
        //========================================
        private Dictionary<string, string> sentimentResponses = new Dictionary<string, string>()
        {//start of sentimentResponses dictionary

            { "worried", "It is completely understandable to feel that way. I am here to help!" },
            { "scared", "Do not worry! With the right knowledge you can protect yourself online." },
            { "frustrated", "I understand your frustration. Let us work through this together." },
            { "confused", "No worries! Let me explain things more clearly for you." },
            { "curious", "I love your curiosity! Learning about cybersecurity is the first step." },
            { "happy", "That is great to hear! Let us keep learning about staying safe online." }

        };//end of sentimentResponses dictionary

        //========================================
        // CONSTRUCTOR
        //========================================
        public Form1()
        {//start of constructor

            InitializeComponent();
            SetupForm();
            PlayVoiceGreeting();
            DisplayWelcomeMessage();

        }//end of constructor

        //========================================
        // METHOD — SetupForm
        // Builds the entire modern purple UI
        //========================================
        private void SetupForm()
        {//start of SetupForm method

            // Main form settings
            this.Text = "CyberSecurity Awareness Chatbot";
            this.BackColor = chatBackground;
            this.Size = new Size(1000, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.MinimumSize = new Size(1000, 720);

            //------------------------------------
            // LEFT SIDEBAR PANEL
            //------------------------------------
            Panel sidebar = new Panel();
            sidebar.BackColor = sidebarColor;
            sidebar.Location = new Point(0, 0);
            sidebar.Size = new Size(210, 720);
            this.Controls.Add(sidebar);

            // Logo image in sidebar
            PictureBox logoBox = new PictureBox();
            logoBox.Name = "logoBox";
            logoBox.Location = new Point(15, 20);
            logoBox.Size = new Size(180, 80);
            logoBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoBox.BackColor = Color.Transparent;
            try
            {//start of try block
                string logoPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "logo.jpg");
                if (System.IO.File.Exists(logoPath))
                {//start of if logo exists
                    logoBox.Image = Image.FromFile(logoPath);
                }//end of if logo exists
            }//end of try block
            catch { }//end of catch
            sidebar.Controls.Add(logoBox);

            // Sidebar divider line
            Panel sideDiv = new Panel();
            sideDiv.BackColor = Color.FromArgb(100, 255, 255, 255);
            sideDiv.Location = new Point(15, 110);
            sideDiv.Size = new Size(180, 1);
            sidebar.Controls.Add(sideDiv);

            // Sidebar menu buttons
            string[] menuItems = { "🏠  Home", "💬  Chat", "🔒  Topics", "⚙️  Settings" };
            int menuY = 125;
            foreach (string item in menuItems)
            {//start of menu items loop
                Button menuBtn = new Button();
                menuBtn.Text = item;
                menuBtn.Location = new Point(5, menuY);
                menuBtn.Size = new Size(200, 48);
                menuBtn.FlatStyle = FlatStyle.Flat;
                menuBtn.FlatAppearance.BorderSize = 0;
                menuBtn.ForeColor = Color.White;
                menuBtn.Font = new Font("Segoe UI", 10);
                menuBtn.TextAlign = ContentAlignment.MiddleLeft;
                menuBtn.Padding = new Padding(15, 0, 0, 0);
                menuBtn.BackColor = Color.Transparent;
                menuBtn.Cursor = Cursors.Hand;

                // Highlight Chat button
                if (item.Contains("Chat"))
                {//start of if chat highlighted
                    menuBtn.BackColor = purpleLight;
                    menuBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }//end of if chat highlighted

                sidebar.Controls.Add(menuBtn);
                menuY += 55;
            }//end of menu items loop

            // Bot online status
            Label statusDot = new Label();
            statusDot.Text = "🟢  Bot Online";
            statusDot.ForeColor = Color.FromArgb(180, 255, 180);
            statusDot.BackColor = Color.Transparent;
            statusDot.Font = new Font("Segoe UI", 9);
            statusDot.Location = new Point(15, 660);
            statusDot.AutoSize = true;
            sidebar.Controls.Add(statusDot);

            //------------------------------------
            // MAIN CHAT AREA
            //------------------------------------
            Panel mainPanel = new Panel();
            mainPanel.BackColor = chatBackground;
            mainPanel.Location = new Point(210, 0);
            mainPanel.Size = new Size(790, 720);
            this.Controls.Add(mainPanel);

            // Chat header
            Panel chatHeader = new Panel();
            chatHeader.BackColor = Color.White;
            chatHeader.Location = new Point(0, 0);
            chatHeader.Size = new Size(790, 70);
            mainPanel.Controls.Add(chatHeader);

            // Bot avatar circle
            Panel avatarCircle = new Panel();
            avatarCircle.BackColor = purpleDark;
            avatarCircle.Location = new Point(15, 15);
            avatarCircle.Size = new Size(42, 42);
            chatHeader.Controls.Add(avatarCircle);

            // Bot icon
            Label avatarIcon = new Label();
            avatarIcon.Text = "🔒";
            avatarIcon.Font = new Font("Segoe UI", 14);
            avatarIcon.BackColor = Color.Transparent;
            avatarIcon.ForeColor = Color.White;
            avatarIcon.Location = new Point(5, 8);
            avatarIcon.AutoSize = true;
            avatarCircle.Controls.Add(avatarIcon);

            // Bot name
            Label botName = new Label();
            botName.Text = "CyberSecurity Bot";
            botName.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            botName.ForeColor = Color.FromArgb(40, 40, 40);
            botName.BackColor = Color.Transparent;
            botName.Location = new Point(68, 10);
            botName.AutoSize = true;
            chatHeader.Controls.Add(botName);

            // Bot status
            Label botStatus = new Label();
            botStatus.Text = "🟢 Online — Always here to help";
            botStatus.Font = new Font("Segoe UI", 9);
            botStatus.ForeColor = Color.FromArgb(120, 120, 120);
            botStatus.BackColor = Color.Transparent;
            botStatus.Location = new Point(68, 38);
            botStatus.AutoSize = true;
            chatHeader.Controls.Add(botStatus);

            // Header border
            Panel hBorder = new Panel();
            hBorder.BackColor = Color.FromArgb(230, 230, 235);
            hBorder.Location = new Point(0, 69);
            hBorder.Size = new Size(790, 1);
            mainPanel.Controls.Add(hBorder);

            //------------------------------------
            // RICHTEXTBOX CHAT DISPLAY
            //------------------------------------
            RichTextBox chatBox = new RichTextBox();
            chatBox.Name = "chatBox";
            chatBox.Location = new Point(0, 70);
            chatBox.Size = new Size(790, 510);
            chatBox.BackColor = Color.FromArgb(248, 248, 252);
            chatBox.ForeColor = Color.FromArgb(40, 40, 40);
            chatBox.Font = new Font("Segoe UI", 10);
            chatBox.ReadOnly = true;
            chatBox.BorderStyle = BorderStyle.None;
            chatBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            chatBox.Padding = new Padding(15);
            mainPanel.Controls.Add(chatBox);

            // Chat bottom border
            Panel chatBorder = new Panel();
            chatBorder.BackColor = Color.FromArgb(225, 225, 235);
            chatBorder.Location = new Point(0, 580);
            chatBorder.Size = new Size(790, 1);
            mainPanel.Controls.Add(chatBorder);

            //------------------------------------
            // INPUT AREA AT BOTTOM
            //------------------------------------
            Panel inputArea = new Panel();
            inputArea.BackColor = Color.White;
            inputArea.Location = new Point(0, 581);
            inputArea.Size = new Size(790, 139);
            mainPanel.Controls.Add(inputArea);

            // Text input box
            TextBox inputBox = new TextBox();
            inputBox.Name = "inputBox";
            inputBox.Location = new Point(15, 35);
            inputBox.Size = new Size(620, 42);
            inputBox.BackColor = Color.FromArgb(245, 245, 250);
            inputBox.ForeColor = Color.FromArgb(40, 40, 40);
            inputBox.Font = new Font("Segoe UI", 11);
            inputBox.BorderStyle = BorderStyle.FixedSingle;
            inputBox.KeyPress += InputBox_KeyPress;
            inputArea.Controls.Add(inputBox);

            // Send button
            Button sendBtn = new Button();
            sendBtn.Name = "sendButton";
            sendBtn.Text = "Send ➤";
            sendBtn.Location = new Point(648, 32);
            sendBtn.Size = new Size(85, 42);
            sendBtn.BackColor = purpleDark;
            sendBtn.ForeColor = Color.White;
            sendBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            sendBtn.FlatStyle = FlatStyle.Flat;
            sendBtn.FlatAppearance.BorderSize = 0;
            sendBtn.Cursor = Cursors.Hand;
            sendBtn.Click += SendButton_Click;
            inputArea.Controls.Add(sendBtn);

            // Clear button
            Button clearBtn = new Button();
            clearBtn.Name = "clearButton";
            clearBtn.Text = "Clear";
            clearBtn.Location = new Point(743, 32);
            clearBtn.Size = new Size(65, 42);
            clearBtn.BackColor = Color.FromArgb(215, 215, 225);
            clearBtn.ForeColor = Color.FromArgb(60, 60, 60);
            clearBtn.Font = new Font("Segoe UI", 10);
            clearBtn.FlatStyle = FlatStyle.Flat;
            clearBtn.FlatAppearance.BorderSize = 0;
            clearBtn.Cursor = Cursors.Hand;
            clearBtn.Click += ClearButton_Click;
            inputArea.Controls.Add(clearBtn);

            // Topics hint label
            Label hint = new Label();
            hint.Text = "💡 Try: password, phishing, scam, privacy, malware, safe browsing";
            hint.ForeColor = Color.FromArgb(160, 160, 170);
            hint.BackColor = Color.Transparent;
            hint.Font = new Font("Segoe UI", 8);
            hint.Location = new Point(15, 88);
            hint.AutoSize = true;
            inputArea.Controls.Add(hint);

        }//end of SetupForm method

        //========================================
        // METHOD — PlayVoiceGreeting
        //========================================
        private void PlayVoiceGreeting()
        {//start of PlayVoiceGreeting method

            try
            {//start of try block
                string path = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "greet.wav");
                if (System.IO.File.Exists(path))
                {//start of if file exists
                    SoundPlayer player = new SoundPlayer(path);
                    player.Play();
                }//end of if file exists
            }//end of try block
            catch { }//end of catch

        }//end of PlayVoiceGreeting method

        //========================================
        // METHOD — DisplayWelcomeMessage
        //========================================
        private void DisplayWelcomeMessage()
        {//start of DisplayWelcomeMessage method

            AppendBotMessage("👋 Welcome to the CyberSecurity Awareness Chatbot!");
            AppendBotMessage("I am here to help you stay safe online. 🔒");
            AppendBotMessage("What is your name?");

        }//end of DisplayWelcomeMessage method

        //========================================
        // METHOD — AppendBotMessage
        // Adds bot message to chat in purple
        //========================================
        private void AppendBotMessage(string message)
        {//start of AppendBotMessage method

            RichTextBox chatBox = this.Controls
                .Find("chatBox", true).Length > 0
                ? this.Controls.Find("chatBox", true)[0] as RichTextBox
                : null;

            if (chatBox != null)
            {//start of if chatBox found
                chatBox.SelectionStart = chatBox.TextLength;
                chatBox.SelectionLength = 0;
                chatBox.SelectionColor = purpleDark;
                chatBox.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
                chatBox.AppendText("  🔒 Bot\n");
                chatBox.SelectionColor = Color.FromArgb(40, 40, 40);
                chatBox.SelectionFont = new Font("Segoe UI", 10);
                chatBox.AppendText($"  {message}\n\n");
                chatBox.ScrollToCaret();
            }//end of if chatBox found

        }//end of AppendBotMessage method

        //========================================
        // METHOD — AppendUserMessage
        // Adds user message to chat in purple
        //========================================
        private void AppendUserMessage(string message)
        {//start of AppendUserMessage method

            RichTextBox chatBox = this.Controls
                .Find("chatBox", true).Length > 0
                ? this.Controls.Find("chatBox", true)[0] as RichTextBox
                : null;

            if (chatBox != null)
            {//start of if chatBox found
                chatBox.SelectionStart = chatBox.TextLength;
                chatBox.SelectionLength = 0;
                chatBox.SelectionColor = purpleLight;
                chatBox.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
                chatBox.AppendText("  👤 You\n");
                chatBox.SelectionColor = purpleDark;
                chatBox.SelectionFont = new Font("Segoe UI", 10);
                chatBox.AppendText($"  {message}\n\n");
                chatBox.ScrollToCaret();
            }//end of if chatBox found

        }//end of AppendUserMessage method

        //========================================
        // EVENT — SendButton_Click
        //========================================
        private void SendButton_Click(object sender, EventArgs e)
        {//start of SendButton_Click event
            ProcessInput();
        }//end of SendButton_Click event

        //========================================
        // EVENT — ClearButton_Click
        //========================================
        private void ClearButton_Click(object sender, EventArgs e)
        {//start of ClearButton_Click event

            RichTextBox chatBox = this.Controls
                .Find("chatBox", true).Length > 0
                ? this.Controls.Find("chatBox", true)[0] as RichTextBox
                : null;

            if (chatBox != null)
            {//start of if chatBox found
                chatBox.Clear();
                userName = "";
                favouriteTopic = "";
                lastTopic = "";
                lastResponseIndex.Clear();
                DisplayWelcomeMessage();
            }//end of if chatBox found

        }//end of ClearButton_Click event

        //========================================
        // EVENT — InputBox_KeyPress
        //========================================
        private void InputBox_KeyPress(object sender, KeyPressEventArgs e)
        {//start of InputBox_KeyPress event

            if (e.KeyChar == (char)Keys.Enter)
            {//start of if Enter pressed
                ProcessInput();
                e.Handled = true;
            }//end of if Enter pressed

        }//end of InputBox_KeyPress event

        //========================================
        // METHOD — ProcessInput
        // Validates input and uses delegate
        //========================================
        private void ProcessInput()
        {//start of ProcessInput method

            TextBox inputBox = this.Controls
                .Find("inputBox", true).Length > 0
                ? this.Controls.Find("inputBox", true)[0] as TextBox
                : null;

            if (inputBox == null) return;

            string userInput = inputBox.Text.Trim();

            // Input validation
            if (string.IsNullOrEmpty(userInput))
            {//start of if empty
                AppendBotMessage("Please type something so I can help you! 😊");
                return;
            }//end of if empty

            AppendUserMessage(userInput);
            inputBox.Clear();

            // Use delegate to call GenerateResponse
            ResponseDelegate processResponse = GenerateResponse;
            string response = processResponse(userInput.ToLower());
            AppendBotMessage(response);

            // Memory recall occasionally
            if (!string.IsNullOrEmpty(favouriteTopic) &&
                !string.IsNullOrEmpty(userName) &&
                random.Next(4) == 0)
            {//start of memory recall
                AppendBotMessage(
                    $"💡 {userName}, as someone interested in " +
                    $"{favouriteTopic}, always stay informed " +
                    "about the latest threats!");
            }//end of memory recall

        }//end of ProcessInput method

        //========================================
        // METHOD — GenerateResponse
        // Main chatbot logic via delegate
        //========================================
        private string GenerateResponse(string input)
        {//start of GenerateResponse method

            // STEP 1 — Get name first
            if (string.IsNullOrEmpty(userName))
            {//start of if no name
                userName = input;
                return $"Nice to meet you {userName}! 😊\n" +
                       "  You can ask me about:\n" +
                       "  🔑 Passwords  |  🎣 Phishing  |  💰 Scams\n" +
                       "  🔒 Privacy  |  🦠 Malware  |  🌐 Safe Browsing\n" +
                       "  What would you like to know?";
            }//end of if no name

            // STEP 2 — Sentiment detection
            foreach (var sentiment in sentimentResponses)
            {//start of sentiment loop
                if (input.Contains(sentiment.Key))
                {//start of if sentiment found
                    lastTopic = "phishing";
                    string tip = GetRandomResponse("phishing");
                    return $"{sentiment.Value}\n\n  💡 Here is a tip: {tip}";
                }//end of if sentiment found
            }//end of sentiment loop

            // STEP 3 — Follow up questions
            if (input.Contains("more") || input.Contains("another") ||
                input.Contains("explain") || input.Contains("tell me more") ||
                input.Contains("give me another"))
            {//start of if follow up
                if (!string.IsNullOrEmpty(lastTopic))
                {//start of if last topic
                    return $"Sure {userName}! Another tip about {lastTopic}:\n" +
                           $"  💡 {GetRandomResponse(lastTopic)}";
                }//end of if last topic
                return $"Which topic would you like more on {userName}?\n" +
                       "  Passwords, Phishing, Scams, Privacy, " +
                       "Malware, or Safe Browsing?";
            }//end of if follow up

            // STEP 4 — Memory feature
            if (input.Contains("interested in") || input.Contains("i like") ||
                input.Contains("i love") || input.Contains("favourite"))
            {//start of if favourite
                foreach (var keyword in keywordResponses.Keys)
                {//start of keyword loop
                    if (input.Contains(keyword))
                    {//start of if keyword found
                        favouriteTopic = keyword;
                        return $"Great {userName}! I will remember you are " +
                               $"interested in {keyword}. 🧠\n" +
                               $"  💡 Here is a tip: {GetRandomResponse(keyword)}";
                    }//end of if keyword found
                }//end of keyword loop
            }//end of if favourite

            // STEP 5 — Keyword recognition
            foreach (var keyword in keywordResponses.Keys)
            {//start of keyword loop
                if (input.Contains(keyword))
                {//start of if keyword matched
                    lastTopic = keyword;
                    return $"💡 {GetRandomResponse(keyword)}";
                }//end of if keyword matched
            }//end of keyword loop

            // STEP 6 — How are you
            if (input.Contains("how are you"))
            {//start of if how are you
                return $"I am doing great {userName}, thank you! 😊\n" +
                       "  What cybersecurity topic can I help you with?";
            }//end of if how are you

            // STEP 7 — Purpose
            if (input.Contains("purpose") || input.Contains("what can you do") ||
                input.Contains("help") || input.Contains("what can i ask"))
            {//start of if purpose
                return $"I am here to educate you about cybersecurity {userName}! 🔒\n" +
                       "  Ask me about Passwords, Phishing, Scams, " +
                       "Privacy, Malware or Safe Browsing.";
            }//end of if purpose

            // STEP 8 — Goodbye
            if (input.Contains("bye") || input.Contains("goodbye") ||
                input.Contains("exit") || input.Contains("quit"))
            {//start of if goodbye
                return $"Goodbye {userName}! 👋 Stay safe online! 🔒\n" +
                       "  Remember to always think before you click!";
            }//end of if goodbye

            // STEP 9 — Default error handling
            return $"I am not sure I understand {userName}. 🤔\n" +
                   "  Could you rephrase? I can help with:\n" +
                   "  Passwords, Phishing, Scams, Privacy, " +
                   "Malware, or Safe Browsing.";

        }//end of GenerateResponse method

        //========================================
        // METHOD — GetRandomResponse
        // Picks random response never repeating
        //========================================
        private string GetRandomResponse(string topic)
        {//start of GetRandomResponse method

            if (keywordResponses.ContainsKey(topic))
            {//start of if topic exists

                string[] responses = keywordResponses[topic];

                int lastIndex = -1;
                if (lastResponseIndex.ContainsKey(topic))
                {//start of if last index exists
                    lastIndex = lastResponseIndex[topic];
                }//end of if last index exists

                int newIndex;
                do
                {//start of do while loop
                    newIndex = random.Next(responses.Length);
                }//end of do
                while (newIndex == lastIndex && responses.Length > 1);

                lastResponseIndex[topic] = newIndex;
                return responses[newIndex];

            }//end of if topic exists

            return "Stay safe online and always be cautious!";

        }//end of GetRandomResponse method

    }//end of class

}//end of namespace