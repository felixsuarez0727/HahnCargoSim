using HahnCargoSim.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using File = System.IO.File;
using System.Threading;

namespace HahnCargoSim.Services
{
  public class LoggerService : ILoggerService
  {
    static readonly object locker = new object();
    public async void Log(string message)
    {
      // Imprimir el mensaje en la consola
      Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()}: {message}");

      var count = 0;
      do
      {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HahnCargoSimLog.txt");
        if(filePath.IsNullOrEmpty()) break;

        try
        {
          lock (locker)
          {
            File.AppendAllText(filePath, $"{DateTime.UtcNow.ToLongTimeString()}: {message}\n");
            break;
          }
        }
        catch
        {
          // ignored
        }

        Thread.Sleep(new Random().Next(10, 100));
        count++;
      } while (count < 5);
    }
  }
}
