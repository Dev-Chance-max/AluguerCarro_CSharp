using MySql.Data.MySqlClient;
using System;

public class Aluguel
{
    private DatabaseConnection dbConnection;

    public Aluguel()
    {
        dbConnection = new DatabaseConnection();
    }

    public void RealizarAluguel(int clienteId, int carroId, int duracaoDias)
    {
        using (var conn = dbConnection.GetConnection())
        {
            conn.Open();

            // Verificar se o carro está disponível
            string checkCarroQuery = "SELECT Disponivel FROM Carros WHERE CarroID = @CarroID";
            MySqlCommand checkCarroCmd = new MySqlCommand(checkCarroQuery, conn);
            checkCarroCmd.Parameters.AddWithValue("@CarroID", carroId);
            bool? isDisponivel = (bool?)checkCarroCmd.ExecuteScalar();

            if (isDisponivel == null)
            {
                Console.WriteLine("Carro não encontrado.");
                return;
            }

            if (!isDisponivel.Value)
            {
                Console.WriteLine("O carro não está disponível para aluguel.");
                return;
            }

            // Registrar o aluguel
            string insertAluguelQuery = "INSERT INTO Alugueis (ClienteID, CarroID, DataInicio, DataFim, Status) VALUES (@ClienteID, @CarroID, @DataInicio, @DataFim, 'Ativo')";
            MySqlCommand insertAluguelCmd = new MySqlCommand(insertAluguelQuery, conn);
            insertAluguelCmd.Parameters.AddWithValue("@ClienteID", clienteId);
            insertAluguelCmd.Parameters.AddWithValue("@CarroID", carroId);
            insertAluguelCmd.Parameters.AddWithValue("@DataInicio", DateTime.Now);
            insertAluguelCmd.Parameters.AddWithValue("@DataFim", DateTime.Now.AddDays(duracaoDias));

            insertAluguelCmd.ExecuteNonQuery();

            // Atualizar o status do carro
            string updateCarroQuery = "UPDATE Carros SET Disponivel = FALSE WHERE CarroID = @CarroID";
            MySqlCommand updateCarroCmd = new MySqlCommand(updateCarroQuery, conn);
            updateCarroCmd.Parameters.AddWithValue("@CarroID", carroId);
            updateCarroCmd.ExecuteNonQuery();

            Console.WriteLine("Carro alugado com sucesso!");
        }
    }
    public void MenuAluguelCarro()
    {
        while (true)
        {
            Console.WriteLine("\n=== Menu de Aluguel de Carros ===");
            Console.Write("Digite o ID do Cliente (ou 0 para voltar): ");
            int clienteId = int.Parse(Console.ReadLine());

            if (clienteId == 0)
            {
                break; // Sai do menu se o usuário digitar 0
            }

            Console.Write("Digite o ID do Carro: ");
            int carroId = int.Parse(Console.ReadLine());

            Console.Write("Digite a duração do aluguel em dias: ");
            int duracaoDias = int.Parse(Console.ReadLine());

            // Tenta realizar o aluguel
            RealizarAluguel(clienteId, carroId, duracaoDias);
        }
    }

    public void VerCarrosAlugados()
    {
        using (var conn = dbConnection.GetConnection())
        {
            conn.Open();

            string query = @"
                SELECT a.AluguelID, c.CarroID, c.Modelo, c.Marca, a.DataInicio, a.DataFim, cl.Nome AS NomeCliente
                FROM Alugueis a
                JOIN Carros c ON a.CarroID = c.CarroID
                JOIN Clientes cl ON a.ClienteID = cl.ClienteID
                WHERE a.Status = 'Ativo'";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\n=== Lista de Carros Alugados ===");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int aluguelId = reader.GetInt32("AluguelID");
                    int carroId = reader.GetInt32("CarroID");
                    string modelo = reader.GetString("Modelo");
                    string marca = reader.GetString("Marca");
                    DateTime dataInicio = reader.GetDateTime("DataInicio");
                    DateTime dataFim = reader.GetDateTime("DataFim");
                    string nomeCliente = reader.GetString("NomeCliente");

                    Console.WriteLine($"\nAluguel ID: {aluguelId}");
                    Console.WriteLine($"Carro ID: {carroId}");
                    Console.WriteLine($"Modelo: {marca} {modelo}");
                    Console.WriteLine($"Cliente: {nomeCliente}");
                    Console.WriteLine($"Data de Início: {dataInicio}");
                    Console.WriteLine($"Data de Término: {dataFim}");
                    Console.WriteLine("--------------------------");
                }
            }
            else
            {
                Console.WriteLine("Nenhum carro alugado no momento.");
            }
        }
    }

    public void VerCarrosDisponiveis()
    {
        using (var conn = dbConnection.GetConnection())
        {
            try
            {
                // Abre a conexão com o banco de dados
                conn.Open();

                // Consulta SQL para obter carros que não estão alugados
                string query = @"
                    SELECT c.CarroID, c.Modelo, c.Marca, c.Ano, PrecoDia
                    FROM Carros c
                    WHERE c.CarroID NOT IN (
                        SELECT a.CarroID FROM Alugueis a WHERE a.Status = 'Ativo'
                    )";

                // Executa o comando SQL
                MySqlCommand cmd = new MySqlCommand(query, conn);

                // Executa o MySqlDataReader
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n=== Lista de Carros Disponíveis ===");

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int carroId = reader.GetInt32("CarroID");
                            string modelo = reader.GetString("Modelo");
                            string marca = reader.GetString("Marca");
                            int ano = reader.GetInt32("Ano");
                            decimal precoDia = reader.GetDecimal("PrecoDia");

                            Console.WriteLine($"\nCarro ID: {carroId}");
                            Console.WriteLine($"Modelo: {marca} {modelo}");
                            Console.WriteLine($"Ano: {ano}");
                            Console.WriteLine($"Preço por Dia: {precoDia:C}");
                            Console.WriteLine("--------------------------");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nenhum carro disponível no momento.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao tentar listar carros disponíveis: {ex.Message}");
            }
            finally
            {
                // Fecha a conexão
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
    public void MostrarTodosOsCarros()
    {
        using (var conn = dbConnection.GetConnection())
        {
            try
            {
                conn.Open();

                // Consulta para obter todos os carros
                string query = @"
                SELECT CarroID, Modelo, Marca, Ano, PrecoDia 
                FROM Carros";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n=== Lista de Todos os Carros ===");

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int carroId = reader.GetInt32("CarroID");
                            string modelo = reader.GetString("Modelo");
                            string marca = reader.GetString("Marca");
                            int ano = reader.GetInt32("Ano");
                            decimal precoDia = reader.GetDecimal("PrecoDia");

                            Console.WriteLine($"\nCarro ID: {carroId}");
                            Console.WriteLine($"Modelo: {marca} {modelo}");
                            Console.WriteLine($"Ano: {ano}");
                            Console.WriteLine($"Preço por Dia: {precoDia:C}");
                            Console.WriteLine("--------------------------");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nenhum carro registrado no sistema.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao tentar listar todos os carros: {ex.Message}");
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }

}