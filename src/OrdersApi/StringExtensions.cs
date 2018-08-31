namespace OrdersApi
{
    public static class StringExtensions
    {
        public static bool ToBool(this string input)
        {
            bool.TryParse(input, out var result);

            return result;
        }

        public static int ToInt(this string input)
        {
            int.TryParse(input, out var result);

            return result;
        }
    }
}