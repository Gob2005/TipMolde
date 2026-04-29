using AutoMapper;
using FluentAssertions;
using TipMolde.Application.Dtos.UserDto;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities;

namespace TipMolde.Tests.Unitario.Mapping
{
    [TestFixture]
    [Category("Unit")]
    public class UserPasswordProfileTests
    {
        private IMapper _mapper = null!;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserPasswordProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        public void shouldHaveValidAutoMapperConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserPasswordProfile>();
            });

            config.AssertConfigurationIsValid();
        }

        [Test]
        public void shouldMapChangeUserPasswordDtoToUser()
        {
            var source = new ChangeUserPasswordDto
            {
                CurrentPassword = "Atual123!",
                NewPassword = "Nova123!"
            };

            var result = _mapper.Map<User>(source);

            result.Password.Should().Be("Nova123!");
        }

        [Test]
        public void shouldMapResetUserPasswordDtoToUser()
        {
            var source = new ResetUserPasswordDto
            {
                NewPassword = "Nova123!"
            };

            var result = _mapper.Map<User>(source);

            result.Password.Should().Be("Nova123!");
        }
    }
}
