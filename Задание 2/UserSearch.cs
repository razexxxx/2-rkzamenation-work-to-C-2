using System.Globalization;
namespace UsersApp;
public record UserRow(string FullName, string Login);
public static class UserSearch
{
    public static List<UserRow> Filter(IEnumerable<UserRow> users, string query)
    {
        var text = query.Trim();
        var culture = CultureInfo.GetCultureInfo("ru-RU").CompareInfo;
        return users.Where(u => text.Length == 0 || culture.IndexOf(u.FullName, text, CompareOptions.IgnoreCase) >= 0).ToList();
    }
}
