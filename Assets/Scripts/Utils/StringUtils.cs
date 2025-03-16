namespace Utils
{
    public static class StringUtils
    {
        public static bool IsLetter(string str)
        {
            if (str.Length != 1)
                return false;

            char key = str.ToUpper()[0];

            return key >= 'A' && key <= 'Z';
        }
    }
}
