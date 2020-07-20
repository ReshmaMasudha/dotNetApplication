using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TeleprompterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTeleprompter().Wait();
             static async Task ShowTeleprompter(TelePrompterConfig config)
              {
                var words = ReadFrom("sampleQuotes.txt");
                foreach (var word in words)
                {
                    Console.Write(word);
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        await Task.Delay(config.DelayInMilliseconds);
                    }
                }
                config.SetDone();
              }  

            static async Task RunTeleprompter()
            {
                var config = new TelePrompterConfig();
                var displayTask = ShowTeleprompter(config);

                var speedTask = GetInput(config);
                await Task.WhenAny(displayTask, speedTask);
            }

            static async Task GetInput(TelePrompterConfig config)
            {
                Action work = () =>
                {
                    do 
                    {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == '>')
                        config.UpdateDelay(-10);
                    else if (key.KeyChar == '<')
                        config.UpdateDelay(10);
                    else if (key.KeyChar == 'X' || key.KeyChar == 'x')
                        config.SetDone();
                    } while (!config.Done);
                };
                await Task.Run(work);
            }

            static IEnumerable<string> ReadFrom(string file)              //Enumerator method returns sequence lazily as requested or consumed by the code
            {
                string line;
                var lineLength = 0;
                using (var reader = File.OpenText(file))                 // var is implicitly typed local variable
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        var words = line.Split(' '); 
                        foreach (var word in words)
                        {
                            yield return word + " ";

                            lineLength += word.Length + 1;
                            if (lineLength > 70)
                            {
                                yield return Environment.NewLine;
                                lineLength = 0;
                            }
                        }
                        yield return Environment.NewLine;
                    }
                }
            }

              

           
        }


 
    }
}
