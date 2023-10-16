using NAudio.Wave;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace youtubeAudioplayer
{
    internal class Work
    {
        public string Songname { private get; set; }
        public Queue<string> queue = new();
        public string intelligentPath = Path.GetFullPath("audio.mp3");
        public string? video;

        public async Task GetAudio()
        {
            Console.WriteLine("searching...");
            var youtube = new YoutubeClient();
            Console.WriteLine(youtube);
            var searchresult = youtube.Search.GetResultsAsync("musik " + Songname);
            var Video = searchresult.ToBlockingEnumerable().FirstOrDefault();
            if (Video != null) { video = Video.ToString(); }
            Console.WriteLine(Video);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Video.Url);
            Console.WriteLine(streamManifest);
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            Console.WriteLine(streamInfo);
            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
            Console.WriteLine(stream);
            using (var fileStream = File.Create("audio.mp3")) { await stream.CopyToAsync(fileStream); }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"filepath = {intelligentPath}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        public async Task Player()
        {
            var audioplayer = new WaveOutEvent();
            try
            {
                var mp3File = new AudioFileReader(intelligentPath);
                audioplayer.DeviceNumber = 0;
                audioplayer.Init(mp3File);
                audioplayer.Play();

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Song not found");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("to type a command press enter");
            Console.CursorVisible = false;
            while (audioplayer.PlaybackState == PlaybackState.Playing || audioplayer.PlaybackState == PlaybackState.Paused)
            {
                if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("     type 'skip' to stop/ skip playback...");
                    Console.WriteLine("     type 'pause' or 'play' to pause/ resume playback...");
                    Console.WriteLine("     type 'download' to download the current song");
                    Console.WriteLine("     type 'queue' to queue another song");
                    Console.WriteLine("     press 'Enter' for stopping input typing");
                    Console.WriteLine();
                    Task.Delay(100).Wait();
                    Console.Write("Insert command: ");
                    Console.CursorVisible = true;
                    string? command = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    if (command != null && command.Length > 0)
                    {
                        Console.CursorVisible = false;
                        switch (command)
                        {
                            case "queue":

                                Console.Write("write next songname: ");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.CursorVisible = true;
                                var Newsongname = Console.ReadLine();
                                Console.CursorVisible = false;
                                Console.ForegroundColor = ConsoleColor.White;
                                if (Newsongname != null) { Que(Newsongname); }
                                break;
                            case "skip":

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Stopping playback...");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("--------------------");
                                audioplayer.Stop();
                                break;

                            case "pause":
                                if (audioplayer.PlaybackState == PlaybackState.Playing)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("paused playback... type 'play' to go on or 'skip' to skip current playback");
                                    Console.WriteLine();
                                    audioplayer.Pause();
                                }
                                break;
                            case "play":
                                if (audioplayer.PlaybackState == PlaybackState.Paused)
                                {
                                    audioplayer.Play();
                                    Console.WriteLine("playing...");
                                    Console.WriteLine();
                                }
                                break;
                            case "download":

                                Download();
                                break;
                            default:
                                Console.WriteLine($"{command} is not a known command");
                                break;
                        }
                    }
                    Console.CursorVisible = false;
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            audioplayer.Dispose();
            try { File.Delete("audio.mp3"); Console.WriteLine(" audio.mp3 deleted!"); } catch (FileNotFoundException) { Console.WriteLine("no file deleted"); }
        }

        public async Task Download()
        {
            var youtube = new YoutubeClient();
            var searchresult = youtube.Search.GetResultsAsync("musik" + Songname);
            var Video = searchresult.ToBlockingEnumerable().FirstOrDefault();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"downloading '{Video}'");
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Video.Url);
            Console.WriteLine(streamManifest);
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            Console.WriteLine(streamInfo);
            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
            Console.WriteLine(stream);
            using (var fileStream = File.Create($"{Video}.mp3")) { await stream.CopyToAsync(fileStream); }
            Console.WriteLine($"Download of {Video}.mp3 completed...");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public void Que(string songname)
        {
            queue.Enqueue(songname);
            Console.WriteLine($"{songname} is place {queue.Count} in the queue");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public async Task PlayQue()
        {
            while (queue.Count > 0)
            {
                Songname = queue.Dequeue();
                await GetAudio();
                await Player();
            }
        }

        public void Speak(string speech, int speed, int Volume)
        {
            using SpeechSynthesizer synth = new();
            synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            synth.Rate = speed;
            synth.Volume = Volume;
            synth.Speak(speech);
        }
    }
}