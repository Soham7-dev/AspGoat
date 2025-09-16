using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspGoat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AspGoat.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.Xml;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;
using System.Text.Json;
using System.Threading.Tasks;
using RazorLight;

namespace AspGoat.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public HomeController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ReflectedXSS(string query)
    {
        ViewData["Query"] = query;
        return View();
    }

    [HttpGet]
    public IActionResult StoredXSS()
    {
        var comments = _context.Comments.Select(c => c.Content).ToList();
        return View(comments);
    }

    [HttpPost]
    public IActionResult StoredXSS(string comment)
    {
        
        var newComment = new Comment
        {
            Content = comment
        };

        _context.Comments.Add(newComment);
        _context.SaveChanges();

        return RedirectToAction("StoredXSS");
    }

    [HttpGet]
    public IActionResult SqlInjection()
    {
        string ip = Request.Headers["X-Forwarded-For"].ToString();

        if (String.IsNullOrEmpty(ip)) ip = "127.0.0.1";

        var _connString = _config.GetConnectionString("DefaultConnection");

        using var conn = new SqliteConnection(_connString);
        conn.Open();

        // Vulnerable to SQL Injection
        string query = "SELECT * FROM Users " + "WHERE LastLoginIP = '" + ip + "'";

        using var cmd = new SqliteCommand(query, conn);
        using var reader = cmd.ExecuteReader();

        if (reader.Read()) // take first row only
        {
            ViewData["Id"] = reader["Id"].ToString();
            ViewData["UserName"] = reader["UserName"].ToString();
            ViewData["PasswordHash"] = reader["PasswordHash"].ToString();
            ViewData["Email"] = reader["Email"].ToString();
            ViewData["LastLoginIP"] = reader["LastLoginIP"].ToString();
            ViewData["Role"] = reader["Role"].ToString();
        }

        return View();
    }

    [HttpGet]
    public IActionResult BrokenAuthentication()
    {
        return View();
    }

    [HttpPost]
    public IActionResult BrokenAuthentication(string username, string password)
    {
        if (username != "admin")
        {
            //Username enumeration vulnerability
            ViewData["Error"] = "User does not exist.";
            return View();
        }

        if (password != "admin")
        {
            //Indicates username is valid
            ViewData["Error"] = "Incorrect password.";
            return View();
        }

        ViewData["LoginMessage"] = $"Welcome, {username}!";
        return View();
    }

    [HttpGet]
    public IActionResult InformationDisclosure()
    {
        return View();
    }

    [HttpGet]
    public IActionResult XXE()
    {
        return View();
    }

    [HttpPost]
    public IActionResult XXE(string xmlInput)
    {
        string result = "";
        try
        {
            var xmlDoc = new XmlDocument
            {
                XmlResolver = new XmlUrlResolver()  //Enables external entity fetching
            };
            //Vulnerable: External entity resolution enabled by default
            xmlDoc.LoadXml(xmlInput);
            result = xmlDoc.InnerText;
        }
        catch (Exception ex)
        {
            result = $"Error: {ex.Message}";
        }

        ViewData["ParsedXml"] = result;
        return View();
    }

    [HttpGet]
    public IActionResult OpenRedirect(string returnUrl)
    {
        if (returnUrl != null)
        {
            return Redirect(returnUrl);
        }

        return View();
    }

    [HttpGet]
    public IActionResult InsecureDirectObjectReference()
    {
        return View(new Dictionary<string, object>());
    }

    [HttpPost]
    public IActionResult InsecureDirectObjectReference(int UserId)
    {
        // Simulating dynamic user data with hardcoding
        var userData = new Dictionary<int, Dictionary<string, object>>
        {
            { 1, new Dictionary<string, object>
                {
                    { "user_id", 1 },
                    { "username", "admin" },
                    { "email", "administrator@aspgoat.net" },
                    { "api_key", "a1b2c3d4e5f678g9h0i1j2k3l4m5n6o7p8q9r0s1t2u3v4w5x6y7z8" },
                    { "account_status", "active" }
                }
            },
            { 2, new Dictionary<string, object>
                {
                    { "user_id", 2 },
                    { "username", "john356" },
                    { "email", "john.smith@user.net" },
                    { "api_key", "a1b2c3d4e5f678g9h0i1j2k3l4m5n6o7p8q9r0s1t2u3v4w5x6y7z8" },
                    { "account_status", "active" }
                }
            }
        };

        if (userData.ContainsKey(UserId))
        {
            return View(userData[UserId]);
        }

        return NotFound("User not found");
    }

    [HttpGet]
    public IActionResult DomXSS()
    {
        return View();
    }

    [HttpGet]
    public IActionResult PrototypePollution()
    {
        return View();
    }

    [HttpGet]
    public IActionResult LFI()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Download(string file)
    {
        // Vulnerable file concatination 
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file);

        if (!System.IO.File.Exists(path))
        {
            return NotFound("File not found");
        }

        var contentType = "application/octet-stream";
        var fileBytes = System.IO.File.ReadAllBytes(path);

        return File(fileBytes, contentType, file);
    }

    [HttpGet]
    public IActionResult FileUpload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> FileUpload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Content("No file selected.");

        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploads);

        // Vulnerable filename concatenation
        var filePath = Path.Combine(uploads, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Content($"File {file.FileName} uploaded successfully to /uploads.");
    }

    [HttpGet]
    public IActionResult CommandInjection(string domain)
    {
        if (!String.IsNullOrEmpty(domain))
        {
            // Choose shell on the basis of OS
            string shell, args;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shell = "cmd.exe";
                // VULNERABLE: direct concatenation of user input into shell command
                args = $"/c nslookup {domain}";
            }
            else
            {
                shell = "/bin/bash";
                // VULNERABLE: direct concatenation of user input into shell command    
                args = $"-c \"nslookup {domain}\"";
            }

            var process = new Process();
            process.StartInfo.FileName = shell;
            process.StartInfo.Arguments = args;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            ViewData["Output"] = output;
        }

        return View();
    }

    [HttpGet]
    public IActionResult InsecureDeserialization()
    {
        return View();
    }

    [HttpPost]
    public IActionResult InsecureDeserialization([FromBody] JsonElement body)
    {
        var json = body.GetRawText();

        // Vulnerable code as TypeNameHandling.All let's attacker inject arbitrary objects of classes
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        var obj = JsonConvert.DeserializeObject<SafeMessage>(json, settings);

        var message = obj?.Message;

        return Json(new { message });
    }

    [HttpGet]
    public async Task<IActionResult> CSRF()
    {
        var email = await _context.EmailIds.Select(e => e.Email).FirstOrDefaultAsync();
        ViewData["Email"] = email;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CSRF(int id, string email) 
    {
        var emailRow = await _context.EmailIds.FindAsync(id);
        if (emailRow == null)
            return NotFound($"EmailId {id} not found.");

        emailRow.Email = email;
        await _context.SaveChangesAsync();

        return RedirectToAction();
    }

    [HttpGet]
    public IActionResult SSRF()
    {
        return View();
    }

    [HttpPost]
public async Task<IActionResult> SSRF(string targetUrl)
{
    // Define a whitelist of allowed domains
    var allowedHosts = new[] { "example.com", "api.example.com" };

    try
    {
        // Parse the target URL
        var uri = new Uri(targetUrl);

        // Check if the host is in the whitelist
        if (!allowedHosts.Contains(uri.Host))
        {
            return BadRequest("The specified URL is not allowed.");
        }

        using var http = new HttpClient();
        var response = await http.GetStringAsync(targetUrl);
        ViewData["Response"] = response;

        return View();
    }
    catch (Exception ex)
    {
        // Handle invalid URLs or other exceptions
        return BadRequest($"Error: {ex.Message}");
    }
}

    [HttpGet]
    // Vulnerable as the X-Forwarded-Host is not taken into account for the Cache Key
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByHeader = "")]
    public IActionResult CachePoisoning()
    {
        var host = Request.Headers["X-Forwarded-Host"];

        ViewData["X-Forwarded-Host"] = host;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SSTI([FromServices] IRazorLightEngine razor)
    {
        var userName = _context.Users.Where(u => u.Id == 2).Select(u => u.UserName).FirstOrDefault();

        var key = Guid.NewGuid().ToString("N");

        var html = "";

        try
        {
            // Vulnerable as it compiles & executes user-supplied Razor (Razorlight Template Engine)
            html = await razor.CompileRenderStringAsync(key, userName ?? "Null", new { });
        }
        catch (Exception e)
        {
            html = e.Message;
        }

        ViewData["Html"] = html;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SSTI(int id, string userName)
    {
        var userRow = await _context.Users.FindAsync(id);
        if (userRow == null)
            return NotFound($"User {id} not found.");

        userRow.UserName = userName;
        await _context.SaveChangesAsync();

        return RedirectToAction();
    }

    [AllowAnonymous]
    [HttpGet("/internal/config")]
    public IActionResult InternalConfig()
    {
        return Ok(new
        {
            service = "AspGoat.Internal.Config",
            dbConnection = "Server=aspgoat-db;User=app;Password=SuperSecret!",
            jwtSigningKey = "FAKE-KEY-123456789",
            adminEmail = "admin@aspgoat.local"
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
