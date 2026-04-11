using System;
using System.Media;

namespace CyberSecurityChatbot
{//start of namespace

    public class VoiceGreeting
    {//start of class

        //get base directory
        string path = AppDomain.CurrentDomain.BaseDirectory;

        //constructor
        public VoiceGreeting()
        {//start constructor

            voice();

        }//end constructor


        //method to play voice
        private void voice()
        {//start method

            //fix path to audio file
            string fullpath = path.Replace(@"\bin\Debug\", @"\greet.wav");

            //create sound player
            SoundPlayer voice_play = new SoundPlayer(fullpath);

            //load audio
            voice_play.Load();

            //play audio
            voice_play.PlaySync();

        }//end method

    }//end class

}//end namespace