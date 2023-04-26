using Microsoft.AspNetCore.Http;
using Phoesion.Glow.SDK;
using Phoesion.Glow.SDK.Authorization;
using Phoesion.Glow.SDK.Firefly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleGlowMicroservice.Modules
{
    [AuthorizeJWT]
    public class SampleModule1 : FireflyModule
    {
        //Notes: test using url http://localhost/SampleService1/SampleModule1/DoTheThing?input=somevalue
        [Action(Methods.GET)]
        public string DoTheThing(string input)
        {
            return $"Did the thing with {input}";
        }

        //Notes: test using url http://localhost/SampleService1/SampleModule1/ClaimsViewer
        [Action(Methods.GET)]
        public List<string> ClaimsViewer()
            => Context.User.Claims.Select(c => $"{c.Type} : {c.Value}").ToList();
    }
}
