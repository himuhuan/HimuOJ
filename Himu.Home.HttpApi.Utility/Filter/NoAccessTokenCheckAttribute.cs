namespace Himu.HttpApi.Utility
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NoAccessTokenCheckAttribute : Attribute
    {
    }
}