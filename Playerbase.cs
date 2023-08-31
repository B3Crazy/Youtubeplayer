using Youtubeplayer;
using youtubeAudioplayer;

namespace Youtubeplayer
{
    internal class Playerbase
    {
        public async void run()
        {
            Work work = new();
            string? songname;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("starting Youtubeplayer 1.0.4...");
            Console.WriteLine(work.intelligentPath);
            Console.WriteLine();
            for (int i = 0; i < 101; i++)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 2);
                Console.Write($"Build {i}%");
                Task.Delay(30).Wait();
            }
            work.Speak("wilkommen zurück", 2, 100);
            Task.Delay(100).Wait();
            work.Speak("nennen sie ihren songwunsch", 2, 100);
            Console.WriteLine();

            var run = true;
            while (run)
            {
                Console.Write("Insert songname: ");
                Console.CursorVisible = true;
                Console.ForegroundColor = ConsoleColor.Green;
                songname = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.CursorVisible = false;
                if (songname != null && songname.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine();
                    Console.WriteLine("empty string!");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    if (songname != null && (songname.Equals("exit") || songname.Equals("Exit"))) { run = false; }
                    else if (songname != null && (songname.Equals("quit") || songname.Equals("Quit")))
                    {
                        run = false;
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("stopping application..."); Console.ResetColor();
                    }
                    else
                    {
                        if (songname != null) { work.Songname = songname; }
                        await work.GetAudio();
                        await work.Player();
                        if (work.queue.Count > 0)
                        {
                            await work.PlayQue();
                        }
                        Console.WriteLine("--------------------");
                        Console.WriteLine("   queue is empty");
                        Console.WriteLine("--------------------");
                    }
                }
            }
        }
    }
}
