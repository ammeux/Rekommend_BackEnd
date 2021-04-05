// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            { new ApiScope("rekommendapi")};

        internal static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("rekommendapi", "API rekommend")
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
                        IdentityServerConstants.StandardScopes.Profile,
                        "rekommendapi"
                    },
                }

            };
    }
}