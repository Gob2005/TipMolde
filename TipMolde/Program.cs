using MySql.Data.MySqlClient;

string connectionString = "Server=localhost;Port=3306;Database=tipmolde;Uid=root;Pwd=password;";
using (MySqlConnection conn = new MySqlConnection(connectionString))
{
    try
    {
        conn.Open();
        Console.WriteLine("Conexão estabelecida com sucesso à TipMolde!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro: " + ex.Message);
    }
}