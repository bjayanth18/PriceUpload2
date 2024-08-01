using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Okta.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Add Okta Authentication Config
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
    options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
    options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
})
.AddOktaWebApi(new OktaWebApiOptions()
{
    OktaDomain = builder.Configuration["Okta:OktaDomain"],
    AuthorizationServerId = builder.Configuration["Okta:AuthorizationServerId"],
    Audience = builder.Configuration["Okta:Audience"]
});

//Enable Okta Authorization on the App
builder.Services.AddAuthorization();
builder.Services.AddMvc(o =>
{
    var policy = new AuthorizationPolicyBuilder()
      .RequireAuthenticatedUser()
      .Build();
    o.Filters.Add(new AuthorizeFilter(policy));
});

//Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
       "AllowAll",
       builder => builder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader());
});

//Add endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Start the app
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.Run();
