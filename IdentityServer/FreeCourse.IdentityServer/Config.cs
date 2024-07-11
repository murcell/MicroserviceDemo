// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("resource_catalog"){Scopes={"catalog_fullpermission"}},
                new ApiResource("resource_photo_stock"){Scopes={"photo_stock_fullpermission"}},
                new ApiResource("resource_basket"){Scopes={"basket_fullpermission"}},
                new ApiResource("resource_discount"){Scopes={"discount_fullpermission"}},
                new ApiResource("resource_order"){Scopes={"order_fullpermission"}},
                new ApiResource("resource_payment"){Scopes={"payment_fullpermission"}},
                new ApiResource("resource_gateway"){Scopes={"gateway_fullpermission"}},
                // örnek olsun diye bıraktım
                // new ApiResource("resource_discount"){Scopes={"discount_fullpermission", "discount_read", "discount_write"}},
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.Email(),
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource(){ Name="roller", DisplayName="Roles", Description="Kullanıcı rolleri", UserClaims=new []{"role" } }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog_fullpermission","Full access for Catalog API"),
                new ApiScope("photo_stock_fullpermission","Full access for Photo Stock API"),
                new ApiScope("basket_fullpermission","Full access for Basket API"),
                new ApiScope("discount_fullpermission","Full access for Discount API"),
                new ApiScope("order_fullpermission","Full access for Order API"),
                new ApiScope("payment_fullpermission","Full access for Payment API"),
                new ApiScope("gateway_fullpermission","Full access for Gateway API"),
                //new ApiScope("discount_read","Only read to access for Discount API"), örnek olsun diye
                //new ApiScope("discount_write","Only write to access for Discount API"), örnek olsun diye
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
               new Client
               {
                   ClientName="Asp.Net Core MCV",
                   ClientId="WebMvcClient",
                   ClientSecrets={new Secret("secret".Sha256())},
                   AllowedGrantTypes = GrantTypes.ClientCredentials,
                   AllowedScopes={ "catalog_fullpermission", "photo_stock_fullpermission", "gateway_fullpermission", IdentityServerConstants.LocalApi.ScopeName }
               },
               new Client
               {
                   ClientName="Asp.Net Core MCV",
                   ClientId="WebMvcClientForUser",
                   AllowOfflineAccess = true,
                   ClientSecrets={new Secret("secret".Sha256())},
                   AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                   //// burayı bilerek yorum bıraktım. tokenexchange devreye almadan bu şekildeydi
                   //AllowedScopes={"basket_fullpermission", "discount_fullpermission", "order_fullpermission", "payment_fullpermission", "gateway_fullpermission", IdentityServerConstants.StandardScopes.Email, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.OfflineAccess,"roles",IdentityServerConstants.LocalApi.ScopeName },
                   AllowedScopes={"basket_fullpermission", "order_fullpermission", "gateway_fullpermission", IdentityServerConstants.StandardScopes.Email, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.OfflineAccess,"roles",IdentityServerConstants.LocalApi.ScopeName },
                   AccessTokenLifetime=1*60*60,
                   RefreshTokenExpiration=TokenExpiration.Absolute,
                   AbsoluteRefreshTokenLifetime=(int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
                   RefreshTokenUsage=TokenUsage.ReUse
               },
               new Client
               {
                   ClientName="Token Exchange Client",
                   ClientId="TokenExchangeClient",
                   ClientSecrets={new Secret("secret".Sha256())},
                   AllowedGrantTypes = new []{ "urn:ietf:params:oauth:grant-type:token-exchange" },
                   AllowedScopes={ "discount_fullpermission", "payment_fullpermission", IdentityServerConstants.StandardScopes.OpenId }
               },
            };
    }
}