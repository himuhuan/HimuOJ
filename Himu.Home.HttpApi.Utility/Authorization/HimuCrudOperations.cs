using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Himu.HttpApi.Utility.Authorization
{
    public class HimuCrudOperations
    {
        public readonly static OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
        public readonly static OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
        public readonly static OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
        public readonly static OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    }
}
