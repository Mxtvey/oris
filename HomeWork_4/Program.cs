using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using HttpServer.Shared;



try
{
    string settings = File.ReadAllText("settings.json");
    SettingsModel settingsModel = JsonSerializer.Deserialize<SettingsModel>(settings);
    var server = new miniHttpServer.HttpServer(settingsModel);
    server.Start();
    while (true)
        if (Console.ReadLine() == "/stop")
        {
            server.Stop();
            break;
        }
        else
        {
            Console.WriteLine("Unknown command");
        }
}
catch (Exception e) when (e is DirectoryNotFoundException or FileNotFoundException)
{
    Console.WriteLine("settings are not found");
}
catch (JsonException e)
{
    Console.WriteLine("settings.json is incorrect");
}
catch (Exception e)
{
    Console.WriteLine("There is an exception: " + e.Message);
}