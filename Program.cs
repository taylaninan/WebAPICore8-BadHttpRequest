using Isekibi.Common;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();

WebApplication app = builder.Build();

ForwardedHeadersOptions HttpHeaderOptions = new ForwardedHeadersOptions();
HttpHeaderOptions.ForwardedHeaders = ForwardedHeaders.All;
app.UseForwardedHeaders(HttpHeaderOptions);
app.UseAntiforgery();

app.Use((Context, Next) =>
{
    var SyncIO = Context.Features.Get<IHttpBodyControlFeature>();
    SyncIO.AllowSynchronousIO = true;

    Context.Response.Headers.Remove("X-Powered-By");
    Context.Response.Headers.Remove("Server");
    Context.Response.Headers.Remove("X-AspNet-Version");
    Context.Response.Headers.Remove("X-AspNetMvc-Version");
    Context.Response.Headers.Remove("X-SourceFiles");

    Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost");
    Context.Response.Headers.Append("Access-Control-Expose-Headers", "set-cookie, cookie");
    Context.Response.Headers.Append("Access-Control-Allow-Headers", "origin, content-type, accept, set-cookie, cookie");
    Context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
    Context.Response.Headers.Append("Access-Control-Allow-Methods", "POST");
    Context.Response.Headers.Append("Access-Control-Max-Age", "3600");
    Context.Response.Headers.Append("Content-Type", "application/json; charset=UTF-8");
    Context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'; connect-src 'self'; img-src 'self'; style-src 'self'; base-uri 'self'; form-action 'self'");
    Context.Response.Headers.Append("Set-Cookie", "HttpOnly; SameSite=Strict");
    Context.Response.Headers.Append("X-Frame-Options", "DENY");
    Context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
    Context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    Context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
    Context.Response.Headers.Append("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
    Context.Response.Headers.Append("Referrer-Policy", "no-referrer");

    UserIP = NetX.GetClientIPAddress(Context);
    RequestedPage = Context.Request.Path.ToString().ToLower().TrimEnd('/');

    Context.Request.EnableBuffering();
    Context.Request.Body.Position = 0;
    StreamReader JSONRequestReader = new StreamReader(Context.Request.Body, System.Text.Encoding.UTF8, false, -1, true);
    JSONRequest   = JSONRequestReader.ReadToEnd();
    JSONRequestReader.Close();
    Context.Request.Body.Position = 0;

    return Next(Context);
});

var APIv1 = app.MapGroup("/api/v1");
APIv1.MapPost("/string", APIString);
APIv1.MapPost("/user_user_login_with_email", APIUserUserLoginWithEmail);

app.Run();

static string APIString()
{
    return "Hello and Welcome Aboard!";
}

static string APIUserUserLoginWithEmail()
{
    ApiV1UserUserLoginWithEmail UserUserLoginWithEmail = JsonConvert.DeserializeObject<ApiV1UserUserLoginWithEmail>(JSONRequest);

    if (UserUserLoginWithEmail != null)
    {
        UserUserLoginWithEmail.IP = UserIP;

        if (UserUserLoginWithEmail.Email.IsValidEmail() && UserUserLoginWithEmail.Password.IsValidPassword() && UserUserLoginWithEmail.IP.IsValidIP())
        {
            int ErrorCode = 0;

            if (ErrorCode == 0 || ErrorCode == -4)
            {
                ApiV1Token Token = new ApiV1Token();

                Token.DomainName = "http://localhost";
                Token.TimeIssued = DateTime.Now.Ticks.ToString();
                Token.TimeExpire = DateTime.Now.AddSeconds(3600).Ticks.ToString();
                Token.UserGUID = "CB6CF2A1-A383-4ABB-8872-8FCDFDD77A34";

                JSONResponse = APIX.Base64Encode(APIX.GZipCompress(JsonConvert.SerializeObject(Token)));
                JSONResponse += "." + APIX.Base64Encode(APIX.GZipCompress(APIX.SHA_512(JSONResponse, APISecret)));

                JSONResponse = @"{ ""token"": """ + JSONResponse + @""", ""error_code"": " + ErrorCode.ToString() + " }";
            }
            else
            {
                JSONResponse = @"{ ""token"": null, ""error_code"": " + ErrorCode.ToString() + " }";
            }
        }
    }

    return (JSONResponse == null) ? "" : JSONResponse;
}

public static partial class Program
{
    const string APISecret = "b9ca9b2cb6f277e191ce327ce45123a8e08ea6a38dfe599b1a04fa6973719cbf22c7a7b073d3a1b1bd0c877dd04edabd57abc391bbed7176ca64d4456d2de08e";

    static string UserIP = null;
    static string RequestedPage = null;
    static string JSONRequest = null;
    static string JSONResponse = null;
}

public class ApiV1UserUserLoginWithEmail
{
    public StringX Email { get; set; }
    public StringX Password { get; set; }
    public StringX IP { get; set; }
}

public class ApiV1Token
{
    public StringX DomainName { get; set; }
    public StringX TimeIssued { get; set; }
    public StringX TimeExpire { get; set; }
    public StringX UserGUID   { get; set; }
}
