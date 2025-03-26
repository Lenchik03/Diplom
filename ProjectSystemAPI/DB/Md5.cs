using System.Text;

namespace ProjectSystemAPI.DB
{
    public class Md5
    {
        public static string HashPassword(string password)
        {
            var bytes = Encoding.ASCII.GetBytes(password);
            StringBuilder result = new StringBuilder();
            using (var md5 = System.Security.Cryptography.MD5.Create())
            using (var ms = new MemoryStream(bytes))
            {
                var hash = md5.ComputeHash(ms);
                foreach (var b in hash)
                    result.Append(b.ToString("x2"));
            }
            return result.ToString();
        }
    }
}
