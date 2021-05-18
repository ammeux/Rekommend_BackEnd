// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityServer4;

namespace IdentityServerHost.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
             new TestUser
             {
                 SubjectId = "40acecde-ba0f-4936-9f70-a4ef44d65ed9",
                 Username = "Frank",
                 Password = "c6bockdo",

                 Claims = new List<Claim>
                 {
                     new Claim("given_name", "Frank"),
                     new Claim("family_name", "Underwood"),
                     new Claim("address", "Main road 1"),
                     new Claim("subscriptionlevel", "FreeUser")
                 }
             },
             new TestUser
             {
                 SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                 Username = "Claire",
                 Password = "password",

                 Claims = new List<Claim>
                 {
                     new Claim("given_name", "Claire"),
                     new Claim("family_name", "Underwood"),
                     new Claim("address", "Big Street 2"),
                     new Claim("subscriptionlevel", "PayingUser")
                 }
             }
         };
    }
}