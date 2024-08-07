using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Tochka.JsonRpc.OpenRpc;
using Tochka.JsonRpc.OpenRpc.Models;
using Tochka.JsonRpc.Server.Extensions;
using Tochka.JsonRpc.Server.Serialization;
using Tochka.JsonRpc.Server.Settings;
using Tochka.JsonRpc.Swagger;
using Tochka.JsonRpc.Tests.WebApplication;
using Tochka.JsonRpc.Tests.WebApplication.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(static options =>
{
    options.Filters.Add<BusinessLogicExceptionWrappingFilter>();
    options.Filters.Add<BusinessLogicExceptionHandlingFilter>();
});
builder.Services.AddJsonRpcServer(static options => options.DefaultMethodStyle = JsonRpcMethodStyle.ActionOnly);

// "business logic"
builder.Services.AddScoped<IResponseProvider, SimpleResponseProvider>();
builder.Services.AddScoped<IRequestValidator, SimpleRequestValidator>();
builder.Services.AddScoped<IBusinessLogicExceptionHandler, BusinessLogicExceptionHandler>();

// custom serializers for requests
builder.Services.AddSingleton<IJsonSerializerOptionsProvider, SnakeCaseJsonSerializerOptionsProvider>();
builder.Services.AddSingleton<IJsonSerializerOptionsProvider, CamelCaseJsonSerializerOptionsProvider>();
builder.Services.AddSingleton<IJsonSerializerOptionsProvider, KebabCaseUpperJsonSerializerOptionsProvider>();

builder.Services.AddSwaggerWithJsonRpc(Assembly.GetExecutingAssembly()); // swagger for json-rpc
builder.Services.ConfigureSwaggerGen(static c => // swagger for REST
{
    c.SwaggerDoc("rest", new OpenApiInfo { Title = "RESTful API", Version = "v1" });
    c.SwaggerDoc("custom_v1", new OpenApiInfo { Title = "Custom group", Version = "v1" });
});

builder.Services.AddOpenRpc(Assembly.GetExecutingAssembly()); // OpenRpc
builder.Services.Configure<OpenRpcOptions>(static o => o.OpenRpcDoc("custom_v1", new OpenRpcInfo("Custom group", "v1")));

// auth
builder.Services.AddAuthentication(AuthConstants.SchemeName)
    .AddScheme<ApiAuthenticationOptions, ApiAuthenticationHandler>(AuthConstants.SchemeName, null);
builder.Services.AddAuthorization();

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

app.UseAuthentication();
app.UseSwaggerUI(c =>
{
    c.JsonRpcSwaggerEndpoints(app.Services); // register json-rpc in swagger UI
    c.SwaggerEndpoint("/swagger/rest/swagger.json", "RESTful"); // register REST in swagger UI
    c.SwaggerEndpoint("/swagger/custom_v1/swagger.json", "Custom group");
});
app.UseJsonRpc();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapSwagger(); // swagger routing, alternative - UseSwagger()
app.MapOpenRpc(); // OpenRpc routing, alternative - UseOpenRpc()

await app.RunAsync();
