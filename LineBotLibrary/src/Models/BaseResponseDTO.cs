using System.Collections.Generic;

namespace LineBotLibrary.Models
{
    public class BaseResponseDTO
    {
        public Dictionary<string, IEnumerable<string>> Headers { get; set; }

        public int Status { get; set; }

        public string Message { get; set; }
    }
}
