using System;
using System.Media;

namespace CyberSecurityChatbot
{//start of namespace

    class Program
    {//start of class

        // Main method - this is where the program starts running
        static void Main(string[] args)
        {//start of main

            //create and display ASCII logo at the top of the program
            Logo logo = new Logo();
            logo.Display();

            // Play voice greeting when the program starts
            SoundPlayer player = new SoundPlayer("greet.wav");
            player.PlaySync();   // Plays and waits until finished

            //display welcome message
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n Hello welcome to the Cyber Security Assistant!");
            Console.ResetColor();

            //ask the user to enter their name
            Console.Write("\nPlease enter your name: ");
            string name = Console.ReadLine();

            //greet the user in a friendly way
            Console.WriteLine("\nHello " + name + "!");
            Console.WriteLine("I'm here to help you understand how to stay safe online and protect your information.");

            //start chatbot loop so the program keeps running until the user exits
            while (true)
            {//start of loop

                //ask the user to enter a question
                Console.Write("\nAsk me anything about cyber security (or type 'exit' to quit): ");
                string userInput = Console.ReadLine().ToLower();
                // Check if user entered nothing
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("I didn't quite understand that. Could you rephrase?s.");
                    continue; // go back to the start of the loop
                }

                //check if user wants to exit the program
                if (userInput == "exit")
                {
                    Console.WriteLine("\nGoodbye " + name + "! Stay safe and take care online.");
                    break; //exit the loop and end program
                }

                //check for password-related questions
                if (userInput.Contains("password"))
                {
                    Console.WriteLine("A strong password should include letters, numbers, and symbols.");
                    Console.WriteLine("Avoid using personal information like your name or birthdate.");
                }

                //check for phishing-related questions
                else if (userInput.Contains("phishing"))
                {
                    Console.WriteLine("Phishing is when attackers try to trick you into giving personal information.");
                    Console.WriteLine("Always check links and email addresses carefully.");
                }

                // Check if the user is asking about safe browsing
                else if (userInput.Contains("safe browsing"))
                {
                    Console.WriteLine("Always check if a website is secure before entering personal information.");
                    Console.WriteLine("Look for 'https' and avoid clicking on suspicious pop-ups.");
                }
                // Check if the user is asking about scams
                else if (userInput.Contains("scam"))
                {
                    Console.WriteLine("Be careful of messages that ask for urgent action or personal details.");
                    Console.WriteLine("If something feels suspicious, it is always better to ignore or verify it.");
                }

                //check for virus or malware-related questions
                else if (userInput.Contains("virus") || userInput.Contains("malware"))
                {
                    Console.WriteLine("\nThat's an important topic.");
                    Console.WriteLine("Viruses and malware can harm your computer and steal your data.");
                    Console.WriteLine("Make sure you install antivirus software and avoid downloading files from unknown sources.");
                }

                //check for general safety or security questions
                else if (userInput.Contains("safe") || userInput.Contains("security"))
                {
                    Console.WriteLine("\nStaying safe online is very important.");
                    Console.WriteLine("Always keep your software updated and avoid using public Wi-Fi for sensitive activities.");
                }

                //default response if the chatbot does not understand the question
                else
                {
                    Console.WriteLine("\nI didn't quite understand that. Could you rephrase?");
                    Console.WriteLine("Try asking about password, phishing, or safe browsing.");
                }

            }//end of loop
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();

        }//end of main

    }//end of class

}//end of namespace