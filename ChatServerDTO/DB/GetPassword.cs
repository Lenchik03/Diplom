namespace ProjectSystemAPI.DB
{
    public class GetPassword
    {
        public static string GetPass()
        {
            string pass = "";
            var r = new Random();
            while (pass.Length < 4)
            {
                Char c = (char)r.Next(33, 125);
                if (Char.IsLetterOrDigit(c))
                    pass += c;
            }
            return pass;
        }
    }
}
