// Polyfill for C# 9 `init` accessor + `record` types when targeting .NET Framework 4.8 (Revit 2023, 2024).
// .NET 5+ defines this type in BCL — guard prevents conflict for Revit 2025+ (net8.0-windows).
#if !NET5_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
   internal static class IsExternalInit
   {
   }
}
#endif
