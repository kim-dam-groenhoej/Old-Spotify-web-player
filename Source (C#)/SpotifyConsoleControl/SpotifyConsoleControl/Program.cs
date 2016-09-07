using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;
using Microsoft.Win32;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpotifyConsoleControl
{
    class Program
    {
        /// <summary>
        /// Check Spotify Web Player MVC data timer
        /// </summary>
        private static System.Timers.Timer _timer;

        /// <summary>
        /// location of the Web Application
        /// </summary>
        private static string _webPlayerLocation;

        /// <summary>
        /// Random Guid ID to not repeating everytime checking JSON data
        /// </summary>
        private static string _cacheSongID;

        static void Main(string[] args)
        {
            // Create timer
            _timer = new System.Timers.Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnCheckWebPlayerList); // Add event handler
            _timer.Interval = 2000; // interval in milliseconds

            Console.WriteLine("Type the http website location");
            Console.WriteLine("fx. http://mydomain.net/SpotifyWebPlayer");
            Console.WriteLine("-------------------------------------");
            string line;

            // wait for typing domain location path for the MVC web player project
            do
            {
                Console.Write("URL:   ");
                line = Console.ReadLine();

                if (line != null)

                Console.WriteLine("   ");
                Console.WriteLine("You typed: " + line);

                // what kind of http protocol?
                if (line.Contains("http://") || line.Contains("https://"))
                {
                    string json = null;

                    try
                    {
                        // Create JSON url
                        line = line + "/Home/GetSong";

                        Console.WriteLine("   ");
                        Console.WriteLine("   Loading... - '" + line + "'");
                        Console.WriteLine("   ");

                        // download json string from MVC application (web player)
                        json = new WebClient().DownloadString(line); 
                    }
                    catch (Exception ex)
                    {
                        // write error messages
                        Console.WriteLine("   ");
                        Console.WriteLine("Failed!");
                        Console.WriteLine(" - " + ex.Message);
                        Console.WriteLine("   ");
                    }

                    // no json, no success
                    if (!string.IsNullOrEmpty(json))
                    {
                        // validate json
                        JObject o = JObject.Parse(json);
                        bool IsSuccess = (bool)((JObject)o["data"])["success"];

                        if (IsSuccess)
                        {
                            // IS VALID: save URL to MVC application (web player)
                            _webPlayerLocation = line;

                            // run timer
                            _timer.Enabled = true;

                            Console.WriteLine("   ");
                            Console.WriteLine("Success!");
                            Console.WriteLine(" - The application is now running");
                            Console.WriteLine("   ");
                            Console.WriteLine("   ");
                            Console.WriteLine("   ");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("   ");
                        Console.WriteLine("Failed!");
                        Console.WriteLine(" - No data returned");
                        Console.WriteLine("   ");
                    }
                }
                else
                {
                    Console.WriteLine("   ");
                    Console.WriteLine("Failed!");
                    Console.WriteLine(" - Please insert http:// or https:// first");
                    Console.WriteLine("   ");
                }

                line = null;
            } while (line != null);


            // information for the user
            Console.WriteLine("This application needs to run for using unofficial Spotify Web Player");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Press enter to close this application");

            // Force not to close console application
            Console.ReadKey(true);

        }

        public static void OnCheckWebPlayerList(Object source, ElapsedEventArgs e)
        {
            // download json string from MVC application (web player)
            string json = new WebClient().DownloadString(_webPlayerLocation);
            JObject o = JObject.Parse(json);
            JObject data = (JObject)o["data"];

            bool IsSuccess = (bool)data["success"];
            if (IsSuccess)
            {
                // Get song from JSON
                JArray songs = (JArray)data["rows"];
                string songID = (String)songs.First["ID"];
                string songSpotifyURL = (String)songs.First["SpotifyURL"];

                // Dont let the timer repeating the song (Only play a song with a new ID)
                if (songID != _cacheSongID)
                {
                    // Cache song url
                    _cacheSongID = songID;

                    // find default spotify location on the machine in Registry
                    RegistryKey spotifyKeyFolder = Registry.ClassesRoot.OpenSubKey("spotify\\DefaultIcon");
                    string spotifyPath = Convert.ToString(spotifyKeyFolder.GetValue(""));
                    // remove unnecessary data in Path
                    spotifyPath = spotifyPath.Substring(1, spotifyPath.Length - 4);

                    // Check spotify is open and get progress
                    var spotifyProgress = Process.GetProcesses().SingleOrDefault(p => p.ProcessName.ToLower() == "spotify");

                    if (spotifyProgress != null)
                    {
                        // note what song is playing in console
                        Console.WriteLine("    ");
                        Console.WriteLine("Playing song: " + songSpotifyURL);

                        // send song 'Spotify-URL' to Spotify progress
                        spotifyProgress.StartInfo.Arguments = songSpotifyURL;
                        spotifyProgress.StartInfo.FileName = spotifyPath;

                        // reuse Spotify progress
                        spotifyProgress.Start();

                        spotifyProgress.Close();
                    }
                    else
                    {
                        Console.WriteLine("    ");
                        Console.WriteLine("Spotify is not running...");
                        Console.WriteLine("    ");
                    }

                }
            }

        }
    }
}



/// <summary>
/// Helper for sending Spotify messages
/// http://stackoverflow.com/questions/8459162/user32-api-custom-postmessage-for-automatisation
/// </summary>
internal class Win32
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd">Insert MainWindowHandle from a progress</param>
    /// <param name="Msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam">Messages type from SpotifyAction enum (fx. new IntPtr((long)SpotifyAction.Stop)  )</param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    internal class Constants
    {
        internal const uint WM_APPCOMMAND = 0x0319;
    }
}

/// <summary>
/// Messages you can use
/// </summary>
public enum SpotifyAction : long
{
    PlayPause = 917504,
    Mute = 524288,
    VolumeDown = 589824,
    VolumeUp = 655360,
    Stop = 851968,
    PreviousTrack = 786432,
    NextTrack = 720896
}