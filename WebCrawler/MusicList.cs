using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{
    public class MusicList
    {
        public async Task RunAsync()
        {
            string path = "E:\\歌";

            string[] files = Directory.GetFiles(path, "*.mp3");

            var stream = new StreamWriter("E:\\歌\\歌名.txt", true, encoding: Encoding.UTF8);
            for (int i = 0; i < files.Length; i++)
            {
                await stream.WriteLineAsync(files[i].Replace(@"E:\歌\",""));
            }

            stream.Flush();
            stream.Close();
        }
    }
}
