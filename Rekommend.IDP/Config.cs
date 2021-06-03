// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Rekommend.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                //new IdentityResources.Profile(),
                //new IdentityResources.Address(),
                new IdentityResource(JwtClaimTypes.Email, "Your email", new List<string>(){ JwtClaimTypes.Email }),
                new IdentityResource(JwtClaimTypes.GivenName, "Your first name", new List<string>(){ JwtClaimTypes.GivenName }),
                new IdentityResource(JwtClaimTypes.FamilyName, "Your family name", new List<string>(){ JwtClaimTypes.FamilyName }),
                new IdentityResource(JwtClaimTypes.Address, "Your city", new List<string>(){ JwtClaimTypes.Address }),
                new IdentityResource("country", "Your country", new List<string>(){ "country" }),
                new IdentityResource("company", "Your company", new List<string>(){ "company" }),
                new IdentityResource("profile", "Your profile tech or non tech", new List<string>(){ "profile" }),
                new IdentityResource("stack", "Your stack", new List<string>(){ "stack" }),
                new IdentityResource("seniority", "Your seniority", new List<string>(){ "seniority" }),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            { new ApiScope("rekommendapi")};

        internal static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("rekommendapi", "API rekommend", new List<string>(){
                    JwtClaimTypes.Email, 
                    JwtClaimTypes.GivenName, 
                    JwtClaimTypes.FamilyName,
                    JwtClaimTypes.Address,
                    "country",
                    "company",
                    "profile",
                    "stack",
                    "seniority"
                })
                {
                    Scopes = {"rekommendapi"}
                }
            };
        }

        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
                new Client
                {
                    ClientName = "Rekommend Desktop",
                    ClientId = "rekommendDesktop",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent=false,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =  { 
                        "https://localhost:3000/signin-oidc",
                        "https://localhost:3000/redirect-silentrenew",
                    },

                    AccessTokenLifetime = 120,

                    PostLogoutRedirectUris =  { "https://localhost:3000" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        //IdentityServerConstants.StandardScopes.Profile,
                        //IdentityServerConstants.StandardScopes.Address,
                        "rekommendapi",
                        JwtClaimTypes.Email,
                        JwtClaimTypes.GivenName,
                        JwtClaimTypes.FamilyName,
                        JwtClaimTypes.Address,
                        "country",
                        "company",
                        "profile",
                        "seniority",
                        "stack"
                    },
                }

            };
    }
}