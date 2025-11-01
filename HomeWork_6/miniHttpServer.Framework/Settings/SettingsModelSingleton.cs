using System.Text.Json;

namespace HttpServer.Shared;

public class SettingsModelSingleton
{
    private static Lazy<SettingsModel> _instance =
        new Lazy<SettingsModel>(Config, LazyThreadSafetyMode.ExecutionAndPublication);
    
    public static SettingsModel Instance => _instance.Value;

    private static SettingsModel Config()
    {   
        try
        {
            string settings = File.ReadAllText("Settings/settings.json");
            SettingsModel settingsModel = JsonSerializer.Deserialize<SettingsModel>(settings);
            return settingsModel;
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
        return new SettingsModel();
    }
   
}