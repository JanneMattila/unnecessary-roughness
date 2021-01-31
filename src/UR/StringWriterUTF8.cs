using System.IO;
using System.Text;

namespace UR
{
    public class StringWriterUTF8 : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
