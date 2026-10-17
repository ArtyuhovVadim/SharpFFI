namespace SharpFFI.Annotations;

[AttributeUsage(AttributeTargets.Struct)]
public class HandleAttribute : Attribute
{
    public DefaultHandleCtor Ctor { get; set; } = DefaultHandleCtor.None;
}
