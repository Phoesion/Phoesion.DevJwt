# Phoesion.DevJwt
Library and dotnet-tool for developing and testing web api services with JWT authorization.
Create custom tokens that can be used localy, without an external authority.


# How to use in your service

1. Install the dotnet tool
```sh
dotnet tool install --global phoesion.devjwt.cli
```

2. Generate token using
```sh
dotnet devjwt create myApi --email user@mail.com
```
![console screenshot](media/console_token_generated.png?raw=true "Console output")

3. Configure in `appsetting.Development.json`
``` json
"Authentication": {
   "Schemes": {
      "Bearer": {
         "ValidAudience": "myApi",
         "ValidIssuer": "phoesion.devjwt",
         "SigningKeys": [
          {
             "Issuer": "phoesion.devjwt",
             "Value": "c29tZV9kZWZhdWx0X2tleV9mb3JfZGV2cw=="
          }
         ]
      }
   }
}
```

4. You can now use the token for your requests.
```
curl -i -H "Authorization: Bearer {token}" https://localhost:{port}/secret
```
![postman screenshot](media/postman_result.png?raw=true "Console output")


# Samples
The repository contains the following samples projects in the `Samples` folder :
- **SampleWebApi** : an ASP.Net core web api application _(net7.0 and above)_
- **SampleWebApi_Older** : an ASP.Net core web api application _(net6.0 and net5.0)_
- **SampleGlowMicroservice** : a [Phoesion Glow](https://glow.phoesion.com) microservice
- **TokenGeneratorSample** : a console application that demononstrates how to generate token programmatically


# Custom signing key
By default, the generator and validator use a predefined key for signing/veryfing the token.
This way it will pass validation and you don't need to care about where/how the token was generated _(doesn't use UserSecrets store)_, which is fine since it's for local development and testing.

You can however generate/validate tokens using a custom key like so :
- In the tool specify a key to be used for signing the token using the `--signkey` parameter :
```
dotnet devjwt create myApi --email user@mail.com --sub 42 --signkey thiskeyisverylargetobreak
```
- Encode the key in base64 format _(so you can add it in your `appsettings.Development.json`)_
```
dotnet devjwt encode-key thiskeyisverylargetobreak
```
- Add the key in your `appsettings.Development.json`
```json
"Authentication": {
   "Schemes": {
      "Bearer": {
         "ValidAudience": "myApi",
         "ValidIssuer": "phoesion.devjwt"
         "SigningKeys": [
          {
             "Issuer": "phoesion.devjwt",
             "Value": "dGhpc2tleWlzdmVyeWxhcmdldG9icmVhaw==" // <-- Set your new encoded key here
          }
         ]
      }
   }
}
```

# General tokens programmatically
You can also generate tokens programmatically using the `TokenGenerator`

1. Add the [![Phoesion.DevJwt](https://img.shields.io/nuget/v/Phoesion.DevJwt?color=0481ff&label=Phoesion.DevJwt&logo=nuget&style=flat-square)](https://www.nuget.org/packages/Phoesion.DevJwt) NuGet package to your project 
``` sh
dotnet add package Phoesion.DevJwt
```

2. Use to `TokenGenerator`
```cs 
string userId = new Guid().ToString();
string email = "john.doe@example.com";
string audience = "myApi";

var token = TokenGenerator.Create(audience, email, userId)
                          .AddScope("openid", "profile")
                          .AddRole("admin")
                          .AddClaim("username", "johndoe")
                          .ExpiresIn(TimeSpan.FromDays(365))
                          .Build();
```


# How to use in net6.0 and net5.0 projects
1. Add the [![Phoesion.DevJwt](https://img.shields.io/nuget/v/Phoesion.DevJwt?color=0481ff&label=Phoesion.DevJwt&logo=nuget&style=flat-square)](https://www.nuget.org/packages/Phoesion.DevJwt) NuGet package to your web API project 
``` sh
dotnet add package Phoesion.DevJwt
```

2. Enable dev-jwt on your JWT authorization services using the `UseDevJwt()` extension
``` cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => o.UseDevJwt(builder.Environment));
```
_Notes : i only enables for 'Development' and 'Testing' environments_

3. Configure in `appsetting.Development.json`
``` json
"Authentication": {
   "Schemes": {
      "Bearer": {
         "ValidAudience": "myApi",
         "ValidIssuer": "phoesion.devjwt"
      }
   }
}
```


