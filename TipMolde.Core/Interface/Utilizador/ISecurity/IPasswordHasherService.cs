namespace TipMolde.Core.Interface.Utilizador.ISecurity
{
    public interface IPasswordHasherService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
        bool IsHash(string value);
    }
}
