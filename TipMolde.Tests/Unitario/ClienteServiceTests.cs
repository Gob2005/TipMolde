using Moq;
using TipMolde.Core.Interface.ICliente;

namespace TipMolde.Tests.Unitario
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _clientes = new();
    }
}
