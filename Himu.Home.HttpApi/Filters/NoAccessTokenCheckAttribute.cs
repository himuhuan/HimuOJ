namespace Himu.Home.HttpApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NoAccessTokenCheckAttribute : Attribute
    {
    }
}