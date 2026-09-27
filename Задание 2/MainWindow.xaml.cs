using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;

namespace UsersApp;

public partial class MainWindow : Window
{
    private List<UserRow> users = new();
    public MainWindow() => InitializeComponent();

    private string ConnectionString(bool withDatabase)
    {
        if (string.IsNullOrWhiteSpace(HostBox.Text) || string.IsNullOrWhiteSpace(UserBox.Text))
            throw new ArgumentException("Укажите сервер и пользователя MySQL.");
        if (!uint.TryParse(PortBox.Text, out uint port) || port < 1 || port > 65535)
            throw new ArgumentException("Порт должен быть числом от 1 до 65535.");
        return new MySqlConnectionStringBuilder
        {
            Server = HostBox.Text.Trim(), Port = port, UserID = UserBox.Text.Trim(),
            Password = PasswordBox.Password, Database = withDatabase ? "obuv_task2" : "",
            ConnectionTimeout = 5, DefaultCommandTimeout = 30,
            CharacterSet = "utf8mb4", Pooling = false
        }.ConnectionString;
    }

    private async void Connect_Click(object sender, RoutedEventArgs e) => await LoadAsync(false);
    private async void Import_Click(object sender, RoutedEventArgs e) => await LoadAsync(true);

    private async Task LoadAsync(bool import)
    {
        ConnectionPanel.IsEnabled = false;
        SearchBox.IsEnabled = false;
        users.Clear();
        UsersGrid.ItemsSource = null;
        StatusText.Text = import ? "Создание базы и импорт пользователей…" : "Подключение…";
        try
        {
            await using var connection = new MySqlConnection(ConnectionString(!import));
            await connection.OpenAsync();
            if (import)
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UsersApp.users.sql")
                    ?? throw new InvalidOperationException("Не найден встроенный SQL-файл.");
                using var reader = new StreamReader(stream);
                await using var command = new MySqlCommand(await reader.ReadToEndAsync(), connection);
                await command.ExecuteNonQueryAsync();
            }
            // Passwords and roles are never selected for the main screen.
            await using var select = new MySqlCommand("SELECT full_name, login FROM obuv_task2.users ORDER BY full_name, login;", connection);
            await using var result = await select.ExecuteReaderAsync();
            while (await result.ReadAsync())
                users.Add(new UserRow(result.GetString(0), result.GetString(1)));
            SearchBox.IsEnabled = true;
            ApplyFilter();
        }
        catch (MySqlException ex)
        {
            StatusText.Text = ex.Number switch
            {
                1045 => "Не удалось войти в MySQL. Проверьте пользователя и пароль MySQL.",
                1049 or 1146 => "База или таблица ещё не созданы. Нажмите «Создать БД и импортировать».",
                1044 or 1142 => "У пользователя MySQL недостаточно прав для этой операции.",
                _ => $"Ошибка MySQL ({ex.Number}). Проверьте, что сервер запущен, адрес и порт верны."
            };
        }
        catch (Exception ex) { StatusText.Text = "Не удалось загрузить пользователей: " + ex.Message; }
        finally { ConnectionPanel.IsEnabled = true; }
    }

    private void Search_Changed(object sender, TextChangedEventArgs e)
    {
        if (UsersGrid is not null && SearchBox.IsEnabled) ApplyFilter();
    }
    private void ApplyFilter()
    {
        var filtered = UserSearch.Filter(users, SearchBox.Text);
        UsersGrid.ItemsSource = filtered;
        StatusText.Text = filtered.Count == 0
            ? $"Совпадений нет. Всего пользователей: {users.Count}."
            : $"Показано: {filtered.Count} из {users.Count}.";
    }
    private void Clear_Click(object sender, RoutedEventArgs e) => SearchBox.Clear();
}
