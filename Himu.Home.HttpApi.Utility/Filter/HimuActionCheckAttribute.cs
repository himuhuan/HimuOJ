namespace Himu.HttpApi.Utility
{

    /// <summary>
    /// Enable argument check for the HTTP API Action.
    /// The legality check of the Action parameter will be uniformly 
    /// checked based on the enumeration value specified by <see cref="HimuActionCheckTargets"/>.
    /// The check will be performed in the <see cref="HimuActionCheckFilter"/>.
    /// 
    /// The corresponding Action must have the corresponding parameter 
    /// specified by <see cref="HimuActionCheckAttribute"/>, 
    /// otherwise a <see cref="HimuActionCheckFilterException"/> will be thrown if <c>HIMU_DEV_MODE</c> is defined.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HimuActionCheckAttribute : Attribute
    {
        public HimuActionCheckTargets Target { get; init; }

        public HimuActionCheckAttribute(HimuActionCheckTargets target)
        {
            Target = target;
        }
    }
}
