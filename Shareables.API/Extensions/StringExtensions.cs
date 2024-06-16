namespace Shareables.API.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value) && 
                   !string.IsNullOrWhiteSpace(value);
        }
    }
}
