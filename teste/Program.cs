
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== Sistema de Login com MySQL ===");

        Console.Write("Usuário: ");
        string username = Console.ReadLine();

        Console.Write("Senha: ");
        string password = Console.ReadLine();

        Auth auth = new Auth();
        bool isAuthenticated = auth.Login(username, password);

        if (isAuthenticated)
        {
            Console.WriteLine("Login bem-sucedido!");
            MenuAluguelCarro();
        }
        else
        {
            Console.WriteLine("Usuário ou senha incorretos!");
        }

        Console.ReadLine();
    } 
    static void MenuAluguelCarro()
    {
        Aluguel aluguel = new Aluguel();

        while (true)
        {
            Console.WriteLine("\n=== Sistema de Aluguel de Carros ===");
            Console.WriteLine("1. Alugar Carro");
            Console.WriteLine("2. Ver Carros Alugados");
            Console.WriteLine("3. Carros Disponíveis");
            Console.WriteLine("4. Mostrar Todos os Carros");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha uma opção: ");
            int opcao = int.Parse(Console.ReadLine());

            switch (opcao)
            {
                case 1:
                    aluguel.MenuAluguelCarro(); // Chamando o método para alugar carro
                    break;
                case 2:
                    aluguel.VerCarrosAlugados();
                    break;
                case 3:
                    aluguel.VerCarrosDisponiveis();
                    break;
                case 4:
                    aluguel.MostrarTodosOsCarros();
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Opção inválida! Tente novamente.");
                    break;
            }
        }
    }
}