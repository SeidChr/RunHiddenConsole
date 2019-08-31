namespace PowerShellWindowHost.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> PrependExt<T>(this IEnumerable<T> enumerable, T entity)
        {
            yield return entity;
            foreach (var listEntity in enumerable)
            {
                yield return listEntity;
            }
        }

        public static IEnumerable<T> AppendExt<T>(this IEnumerable<T> enumerable, T entity)
        {
            foreach (var listEntity in enumerable)
            {
                yield return listEntity;
            }

            yield return entity;
        }

        public static IEnumerable<string> NotNullOrWhitespaceExt(this IEnumerable<string> strings)
            => strings?.Where(s => !string.IsNullOrWhiteSpace(s)) ?? Enumerable.Empty<string>();

        public static IEnumerable<string> TrimExt(this IEnumerable<string> strings)
            => strings.Select(s => s.Trim());

        public static IEnumerable<T> NotNullExt<T>(this IEnumerable<T> enumerable)
            => enumerable?.Where(s => s != null) ?? Enumerable.Empty<T>();
    }
}
