# AspNetCore.MongoDB.JWTIdentity
AspNetCore MongoDB JWTIdentity is like as AspNetCore.MongoDB.Identity and can produce access token and validate each http request.

# Nuget Package
https://www.nuget.org/packages/AspNetCore.MongoDB.JWTIdentity

# How to use
You have to add the following lines to your Startup.cs in the ASP.NET Core Project.
```C#
services.AddIdentity<ApplicationUser, MongoIdentityRole>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
}).AddDefaultTokenProviders();

services
	.Configure<MongoDBOption>(Configuration.GetSection("MongoDBOption"))
	.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"))
	.AddMongoDatabase()
	.AddJWTIdentity<ApplicationUser, MongoIdentityRole>(Configuration.GetSection("JWTSettings"));
```

In appsettings.json you can configure the correct Path with a new section to your MongoDB instance.

```json
{
  "Logging": {
    "IncludeScopes": false,
    "Debug": {
      "LogLevel": {
        "Default": "Warning"
      }
    },
    "Console": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  },
  "MongoDBOption": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "AspNetCoreMongoIdentity",
    "User": {
      "CollectionName": "Users",
      "ManageIndicies": true
    },
    "Role": {
      "CollectionName": "Roles",
      "ManageIndicies": true
    }
  },
  "JWTSettings": {
    "SecretKey": "nahidhasansweengineer",
    "Issuer": "nahidhasan.swe",
    "Audience": "New Audience"
  }
}

```
You have to knowledge about the uses of my another library [AspNetCore.MongoDB.Identity](https://github.com/nahidhasanswe/AspNetCore.MongoDB) properly to use this libray. You will able to do AspNetCore Identity, MongoOperation and also JWTIdentity.

## How to use JWT in controller

Inject the ```MongoJWT``` with ApplicationUser which are inherited from MongoIdentityUser. Please follow the below instruction :
```C#
public class ApplicationUser : MongoIdentityUser
{

}
```

```C#
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        MongoJWT<ApplicationUser> _mongoJWT;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            MongoJWT<ApplicationUser> mongoJWT
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mongoJWT = mongoJWT;
        }

        [HttpPost("token")]
        public async Task<JsonResult> Login([FromBody] LoginModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {

                    return await _mongoJWT.GetAuthTokenAsync(model.UserName);
                }

                return new JsonResult("UserName or Password is incorrect") { StatusCode = 400 };
            }

            return new JsonResult(ModelState.Values.SelectMany(x => x.Errors)) { StatusCode = 400 };
        }

        [HttpPost("register")]
        public async Task<JsonResult> Register([FromBody] RegisterModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    return new JsonResult("Successfully Register");
                }

                return GetErrors(result);
            }

            return new JsonResult(ModelState.Values.SelectMany(x => x.Errors)) { StatusCode = 400 };
        }

        private JsonResult GetErrors(IdentityResult result)
        {
            Dictionary<string, string> errorResult = new Dictionary<string, string>();

            foreach (var error in result.Errors)
            {
                errorResult.Add("", error.Description);
            }

            return new JsonResult(errorResult);
        }
    }
```

Now you have placed ```Authorize``` attribute in your controller as below:
```C#
[HttpPost("save")]
[Authorize]
public async Task<JsonResult> SaveSample([FromBody] SampleModel model)
{
	try
	{
		if(ModelState.IsValid)
		{
			model.CreatedBy = User.Identity.Name;
			model.CreatedDate = DateTime.Now;

			await _operation.SaveAsync(model);
			return new JsonResult("Save Successful");
		}

		return new JsonResult("Model is invalid") { StatusCode = 401 };

	}
	catch (Exception ex)
	{
		return new JsonResult(ex) { StatusCode = 401 };
	}
}
```

You will need to pass token with headers with ```Authorization``` key as below
``` 
Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6Im5haGlkQGdtYWlsLmNvbSIsInJvbGVzIjpbXSwidW5pcXVlX25hbWUiOiJuYWhpZCIsImlzcyI6Im5haGlkaGFzYW4uc3dlIiwiYXVkIjoiTmV3IEF1ZGllbmNlIiwibmJmIjoxNTE4NzU5MTU4LjAsImlhdCI6MTUxODc1OTE1OC4wLCJleHAiOjE1MTg3NjYzNTguMCwiQ3VzdG9tVGV4dCI6IkN1c3RvbSBwYXlsb2FkIEhlcmUifQ.-5sT9mVxpZrExqur5AGRSOoSQHyGt_FsDKABnm7aAhU
```


## How to add Expired time to token 
```C#
 _mongoJWT.SetTokenExpired(DateTime.Now.AddHours(2));
 return await _mongoJWT.GetAuthTokenAsync(model.UserName);
```

## How to add custom payload to access token
```C#
 Dictionary<string, object> customPayload = new Dictionary<string, object>();
customPayload.Add("payloadKey1", "PayloadContent1");
customPayload.Add("payloadKey2", "PayloadContent2");


_mongoJWT.SetCustomPayload(customPayload);

return await _mongoJWT.GetAuthTokenAsync(model.UserName);
```

## Contact Me
If you fetch any problem or suggession to implementing. Please tell me anytime through mail `nahidh527@gmail.com` and create a issue. Thanks for using this.