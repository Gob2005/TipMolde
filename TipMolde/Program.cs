using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TipMolde.API.Middleware;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Application.Interface.Comercio.IEncomenda;
using TipMolde.Application.Interface.Comercio.IEncomendaMolde;
using TipMolde.Application.Interface.Comercio.IFornecedor;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial;
using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto;
using TipMolde.Application.Interface.Desenho.IRevisao;
using TipMolde.Application.Interface.Fichas.IFichaDocumento;
using TipMolde.Application.Interface.Fichas.IFichaProducao;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Application.Interface.Producao.IMaquina;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Application.Interface.Producao.IRegistosProducao;
using TipMolde.Application.Interface.Relatorios;
using TipMolde.Application.Interface.Utilizador.IAuth;
using TipMolde.Application.Interface.Utilizador.ISecurity;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Application.Service;
using TipMolde.Infrastructure.DB;
using TipMolde.Infrastructure.Repositorio;
using TipMolde.Infrastructure.Service;
using TipMolde.Infrastructure.Settings;
using TipMolde.Application.Mappings;

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

// Repositories
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

// Application Services
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
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

// Infrastructure Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddScoped<IFichaDocumentoService, FichaDocumentoService>();
builder.Services.AddScoped<IFichaProducaoService, FichaProducaoService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);
builder.Services.AddAutoMapper(typeof(UserPasswordProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ClienteProfile).Assembly);
builder.Services.AddAutoMapper(typeof(EncomendaProfile).Assembly);
builder.Services.AddAutoMapper(typeof(EncomendaMoldeProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MoldeProfile).Assembly);


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
