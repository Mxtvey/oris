namespace HttpServer.Shared;

public class SettingsModel
{
    public string StaticDirectoryPath { get; set; }
    public string Domain { get; set; }
    public string Port { get; set; }
    
    public Dictionary<string, string> MimeType = new()
    {
        ["html"] = "text/html",
        ["txt"]  = "text/plain",
        ["json"] = "application/json",
        ["css"]  = "text/css",
        ["js"]   = "application/javascript",
        ["mjs"]  = "application/javascript",
        ["png"]  = "image/png",
        ["jpg"]  = "image/jpeg",
        ["jpeg"] = "image/jpeg",
        ["svg"]  = "image/svg+xml",
        ["webp"] = "image/webp",
        ["ico"]  = "image/x-icon",
      
    };
    
    public string ConnectionString { get; set; }


   
}