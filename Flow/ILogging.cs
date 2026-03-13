using System.Text;
using System.Threading.Tasks;

namespace Flow
{
    public interface ILogging
    {
        void Log(string log);
        
        void Log(string path, string log, Encoding encoding = null);

        Task LogAsync(string log);
    }
}