using System.IO;
using System.Text;

namespace ProfSite.Utils
{
    class MultipartWriter
    {
        static readonly byte[] seperatorBytes = Encoding.UTF8.GetBytes("--");
        static readonly byte[] newlineBytes = Encoding.UTF8.GetBytes("\r\n");

        byte[] boundaryBytes;
        Stream stream;
        public MultipartWriter(Stream stream, string boundary)
        {
            this.stream = stream;
            this.boundaryBytes = Encoding.UTF8.GetBytes(boundary);
        }

        public MultipartWriter Write(byte[] data) { stream.Write(data, 0, data.Length); return this; }
        public MultipartWriter Write(string s) { Write(Encoding.UTF8.GetBytes(s)); return this; }
        public MultipartWriter WriteBoundary() { Write(boundaryBytes); return this; }
        public MultipartWriter WriteSeperator() { Write(seperatorBytes); return this; }
        public MultipartWriter WriteNewline() { Write(newlineBytes); return this; }
    }
}