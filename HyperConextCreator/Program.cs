// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net;
using System.Text;

string windowsUsername = Environment.UserName;
string tmpFileName = "temp.reg";
string hyperPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs\\Hyper") + "\\Hyper.exe";
string documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ConsoleStartup");
if (!Directory.Exists(documentsPath))
{
    Directory.CreateDirectory(documentsPath);
}

if (File.Exists("temp.reg"))
{
    File.Delete("temp.reg");
}

Console.WriteLine("Creating temp.reg");
using (StreamWriter sw = File.CreateText(tmpFileName))    
{      
    sw.WriteLine("Windows Registry Editor Version 5.00");
    sw.WriteLine("");
    sw.WriteLine(@"[HKEY_CLASSES_ROOT\Directory\Background\shell\HyperAdmin]");
    sw.WriteLine("\"icon\"=\"C:\\\\Users\\\\" + windowsUsername + "\\\\AppData\\\\Local\\\\Programs\\\\Hyper\\\\Hyper.exe\"");
    sw.WriteLine("@=\"Open Hyper here (Admin)\"");
    sw.WriteLine(@"[HKEY_CLASSES_ROOT\Directory\Background\shell\HyperAdmin\Command]");
    sw.WriteLine("@=\"C:\\\\Users\\\\" + windowsUsername + "\\\\Documents\\\\ConsoleStartup\\\\CustomUACLauncher.exe -app \\\"C:\\\\Users\\\\" + windowsUsername + "\\\\AppData\\\\Local\\\\Programs\\\\Hyper\\\\Hyper.exe\\\" -path \\\"%V\\\"\"");
    sw.WriteLine(@"");
    sw.WriteLine(@"[HKEY_CLASSES_ROOT\Directory\shell\HyperAdmin]");
    sw.WriteLine("\"icon\"=\"C:\\\\Users\\\\" + windowsUsername + "\\\\AppData\\\\Local\\\\Programs\\\\Hyper\\\\Hyper.exe\"");
    sw.WriteLine("@=\"Open Hyper here (Admin)\"");
    sw.WriteLine(@"[HKEY_CLASSES_ROOT\Directory\shell\HyperAdmin\Command]");
    sw.WriteLine("@=\"C:\\\\Users\\\\" + windowsUsername + "\\\\Documents\\\\ConsoleStartup\\\\CustomUACLauncher.exe -app \\\"C:\\\\Users\\\\" + windowsUsername + "\\\\AppData\\\\Local\\\\Programs\\\\Hyper\\\\Hyper.exe\\\" -path \\\"%V\\\"\"");
}

Process p = new Process();
p.StartInfo = new ProcessStartInfo(tmpFileName)
{
    UseShellExecute = true
};
p.Start();

Console.WriteLine("Downloading CustomUACLauncher.exe");
using (var client = new WebClient())
{
    client.DownloadFile(@"https://github.com/WeeXnes/CustomUACLauncher/releases/download/1.1/CustomUACLauncher.exe","CustomUACLauncher.exe");
    File.Move("CustomUACLauncher.exe", documentsPath + "\\CustomUACLauncher.exe", true);
}



if(File.Exists(documentsPath + "\\startup.ps1"))
    File.Delete(documentsPath + "\\startup.ps1");


Console.WriteLine("Creating startup.ps1");
using (StreamWriter sw = File.CreateText(documentsPath + "\\startup.ps1"))    
{      
    sw.WriteLine("dotnet \"" + documentsPath + "\\ConsoleStartupApp.dll\"");
    sw.WriteLine("Invoke-Expression (&starship init powershell)");
}    


using (var client = new WebClient())
{
    client.DownloadFile(@"https://github.com/WeeXnes/ConsoleStartupApp/releases/download/1.0/ConsoleStartup.zip","ConsoleStartup.zip");
}

System.IO.Compression.ZipFile.ExtractToDirectory("ConsoleStartup.zip", documentsPath, true);

using (var client = new WebClient())
{
    string configfolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.config";
    if (!Directory.Exists(configfolder))
        Directory.CreateDirectory(configfolder);
    client.DownloadFile(@"https://raw.githubusercontent.com/WeeXnes/dotconfigfiles/master/starship.toml","starship.toml");
    File.Move("starship.toml",  configfolder + "\\starship.toml", true);
}




using (var client = new WebClient())
{
    client.DownloadFile(@"https://raw.githubusercontent.com/WeeXnes/dotconfigfiles/master/.hyper.js","tmphyper.txt");
}
string text = File.ReadAllText("tmphyper.txt");
string filepatorg = documentsPath + "\\startup.ps1";
text = text.Replace(@"C:\\Users\\wxc85\\Desktop\\startup.ps1", filepatorg.Replace(@"\", @"\\"));
File.WriteAllText(".hyper.js", text);
string hyperfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hyper";
File.Move(".hyper.js",  hyperfolder + "\\.hyper.js", true);
