﻿using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Himu.Home.HttpApi.Services.Authorization
{
    public class HimuCrudOperations
    {
        public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
        public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
        public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
        public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    }
}