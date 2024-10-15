using MySql.Data.MySqlClient;

public class Auth
{
    private DatabaseConnection dbConnection = new DatabaseConnection();

    public bool Login(string username, string password)
    {
        using (MySqlConnection conn = dbConnection.GetConnection())  // Use MySqlConnection
        {
            conn.Open();
            string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password";
            MySqlCommand cmd = new MySqlCommand(query, conn);  // Use MySqlCommand
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count == 1; // Se encontrar um usuário, retorna verdadeiro
        }
    }
}
