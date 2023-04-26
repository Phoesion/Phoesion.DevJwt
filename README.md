# Phoesion.DevJwt
Library and dotnet-tool for developing and testing web api services with JWT authorization.
Create custom tokens that can be used localy, without an external authority.


# How to use in your service
1. Install the [![Phoesion.DevJwt](https://img.shields.io/nuget/v/Phoesion.DevJwt?color=0481ff&label=Phoesion.DevJwt&logo=nuget&style=flat-square)](https://www.nuget.org/packages/Phoesion.DevJwt) NuGet package
``` sh
dotnet add package Phoesion.DevJwt
```

2. Enable dev-jwt on your JWT authorization services using the `UseDevJwt()` extension
``` cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => o.UseDevJwt(builder.Environment));
```

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

# Generate a jwt
1. Install the dotnet tool
```sh
dotnet tool install --global phoesion.devjwt.cli
```

2. Generate token using
```sh
dotnet devjwt create myApi --email user@mail.com --sub 42
```
![console screenshot](media/console_token_generated.png?raw=true "Console output")
You can now use the token for your requests.


# General Information
The `UseDevJwt()` extension configures an `ISecurityTokenValidator` that validates the token. 
Using the `HostingEnvironment`, it checks that the handler is only added for `Development` and `Testing` environments.


# Samples
The repository contains the following samples projects in the `Samples` folder :
- **SampleWebApi** : an ASP.Net core web api application
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
- In the service authentication setup, provide the key to the `UseDevJwt()` function
```cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => o.UseDevJwt(builder.Environment, "thiskeyisverylargetobreak"));
```



