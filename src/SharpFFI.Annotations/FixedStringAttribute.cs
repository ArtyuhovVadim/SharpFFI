using System.Diagnostics;

namespace SharpFFI.Annotations;

[Conditional("SHARPFFI_COMPILE_TIME")]
[AttributeUsage(AttributeTargets.Struct)]
public class FixedStringAttribute : Attribute;
