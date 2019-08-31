namespace PowerShellWindowHost.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class StringExtensions
    {
        public static IEnumerable<string> SplitExt(this string target, params char[] separators) 
            => target?.Split(separators) 
               ?? Enumerable.Empty<string>();
    }
}
