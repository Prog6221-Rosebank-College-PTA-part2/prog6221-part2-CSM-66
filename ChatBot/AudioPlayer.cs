using System;
using System.Collections.Generic;
using System.Media;
using System.Text;

namespace ChatBOT.ChatBot
{
     class AudioPlayer
    {
        public static void PlayVoiceGreeting()
        {
            try
            {
                // Build the full file path to the audio file in the application's directory
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                // Check if the audio file exists before attempting to play it
                if (File.Exists(filePath))
                {
                    // Create a SoundPlayer object and play the audio synchronously
                    SoundPlayer player = new SoundPlayer(filePath);
                    player.PlaySync();
                }
                else
                {
                    // Inform the user if the file cannot be found
                    Console.WriteLine("Audio file not found: " + filePath);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during audio playback
                Console.WriteLine("Error playing sound: " + ex.Message);
            }
        }
    }
}
