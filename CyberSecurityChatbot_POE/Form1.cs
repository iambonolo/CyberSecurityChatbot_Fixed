using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace CyberSecurityChatbot_POE
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

        // Stores the last topic discussed
        private string lastTopic = "";

        // Random number generator
        private Random random = new Random();

        // Tracks last response index per topic
        private Dictionary<string, int> lastResponseIndex = new Dictionary<string, int>();

        // Delegate for processing responses
        private delegate string ResponseDelegate(string input);

        // Activity log list — stores last 10 actions
        private List<string> activityLog = new List<string>();

        // Task list for task assistant
        private List<CyberTask> taskList = new List<CyberTask>();

        // Quiz variables
        private int quizScore = 0;
        private int currentQuestionIndex = 0;
        private bool quizActive = false;
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();

        // Colours
        private Color purpleDark = Color.FromArgb(88, 44, 131);
        private Color purpleLight = Color.FromArgb(108, 64, 171);
        private Color sidebarColor = Color.FromArgb(68, 34, 111);
        private Color chatBackground = Color.FromArgb(245, 245, 250);

        //========================================
        // CLASS — CyberTask
        // Stores task details
        //========================================
        private class CyberTask
        {//start of CyberTask class
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Reminder { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime DateAdded { get; set; }
        }//end of CyberTask class

        //========================================
        // CLASS — QuizQuestion
        // Stores quiz question details
        //========================================
        private class QuizQuestion
        {//start of QuizQuestion class
            public string Question { get; set; }
            public List<string> Options { get; set; }
            public int CorrectIndex { get; set; }
            public string Explanation { get; set; }
        }//end of QuizQuestion class

        //========================================
        // DICTIONARY — Keyword Responses
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
            SetupQuizQuestions();
            PlayVoiceGreeting();
            DisplayWelcomeMessage();

        }//end of constructor

        //========================================
        // METHOD — SetupQuizQuestions
        // Initialises all quiz questions
        //========================================
        private void SetupQuizQuestions()
        {//start of SetupQuizQuestions method

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What should you do if you receive an email asking for your password?",
                Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report it as phishing", "D) Ignore it" },
                CorrectIndex = 2,
                Explanation = "Correct! Reporting phishing emails helps prevent scams and protects others."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: Using the same password for multiple accounts is safe.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectIndex = 1,
                Explanation = "False! Using the same password means if one account is hacked, all accounts are at risk."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What does HTTPS mean in a website address?",
                Options = new List<string> { "A) The site is fast", "B) The site is secure", "C) The site is free", "D) The site is popular" },
                CorrectIndex = 1,
                Explanation = "Correct! HTTPS means the website uses encryption to protect your data."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What is social engineering?",
                Options = new List<string> { "A) Building social media apps", "B) Manipulating people to reveal information", "C) Engineering social networks", "D) Creating online communities" },
                CorrectIndex = 1,
                Explanation = "Correct! Social engineering tricks people into revealing confidential information."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: Public WiFi is always safe to use for banking.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectIndex = 1,
                Explanation = "False! Public WiFi can be monitored by hackers. Use a VPN for sensitive activities."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What is two-factor authentication (2FA)?",
                Options = new List<string> { "A) Using two passwords", "B) A second verification step", "C) Having two accounts", "D) Two email addresses" },
                CorrectIndex = 1,
                Explanation = "Correct! 2FA adds an extra layer of security beyond just your password."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What should you do before clicking a link in an email?",
                Options = new List<string> { "A) Click immediately", "B) Verify the sender and hover over the link", "C) Forward it to friends", "D) Reply to the email" },
                CorrectIndex = 1,
                Explanation = "Correct! Always verify links before clicking to avoid phishing attacks."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "True or False: Antivirus software makes your computer 100% safe.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectIndex = 1,
                Explanation = "False! Antivirus helps but no software provides 100% protection. Stay vigilant!"
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "What is malware?",
                Options = new List<string> { "A) Good software", "B) Malicious software designed to harm", "C) A type of hardware", "D) A network protocol" },
                CorrectIndex = 1,
                Explanation = "Correct! Malware is software designed to disrupt, damage or gain unauthorised access."
            });

            quizQuestions.Add(new QuizQuestion
            {
                Question = "How often should you update your passwords?",
                Options = new List<string> { "A) Never", "B) Every 10 years", "C) Regularly, every 3-6 months", "D) Only when hacked" },
                CorrectIndex = 2,
                Explanation = "Correct! Regular password updates help keep your accounts secure."
            });

        }//end of SetupQuizQuestions method

        //========================================
        // METHOD — SetupForm
        // Builds the entire modern purple UI
        //========================================
        private void SetupForm()
        {//start of SetupForm method

            // Main form settings
            this.Text = "CyberSecurity Awareness Chatbot — POE";
            this.BackColor = chatBackground;
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.MinimumSize = new Size(1100, 750);

            //------------------------------------
            // LEFT SIDEBAR
            //------------------------------------
            Panel sidebar = new Panel();
            sidebar.BackColor = sidebarColor;
            sidebar.Location = new Point(0, 0);
            sidebar.Size = new Size(210, 750);
            this.Controls.Add(sidebar);

            // Logo
            PictureBox logoBox = new PictureBox();
            logoBox.Name = "logoBox";
            logoBox.Location = new Point(15, 20);
            logoBox.Size = new Size(180, 80);
            logoBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoBox.BackColor = Color.Transparent;
            try
            {//start of try
                string logoPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "logo.jpg");
                if (System.IO.File.Exists(logoPath))
                {//start of if logo exists
                    logoBox.Image = Image.FromFile(logoPath);
                }//end of if logo exists
            }//end of try
            catch { }//end of catch
            sidebar.Controls.Add(logoBox);

            // Sidebar divider
            Panel sideDiv = new Panel();
            sideDiv.BackColor = Color.FromArgb(100, 255, 255, 255);
            sideDiv.Location = new Point(15, 110);
            sideDiv.Size = new Size(180, 1);
            sidebar.Controls.Add(sideDiv);

            // Sidebar menu buttons
            string[] menuItems = {
                "💬  Chat",
                "📋  Tasks",
                "🎮  Quiz",
                "📜  Activity Log",
                "⚙️  Settings"
            };
            int menuY = 125;
            foreach (string item in menuItems)
            {//start of menu loop
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
                menuBtn.Tag = item;
                menuBtn.Click += MenuButton_Click;

                if (item.Contains("Chat"))
                {//start of if chat highlighted
                    menuBtn.BackColor = purpleLight;
                    menuBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }//end of if chat highlighted

                sidebar.Controls.Add(menuBtn);
                menuY += 55;
            }//end of menu loop

            // Status dot
            Label statusDot = new Label();
            statusDot.Text = "🟢  Bot Online";
            statusDot.ForeColor = Color.FromArgb(180, 255, 180);
            statusDot.BackColor = Color.Transparent;
            statusDot.Font = new Font("Segoe UI", 9);
            statusDot.Location = new Point(15, 700);
            statusDot.AutoSize = true;
            sidebar.Controls.Add(statusDot);

            //------------------------------------
            // MAIN CONTENT AREA
            //------------------------------------
            Panel mainPanel = new Panel();
            mainPanel.Name = "mainPanel";
            mainPanel.BackColor = chatBackground;
            mainPanel.Location = new Point(210, 0);
            mainPanel.Size = new Size(890, 750);
            this.Controls.Add(mainPanel);

            // Chat header
            Panel chatHeader = new Panel();
            chatHeader.BackColor = Color.White;
            chatHeader.Location = new Point(0, 0);
            chatHeader.Size = new Size(890, 70);
            mainPanel.Controls.Add(chatHeader);

            // Bot avatar
            Panel avatarCircle = new Panel();
            avatarCircle.BackColor = purpleDark;
            avatarCircle.Location = new Point(15, 15);
            avatarCircle.Size = new Size(42, 42);
            chatHeader.Controls.Add(avatarCircle);

            Label avatarIcon = new Label();
            avatarIcon.Text = "🔒";
            avatarIcon.Font = new Font("Segoe UI", 14);
            avatarIcon.BackColor = Color.Transparent;
            avatarIcon.ForeColor = Color.White;
            avatarIcon.Location = new Point(5, 8);
            avatarIcon.AutoSize = true;
            avatarCircle.Controls.Add(avatarIcon);

            Label botName = new Label();
            botName.Text = "CyberSecurity Awareness Bot";
            botName.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            botName.ForeColor = Color.FromArgb(40, 40, 40);
            botName.BackColor = Color.Transparent;
            botName.Location = new Point(68, 10);
            botName.AutoSize = true;
            chatHeader.Controls.Add(botName);

            Label botStatus = new Label();
            botStatus.Text = "🟢 Online — Chat | Tasks | Quiz | Activity Log";
            botStatus.Font = new Font("Segoe UI", 9);
            botStatus.ForeColor = Color.FromArgb(120, 120, 120);
            botStatus.BackColor = Color.Transparent;
            botStatus.Location = new Point(68, 38);
            botStatus.AutoSize = true;
            chatHeader.Controls.Add(botStatus);

            Panel hBorder = new Panel();
            hBorder.BackColor = Color.FromArgb(230, 230, 235);
            hBorder.Location = new Point(0, 69);
            hBorder.Size = new Size(890, 1);
            mainPanel.Controls.Add(hBorder);

            //------------------------------------
            // CHAT DISPLAY
            //------------------------------------
            RichTextBox chatBox = new RichTextBox();
            chatBox.Name = "chatBox";
            chatBox.Location = new Point(0, 70);
            chatBox.Size = new Size(890, 510);
            chatBox.BackColor = Color.FromArgb(248, 248, 252);
            chatBox.ForeColor = Color.FromArgb(40, 40, 40);
            chatBox.Font = new Font("Segoe UI", 10);
            chatBox.ReadOnly = true;
            chatBox.BorderStyle = BorderStyle.None;
            chatBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            mainPanel.Controls.Add(chatBox);

            Panel chatBorder = new Panel();
            chatBorder.BackColor = Color.FromArgb(225, 225, 235);
            chatBorder.Location = new Point(0, 580);
            chatBorder.Size = new Size(890, 1);
            mainPanel.Controls.Add(chatBorder);

            //------------------------------------
            // INPUT AREA
            //------------------------------------
            Panel inputArea = new Panel();
            inputArea.BackColor = Color.White;
            inputArea.Location = new Point(0, 581);
            inputArea.Size = new Size(890, 169);
            mainPanel.Controls.Add(inputArea);

            TextBox inputBox = new TextBox();
            inputBox.Name = "inputBox";
            inputBox.Location = new Point(15, 35);
            inputBox.Size = new Size(720, 42);
            inputBox.BackColor = Color.FromArgb(245, 245, 250);
            inputBox.ForeColor = Color.FromArgb(40, 40, 40);
            inputBox.Font = new Font("Segoe UI", 11);
            inputBox.BorderStyle = BorderStyle.FixedSingle;
            inputBox.KeyPress += InputBox_KeyPress;
            inputArea.Controls.Add(inputBox);

            Button sendBtn = new Button();
            sendBtn.Name = "sendButton";
            sendBtn.Text = "Send ➤";
            sendBtn.Location = new Point(748, 32);
            sendBtn.Size = new Size(80, 42);
            sendBtn.BackColor = purpleDark;
            sendBtn.ForeColor = Color.White;
            sendBtn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            sendBtn.FlatStyle = FlatStyle.Flat;
            sendBtn.FlatAppearance.BorderSize = 0;
            sendBtn.Cursor = Cursors.Hand;
            sendBtn.Click += SendButton_Click;
            inputArea.Controls.Add(sendBtn);

            Button clearBtn = new Button();
            clearBtn.Name = "clearButton";
            clearBtn.Text = "Clear";
            clearBtn.Location = new Point(838, 32);
            clearBtn.Size = new Size(60, 42);
            clearBtn.BackColor = Color.FromArgb(215, 215, 225);
            clearBtn.ForeColor = Color.FromArgb(60, 60, 60);
            clearBtn.Font = new Font("Segoe UI", 10);
            clearBtn.FlatStyle = FlatStyle.Flat;
            clearBtn.FlatAppearance.BorderSize = 0;
            clearBtn.Cursor = Cursors.Hand;
            clearBtn.Click += ClearButton_Click;
            inputArea.Controls.Add(clearBtn);

            Label hint = new Label();
            hint.Text = "💡 Try: password | phishing | add task | start quiz | show log | show tasks";
            hint.ForeColor = Color.FromArgb(160, 160, 170);
            hint.BackColor = Color.Transparent;
            hint.Font = new Font("Segoe UI", 8);
            hint.Location = new Point(15, 88);
            hint.AutoSize = true;
            inputArea.Controls.Add(hint);

            Label hint2 = new Label();
            hint2.Text = "💡 Tasks: 'add task [name]' | 'view tasks' | 'complete task [number]' | 'delete task [number]'";
            hint2.ForeColor = Color.FromArgb(160, 160, 170);
            hint2.BackColor = Color.Transparent;
            hint2.Font = new Font("Segoe UI", 8);
            hint2.Location = new Point(15, 108);
            hint2.AutoSize = true;
            inputArea.Controls.Add(hint2);

            Label hint3 = new Label();
            hint3.Text = "💡 Quiz: 'start quiz' | Activity Log: 'show log' or 'what have you done'";
            hint3.ForeColor = Color.FromArgb(160, 160, 170);
            hint3.BackColor = Color.Transparent;
            hint3.Font = new Font("Segoe UI", 8);
            hint3.Location = new Point(15, 128);
            hint3.AutoSize = true;
            inputArea.Controls.Add(hint3);

        }//end of SetupForm method

        //========================================
        // METHOD — MenuButton_Click
        // Handles sidebar menu button clicks
        //========================================
        private void MenuButton_Click(object sender, EventArgs e)
        {//start of MenuButton_Click

            Button btn = sender as Button;
            if (btn == null) return;

            string tag = btn.Tag.ToString();

            if (tag.Contains("Tasks"))
            {//start of if tasks
                AppendBotMessage("📋 TASK ASSISTANT\n" +
                    "  Commands:\n" +
                    "  • 'add task [name]' — Add a new task\n" +
                    "  • 'view tasks' — See all your tasks\n" +
                    "  • 'complete task [number]' — Mark task as done\n" +
                    "  • 'delete task [number]' — Remove a task\n" +
                    "  • 'remind me in [X] days' — Set a reminder");
                AddToLog("User opened Task Assistant");
            }//end of if tasks
            else if (tag.Contains("Quiz"))
            {//start of if quiz
                AppendBotMessage("🎮 CYBERSECURITY QUIZ\n" +
                    "  Type 'start quiz' to begin!\n" +
                    "  Test your cybersecurity knowledge with 10 questions.");
                AddToLog("User opened Quiz menu");
            }//end of if quiz
            else if (tag.Contains("Activity Log"))
            {//start of if log
                ShowActivityLog();
            }//end of if log
            else if (tag.Contains("Chat"))
            {//start of if chat
                AppendBotMessage("💬 Welcome back to Chat!\n" +
                    "  Ask me about passwords, phishing, scams, privacy, malware or safe browsing.");
            }//end of if chat

        }//end of MenuButton_Click

        //========================================
        // METHOD — PlayVoiceGreeting
        //========================================
        private void PlayVoiceGreeting()
        {//start of PlayVoiceGreeting

            try
            {//start of try
                string path = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "greet.wav");
                if (System.IO.File.Exists(path))
                {//start of if exists
                    SoundPlayer player = new SoundPlayer(path);
                    player.Play();
                }//end of if exists
            }//end of try
            catch { }//end of catch

        }//end of PlayVoiceGreeting

        //========================================
        // METHOD — DisplayWelcomeMessage
        //========================================
        private void DisplayWelcomeMessage()
        {//start of DisplayWelcomeMessage

            AppendBotMessage("👋 Welcome to the CyberSecurity Awareness Chatbot POE!");
            AppendBotMessage("I am your complete cybersecurity assistant. I can help you with:");
            AppendBotMessage("💬 Cybersecurity Chat — Ask me anything about staying safe online\n" +
                "  📋 Task Assistant — Manage your cybersecurity tasks\n" +
                "  🎮 Mini Quiz — Test your cybersecurity knowledge\n" +
                "  📜 Activity Log — View recent actions\n\n" +
                "  What is your name?");

        }//end of DisplayWelcomeMessage

        //========================================
        // METHOD — AddToLog
        // Adds action to activity log
        //========================================
        private void AddToLog(string action)
        {//start of AddToLog

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            activityLog.Insert(0, $"[{timestamp}] {action}");

            // Keep only last 10 actions
            if (activityLog.Count > 10)
            {//start of if too many
                activityLog.RemoveAt(activityLog.Count - 1);
            }//end of if too many

        }//end of AddToLog

        //========================================
        // METHOD — ShowActivityLog
        // Displays the activity log to user
        //========================================
        private void ShowActivityLog()
        {//start of ShowActivityLog

            if (activityLog.Count == 0)
            {//start of if empty log
                AppendBotMessage("📜 No recent activity to show yet. Start chatting to build your log!");
                return;
            }//end of if empty log

            string log = "📜 ACTIVITY LOG — Last " + activityLog.Count + " actions:\n\n";
            for (int i = 0; i < activityLog.Count; i++)
            {//start of log loop
                log += $"  {i + 1}. {activityLog[i]}\n";
            }//end of log loop

            AppendBotMessage(log);
            AddToLog("User viewed activity log");

        }//end of ShowActivityLog

        //========================================
        // METHOD — AppendBotMessage
        //========================================
        private void AppendBotMessage(string message)
        {//start of AppendBotMessage

            RichTextBox chatBox = this.Controls
                .Find("chatBox", true).Length > 0
                ? this.Controls.Find("chatBox", true)[0] as RichTextBox
                : null;

            if (chatBox != null)
            {//start of if found
                chatBox.SelectionStart = chatBox.TextLength;
                chatBox.SelectionLength = 0;
                chatBox.SelectionColor = purpleDark;
                chatBox.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
                chatBox.AppendText("  🔒 Bot\n");
                chatBox.SelectionColor = Color.FromArgb(40, 40, 40);
                chatBox.SelectionFont = new Font("Segoe UI", 10);
                chatBox.AppendText($"  {message}\n\n");
                chatBox.ScrollToCaret();
            }//end of if found

        }//end of AppendBotMessage

        //========================================
        // METHOD — AppendUserMessage
        //========================================
        private void AppendUserMessage(string message)
        {//start of AppendUserMessage

            RichTextBox chatBox = this.Controls
                .Find("chatBox", true).Length > 0
                ? this.Controls.Find("chatBox", true)[0] as RichTextBox
                : null;

            if (chatBox != null)
            {//start of if found
                chatBox.SelectionStart = chatBox.TextLength;
                chatBox.SelectionLength = 0;
                chatBox.SelectionColor = purpleLight;
                chatBox.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
                chatBox.AppendText("  👤 You\n");
                chatBox.SelectionColor = purpleDark;
                chatBox.SelectionFont = new Font("Segoe UI", 10);
                chatBox.AppendText($"  {message}\n\n");
                chatBox.ScrollToCaret();
            }//end of if found

        }//end of AppendUserMessage

        //========================================
        // EVENT — SendButton_Click
        //========================================
        private void SendButton_Click(object sender, EventArgs e)
        {//start of SendButton_Click
            ProcessInput();
        }//end of SendButton_Click

        //========================================
        // EVENT — ClearButton_Click
        //========================================
        private void ClearButton_Click(object sender, EventArgs e)
        {//start of ClearButton_Click

            RichTextBox chatBox = this.Controls
                .Find("chatBox", true).Length > 0
                ? this.Controls.Find("chatBox", true)[0] as RichTextBox
                : null;

            if (chatBox != null)
            {//start of if found
                chatBox.Clear();
                userName = "";
                favouriteTopic = "";
                lastTopic = "";
                lastResponseIndex.Clear();
                quizActive = false;
                currentQuestionIndex = 0;
                quizScore = 0;
                DisplayWelcomeMessage();
                AddToLog("Chat cleared and restarted");
            }//end of if found

        }//end of ClearButton_Click

        //========================================
        // EVENT — InputBox_KeyPress
        //========================================
        private void InputBox_KeyPress(object sender, KeyPressEventArgs e)
        {//start of InputBox_KeyPress

            if (e.KeyChar == (char)13)
            {//start of if Enter
                ProcessInput();
                e.Handled = true;
            }//end of if Enter

        }//end of InputBox_KeyPress

        //========================================
        // METHOD — ProcessInput
        // Validates and processes user input
        //========================================
        private void ProcessInput()
        {//start of ProcessInput

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

            // If quiz is active handle quiz input
            if (quizActive)
            {//start of if quiz active
                HandleQuizAnswer(userInput.ToLower());
                return;
            }//end of if quiz active

            // Use delegate to generate response
            ResponseDelegate processResponse = GenerateResponse;
            string response = processResponse(userInput.ToLower());
            AppendBotMessage(response);

            // Memory recall
            if (!string.IsNullOrEmpty(favouriteTopic) &&
                !string.IsNullOrEmpty(userName) &&
                random.Next(4) == 0)
            {//start of memory recall
                AppendBotMessage(
                    $"💡 {userName}, as someone interested in " +
                    $"{favouriteTopic}, always stay informed about the latest threats!");
            }//end of memory recall

        }//end of ProcessInput

        //========================================
        // METHOD — GenerateResponse
        // Main chatbot logic with NLP simulation
        //========================================
        private string GenerateResponse(string input)
        {//start of GenerateResponse

            // STEP 1 — Get name
            if (string.IsNullOrEmpty(userName))
            {//start of if no name
                userName = input;
                AddToLog($"User introduced as: {userName}");
                return $"Nice to meet you {userName}! 😊\n" +
                       "  I can help you with:\n" +
                       "  💬 Cybersecurity tips — just ask!\n" +
                       "  📋 Tasks — type 'add task [name]'\n" +
                       "  🎮 Quiz — type 'start quiz'\n" +
                       "  📜 Log — type 'show log'\n" +
                       "  What would you like to do?";
            }//end of if no name

            // STEP 2 — NLP: Activity log commands
            if (input.Contains("show log") || input.Contains("activity log") ||
                input.Contains("what have you done") || input.Contains("recent actions") ||
                input.Contains("history"))
            {//start of if show log
                ShowActivityLog();
                return "";
            }//end of if show log

            // STEP 3 — NLP: Quiz commands
            if (input.Contains("start quiz") || input.Contains("begin quiz") ||
                input.Contains("play quiz") || input.Contains("quiz me") ||
                input.Contains("test me") || input.Contains("quiz"))
            {//start of if quiz
                return StartQuiz();
            }//end of if quiz

            // STEP 4 — NLP: Task commands — Add task
            if (input.Contains("add task") || input.Contains("new task") ||
                input.Contains("create task") || input.Contains("set task") ||
                input.Contains("remind me to") || input.Contains("i need to"))
            {//start of if add task
                return HandleAddTask(input);
            }//end of if add task

            // STEP 5 — NLP: View tasks
            if (input.Contains("view tasks") || input.Contains("show tasks") ||
                input.Contains("my tasks") || input.Contains("list tasks") ||
                input.Contains("what tasks"))
            {//start of if view tasks
                return ViewTasks();
            }//end of if view tasks

            // STEP 6 — NLP: Complete task
            if (input.Contains("complete task") || input.Contains("done task") ||
                input.Contains("finish task") || input.Contains("mark task"))
            {//start of if complete task
                return HandleCompleteTask(input);
            }//end of if complete task

            // STEP 7 — NLP: Delete task
            if (input.Contains("delete task") || input.Contains("remove task") ||
                input.Contains("cancel task"))
            {//start of if delete task
                return HandleDeleteTask(input);
            }//end of if delete task

            // STEP 8 — Sentiment detection
            foreach (var sentiment in sentimentResponses)
            {//start of sentiment loop
                if (input.Contains(sentiment.Key))
                {//start of if sentiment
                    AddToLog($"Sentiment detected: {sentiment.Key}");
                    string tip = GetRandomResponse("phishing");
                    return $"{sentiment.Value}\n\n  💡 Here is a tip: {tip}";
                }//end of if sentiment
            }//end of sentiment loop

            // STEP 9 — Follow up
            if (input.Contains("more") || input.Contains("another") ||
                input.Contains("tell me more") || input.Contains("explain more"))
            {//start of if follow up
                if (!string.IsNullOrEmpty(lastTopic))
                {//start of if last topic
                    AddToLog($"Follow up on topic: {lastTopic}");
                    return $"Sure {userName}! Another tip about {lastTopic}:\n" +
                           $"  💡 {GetRandomResponse(lastTopic)}";
                }//end of if last topic
                return $"Which topic would you like more on {userName}?\n" +
                       "  Passwords, Phishing, Scams, Privacy, Malware, or Safe Browsing?";
            }//end of if follow up

            // STEP 10 — Memory
            if (input.Contains("interested in") || input.Contains("i like") ||
                input.Contains("i love") || input.Contains("favourite"))
            {//start of if favourite
                foreach (var keyword in keywordResponses.Keys)
                {//start of keyword loop
                    if (input.Contains(keyword))
                    {//start of if keyword
                        favouriteTopic = keyword;
                        AddToLog($"User favourite topic set: {keyword}");
                        return $"Great {userName}! I will remember you are interested in {keyword}. 🧠\n" +
                               $"  💡 {GetRandomResponse(keyword)}";
                    }//end of if keyword
                }//end of keyword loop
            }//end of if favourite

            // STEP 11 — Keyword recognition
            foreach (var keyword in keywordResponses.Keys)
            {//start of keyword loop
                if (input.Contains(keyword))
                {//start of if keyword
                    lastTopic = keyword;
                    AddToLog($"Keyword recognised: {keyword}");
                    return $"💡 {GetRandomResponse(keyword)}";
                }//end of if keyword
            }//end of keyword loop

            // STEP 12 — How are you
            if (input.Contains("how are you"))
            {//start of if how are you
                return $"I am doing great {userName}, thank you! 😊\n" +
                       "  What can I help you with today?";
            }//end of if how are you

            // STEP 13 — Help
            if (input.Contains("help") || input.Contains("what can you do") ||
                input.Contains("commands"))
            {//start of if help
                return $"Here is what I can do {userName}:\n" +
                       "  💬 Cybersecurity tips — ask about passwords, phishing, scams etc\n" +
                       "  📋 'add task [name]' — Add a cybersecurity task\n" +
                       "  📋 'view tasks' — See all tasks\n" +
                       "  📋 'complete task [number]' — Mark task done\n" +
                       "  📋 'delete task [number]' — Remove a task\n" +
                       "  🎮 'start quiz' — Take the cybersecurity quiz\n" +
                       "  📜 'show log' — View recent activity";
            }//end of if help

            // STEP 14 — Goodbye
            if (input.Contains("bye") || input.Contains("goodbye") ||
                input.Contains("exit") || input.Contains("quit"))
            {//start of if goodbye
                AddToLog("User said goodbye");
                return $"Goodbye {userName}! 👋 Stay safe online! 🔒\n" +
                       "  Remember to always think before you click!";
            }//end of if goodbye

            // STEP 15 — Default error handling
            AddToLog($"Unrecognised input from {userName}");
            return $"I am not quite sure about that {userName}. 🤔\n" +
                   "  Try asking about: passwords, phishing, scams, privacy, malware\n" +
                   "  Or type 'help' to see all commands.";

        }//end of GenerateResponse

        //========================================
        // METHOD — HandleAddTask
        // NLP: Handles adding a new task
        //========================================
        private string HandleAddTask(string input)
        {//start of HandleAddTask

            // Extract task name from input
            string taskName = input;
            string[] removeWords = { "add task", "new task", "create task",
                "set task", "remind me to", "i need to", "add a task",
                "create a task", "new a task" };

            foreach (string word in removeWords)
            {//start of remove loop
                taskName = taskName.Replace(word, "").Trim();
            }//end of remove loop

            // If task name is empty use default
            if (string.IsNullOrEmpty(taskName))
            {//start of if empty
                taskName = "Cybersecurity task";
            }//end of if empty

            // Capitalise first letter
            if (taskName.Length > 0)
            {//start of if has content
                taskName = char.ToUpper(taskName[0]) + taskName.Substring(1);
            }//end of if has content

            // Create new task
            CyberTask newTask = new CyberTask
            {
                Id = taskList.Count + 1,
                Title = taskName,
                Description = $"Cybersecurity task: {taskName}",
                Reminder = "No reminder set",
                IsCompleted = false,
                DateAdded = DateTime.Now
            };

            taskList.Add(newTask);
            AddToLog($"Task added: {taskName}");

            return $"✅ Task added successfully!\n\n" +
                   $"  📋 Task #{newTask.Id}: {newTask.Title}\n" +
                   $"  📅 Added: {newTask.DateAdded.ToString("dd MMM yyyy")}\n\n" +
                   $"  Would you like to set a reminder? Type 'remind me in [X] days'";

        }//end of HandleAddTask

        //========================================
        // METHOD — ViewTasks
        // Displays all tasks
        //========================================
        private string ViewTasks()
        {//start of ViewTasks

            if (taskList.Count == 0)
            {//start of if no tasks
                return "📋 You have no tasks yet!\n" +
                       "  Type 'add task [name]' to add your first cybersecurity task.";
            }//end of if no tasks

            string result = $"📋 YOUR CYBERSECURITY TASKS ({taskList.Count} total):\n\n";

            foreach (CyberTask task in taskList)
            {//start of task loop
                string status = task.IsCompleted ? "✅ Completed" : "⏳ Pending";
                result += $"  [{task.Id}] {task.Title}\n";
                result += $"       Status: {status}\n";
                result += $"       Reminder: {task.Reminder}\n";
                result += $"       Added: {task.DateAdded.ToString("dd MMM yyyy")}\n\n";
            }//end of task loop

            result += "  To complete: 'complete task [number]'\n";
            result += "  To delete: 'delete task [number]'";

            AddToLog("User viewed tasks");
            return result;

        }//end of ViewTasks

        //========================================
        // METHOD — HandleCompleteTask
        // Marks a task as completed
        //========================================
        private string HandleCompleteTask(string input)
        {//start of HandleCompleteTask

            // Extract task number from input
            int taskNumber = ExtractNumber(input);

            if (taskNumber == -1)
            {//start of if no number
                return "Please specify a task number. Example: 'complete task 1'\n" +
                       "  Type 'view tasks' to see your task numbers.";
            }//end of if no number

            CyberTask task = taskList.Find(t => t.Id == taskNumber);

            if (task == null)
            {//start of if not found
                return $"Task #{taskNumber} not found. Type 'view tasks' to see your tasks.";
            }//end of if not found

            if (task.IsCompleted)
            {//start of if already done
                return $"Task #{taskNumber} '{task.Title}' is already completed! ✅";
            }//end of if already done

            task.IsCompleted = true;
            AddToLog($"Task completed: {task.Title}");

            return $"🎉 Great job {userName}!\n" +
                   $"  Task #{taskNumber} '{task.Title}' marked as completed! ✅\n" +
                   "  Keep up the great cybersecurity work!";

        }//end of HandleCompleteTask

        //========================================
        // METHOD — HandleDeleteTask
        // Deletes a task
        //========================================
        private string HandleDeleteTask(string input)
        {//start of HandleDeleteTask

            int taskNumber = ExtractNumber(input);

            if (taskNumber == -1)
            {//start of if no number
                return "Please specify a task number. Example: 'delete task 1'\n" +
                       "  Type 'view tasks' to see your task numbers.";
            }//end of if no number

            CyberTask task = taskList.Find(t => t.Id == taskNumber);

            if (task == null)
            {//start of if not found
                return $"Task #{taskNumber} not found. Type 'view tasks' to see your tasks.";
            }//end of if not found

            string taskTitle = task.Title;
            taskList.Remove(task);
            AddToLog($"Task deleted: {taskTitle}");

            return $"🗑️ Task #{taskNumber} '{taskTitle}' has been deleted.\n" +
                   "  Type 'view tasks' to see your remaining tasks.";

        }//end of HandleDeleteTask

        //========================================
        // METHOD — ExtractNumber
        // Extracts a number from user input
        //========================================
        private int ExtractNumber(string input)
        {//start of ExtractNumber

            string[] words = input.Split(' ');
            foreach (string word in words)
            {//start of word loop
                int number;
                if (int.TryParse(word, out number))
                {//start of if parsed
                    return number;
                }//end of if parsed
            }//end of word loop
            return -1;

        }//end of ExtractNumber

        //========================================
        // METHOD — StartQuiz
        // Starts the cybersecurity quiz
        //========================================
        private string StartQuiz()
        {//start of StartQuiz

            quizActive = true;
            quizScore = 0;
            currentQuestionIndex = 0;
            AddToLog("Quiz started");

            return "🎮 CYBERSECURITY QUIZ STARTED!\n\n" +
                   $"  You will be asked {quizQuestions.Count} questions.\n" +
                   "  Type the letter of your answer (A, B, C, or D)\n\n" +
                   GetCurrentQuestion();

        }//end of StartQuiz

        //========================================
        // METHOD — GetCurrentQuestion
        // Returns the current quiz question
        //========================================
        private string GetCurrentQuestion()
        {//start of GetCurrentQuestion

            if (currentQuestionIndex >= quizQuestions.Count)
            {//start of if done
                return EndQuiz();
            }//end of if done

            QuizQuestion q = quizQuestions[currentQuestionIndex];
            string questionText = $"  Question {currentQuestionIndex + 1} of {quizQuestions.Count}:\n\n";
            questionText += $"  {q.Question}\n\n";

            foreach (string option in q.Options)
            {//start of option loop
                questionText += $"  {option}\n";
            }//end of option loop

            return questionText;

        }//end of GetCurrentQuestion

        //========================================
        // METHOD — HandleQuizAnswer
        // Processes quiz answers
        //========================================
        private void HandleQuizAnswer(string input)
        {//start of HandleQuizAnswer

            if (currentQuestionIndex >= quizQuestions.Count)
            {//start of if done
                quizActive = false;
                AppendBotMessage(EndQuiz());
                return;
            }//end of if done

            QuizQuestion q = quizQuestions[currentQuestionIndex];

            // Map answer to index
            int answerIndex = -1;
            if (input.StartsWith("a")) answerIndex = 0;
            else if (input.StartsWith("b")) answerIndex = 1;
            else if (input.StartsWith("c")) answerIndex = 2;
            else if (input.StartsWith("d")) answerIndex = 3;

            if (answerIndex == -1)
            {//start of if invalid
                AppendBotMessage("Please answer with A, B, C, or D");
                return;
            }//end of if invalid

            string feedback = "";

            if (answerIndex == q.CorrectIndex)
            {//start of if correct
                quizScore++;
                feedback = $"✅ CORRECT! Well done {userName}!\n  {q.Explanation}";
                AddToLog($"Quiz Q{currentQuestionIndex + 1}: Correct answer");
            }//end of if correct
            else
            {//start of if wrong
                string correctAnswer = q.Options[q.CorrectIndex];
                feedback = $"❌ Not quite! The correct answer was: {correctAnswer}\n  {q.Explanation}";
                AddToLog($"Quiz Q{currentQuestionIndex + 1}: Wrong answer");
            }//end of if wrong

            currentQuestionIndex++;

            if (currentQuestionIndex >= quizQuestions.Count)
            {//start of if last question
                quizActive = false;
                AppendBotMessage(feedback + "\n\n" + EndQuiz());
            }//end of if last question
            else
            {//start of if more questions
                AppendBotMessage(feedback + "\n\n" + GetCurrentQuestion());
            }//end of if more questions

        }//end of HandleQuizAnswer

        //========================================
        // METHOD — EndQuiz
        // Shows final quiz results
        //========================================
        private string EndQuiz()
        {//start of EndQuiz

            quizActive = false;
            int total = quizQuestions.Count;
            int percentage = (quizScore * 100) / total;

            string result = $"🏆 QUIZ COMPLETE!\n\n";
            result += $"  Your Score: {quizScore}/{total} ({percentage}%)\n\n";

            if (percentage >= 80)
            {//start of if great
                result += "  🌟 Great job! You are a cybersecurity pro!";
            }//end of if great
            else if (percentage >= 60)
            {//start of if good
                result += "  👍 Good effort! Keep learning to stay safe online!";
            }//end of if good
            else
            {//start of if needs work
                result += "  📚 Keep learning to stay safe online! Try the quiz again!";
            }//end of if needs work

            AddToLog($"Quiz completed — Score: {quizScore}/{total} ({percentage}%)");
            return result;

        }//end of EndQuiz

        //========================================
        // METHOD — GetRandomResponse
        // Picks random non-repeating response
        //========================================
        private string GetRandomResponse(string topic)
        {//start of GetRandomResponse

            if (keywordResponses.ContainsKey(topic))
            {//start of if topic exists

                string[] responses = keywordResponses[topic];
                int lastIndex = -1;

                if (lastResponseIndex.ContainsKey(topic))
                {//start of if last index
                    lastIndex = lastResponseIndex[topic];
                }//end of if last index

                int newIndex;
                do
                {//start of do while
                    newIndex = random.Next(responses.Length);
                }//end of do
                while (newIndex == lastIndex && responses.Length > 1);

                lastResponseIndex[topic] = newIndex;
                return responses[newIndex];

            }//end of if topic exists

            return "Stay safe online and always be cautious!";

        }//end of GetRandomResponse

    }//end of class

}//end of namespace