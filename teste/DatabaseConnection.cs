using MySql.Data.MySqlClient;

public class DatabaseConnection
{
    private string connectionString = "Server=localhost;Port=3306;Database=teste;Uid=root;Pwd=chance;";

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);  // Use MySqlConnection
    }
}

