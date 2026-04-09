using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TipMolde.API.Middleware;
using TipMolde.Core.Interface.Comercio.ICliente;
using TipMolde.Core.Interface.Comercio.IEncomenda;
using TipMolde.Core.Interface.Comercio.IEncomendaMolde;
using TipMolde.Core.Interface.Comercio.IFornecedor;
using TipMolde.Core.Interface.Comercio.IPedidoMaterial;
using TipMolde.Core.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial;
using TipMolde.Core.Interface.Desenho.IProjeto;
using TipMolde.Core.Interface.Desenho.IRegistoTempoProjeto;
using TipMolde.Core.Interface.Desenho.IRevisao;
using TipMolde.Core.Interface.Fichas.IFichaDocumento;
using TipMolde.Core.Interface.Fichas.IFichaProducao;
using TipMolde.Core.Interface.Producao.IFasesProducao;
using TipMolde.Core.Interface.Producao.IMaquina;
using TipMolde.Core.Interface.Producao.IMolde;
using TipMolde.Core.Interface.Producao.IPeca;
using TipMolde.Core.Interface.Producao.IRegistosProducao;
using TipMolde.Core.Interface.Relatorios;
using TipMolde.Core.Interface.Utilizador.IAuth;
using TipMolde.Core.Interface.Utilizador.ISecurity;
using TipMolde.Core.Interface.Utilizador.IUser;
using TipMolde.Infrastructure.DB;
using TipMolde.Infrastructure.Repositorio;
using TipMolde.Infrastructure.Service;
using TipMolde.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' nao configurada.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
          ?? throw new InvalidOperationException("Secao Jwt nao configurada.");
if (string.IsNullOrWhiteSpace(jwt.SecretKey) || jwt.SecretKey.Length < 32)
    throw new InvalidOperationException("Jwt:SecretKey deve ter pelo menos 32 caracteres.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var repo = context.HttpContext.RequestServices.GetRequiredService<IRevokedTokenRepository>();
                var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrWhiteSpace(jti) || await repo.IsRevokedAsync(jti))
                    context.Fail("Token revogado.");
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IEncomendaRepository, EncomendaRepository>();
builder.Services.AddScoped<IMoldeRepository, MoldeRepository>();
builder.Services.AddScoped<IFasesProducaoRepository, FasesProducaoRepository>();
builder.Services.AddScoped<IPecaRepository, PecaRepository>();
builder.Services.AddScoped<IRegistosProducaoRepository, RegistosProducaoRepository>();
builder.Services.AddScoped<IFornecedorRepository, FornecedorRepository>();
builder.Services.AddScoped<IPedidoMaterialRepository, PedidoMaterialRepository>();
builder.Services.AddScoped<IItemPedidoMaterialRepository, ItemPedidoMaterialRepository>();
builder.Services.AddScoped<IEncomendaMoldeRepository, EncomendaMoldeRepository>();
builder.Services.AddScoped<IMaquinaRepository, MaquinaRepository>();
builder.Services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IRevisaoRepository, RevisaoRepository>();
builder.Services.AddScoped<IRegistoTempoProjetoRepository, RegistoTempoProjetoRepository>();
builder.Services.AddScoped<IFichaProducaoRepository, FichaProducaoRepository>();
builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();
builder.Services.AddScoped<IFichaDocumentoRepository, FichaDocumentoRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IEncomendaService, EncomendaService>();
builder.Services.AddScoped<IMoldeService, MoldeService>();
builder.Services.AddScoped<IFasesProducaoService, FasesProducaoService>();
builder.Services.AddScoped<IPecaService, PecaService>();
builder.Services.AddScoped<IRegistosProducaoService, RegistosProducaoService>();
builder.Services.AddScoped<IFornecedorService, FornecedorService>();
builder.Services.AddScoped<IPedidoMaterialService, PedidoMaterialService>();
builder.Services.AddScoped<IEncomendaMoldeService, EncomendaMoldeService>();
builder.Services.AddScoped<IMaquinaService, MaquinaService>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddScoped<IRevisaoService, RevisaoService>();
builder.Services.AddScoped<IRegistoTempoProjetoService, RegistoTempoProjetoService>();
builder.Services.AddScoped<IFichaProducaoService, FichaProducaoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddScoped<IFichaDocumentoService, FichaDocumentoService>();

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
