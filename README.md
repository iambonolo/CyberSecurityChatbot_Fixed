\# CyberSecurity Awareness Chatbot - Part 1



\## Description

This is a command-line cybersecurity awareness chatbot built in C# as part of a university assignment.

The chatbot educates South African citizens about cybersecurity threats such as phishing,

password safety, and safe browsing practices.



\## Features

\- Voice greeting when the application launches

\- ASCII art logo displayed as a header

\- Personalised responses using the user's name

\- Cybersecurity topic responses including:

&#x20; - Password safety

&#x20; - Phishing awareness

&#x20; - Safe browsing

\- Input validation for unrecognised or empty inputs

\- Coloured console text for better readability

\- Typing effect for a conversational feel



\## Project Structure

CyberSecurityChatbot\_FIXED/

│

├── Program.cs            → Main entry point of the application

├── VoiceGreeting.cs      → Handles playing the WAV voice greeting

├── Logo.cs               → Displays the ASCII art logo

├── greet.wav             → Voice greeting audio file

├── logo.jpg              → Logo image reference

└── Properties/

&#x20;   └── AssemblyInfo.cs   → Project assembly information



\## How to Run the Program



\### Requirements

\- Windows operating system

\- Visual Studio 2019 or later

\- .NET Framework installed



\### Steps to Run

1\. Clone the repository from GitHub:

&#x20;  git clone https://github.com/iambonolo/CyberSecurityChatbot\_Fixed.git

2\. Open Visual Studio

3\. Click "Open a project or solution"

4\. Navigate to the cloned folder and open "CyberSecurityChatbot\_PART 1 FIXED.sln"

5\. Press F5 or click the "Start" button to run the application

6\. The chatbot will launch in the console window

7\. Follow the on-screen prompts to interact with the chatbot



\## GitHub Actions CI Workflow

This project uses GitHub Actions for Continuous Integration (CI).

Every time code is pushed to the repository, the workflow automatically:

\- Checks out the code

\- Sets up .NET

\- Restores dependencies

\- Builds the project



\### CI Workflow Green Checkmark Screenshot

!\[CI Workflow Success](ci-screenshot.png)

\## Author

\- GitHub: iambonolo



\## References

Pieterse, H. 2021. The Cyber Threat Landscape in South Africa: A 10-Year Review.

The African Journal of Information and Communication, 28(28).

Available at: https://www.scielo.org.za/scielo.php?pid=S2077-72132021000200003\&script=sci\_arttext

