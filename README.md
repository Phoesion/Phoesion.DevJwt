# Phoesion.DevJwt
Easy JWT authentication for developing and testing.

# How to use in your service
1. Install the [Phoesion.DevJwt](https://www.nuget.org/packages/Phoesion.DevJwt) NuGet package
``` sh
dotnet add package Phoesion.DevJwt
```

2. Enable dev-jwt on your JWT authorization services
``` cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => o.AddDevJwt(builder.Environment));
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
You can now use the token for your requests.


# General Information
The `AddDevJwt()` extension configures an `ISecurityTokenValidator` that validates the token. 
Using the `HostingEnvironment`, it checks that the handler is only added for `Development` and `Testing` environments.



