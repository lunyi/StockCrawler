using System;
using System.Collections.Generic;
using System.IO;
using SimpleImpersonation;

namespace WebCrawler
{
    public class CopyImage
    {
        private Dictionary<int, string> _types = new Dictionary<int, string>
            {
                { 1, "daily"},
                { 2, "weekly"},
                { 3, "monthly"},
                { 4, "five-minutes"},
                { 5, "quarter-hour"},
                { 6, "half-hour"},
                { 7, "hour"}
            };

        public void Run()
        {
            var today = "2020-09-15";
            var path = $@"\\192.168.9.102\Deploy\photo\{today}\";
            var credentials = new UserCredentials("192.168.9.102", "admin", "53773222");
            var target = $@"G:\Deploy\BlazorWeb\wwwroot\photo\{today}";

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            foreach (var t in _types)
            {
                var result = Impersonation.RunAsUser(credentials, LogonType.NewCredentials, () =>
                {
                    var originalPath = $"{path}\\{t.Value}";
                    if (Directory.Exists(originalPath))
                    {
                        return Directory.GetFiles(originalPath);
                    }
                    return new string[] { };
                });

                var targetFolder = $@"{target}\\{t.Value}";

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                for (int i = 0; i < result.Length; i++)
                {
                    var s = result[i].Split('\\');
                    var targetPath = $@"{target}\\{s[7]}\\{s[8]}";

                    if (!File.Exists(targetPath))
                    {
                        File.Copy(result[i], targetPath);
                        Console.WriteLine(targetPath　+ " Copied..");
                    }
                }
            }       
        }
    }
}
