using System.Reflection;

namespace Phone_Book.Helpers
{
    internal static class Extensions
    {
        internal static TAttribute? GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();

        }

    }
}