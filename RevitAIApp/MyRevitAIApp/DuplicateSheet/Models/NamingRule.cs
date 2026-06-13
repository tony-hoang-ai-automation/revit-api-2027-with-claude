namespace MyRevitAIApp.DuplicateSheet.Models
{
   /// <summary>
   ///     Immutable naming-rule configuration passed to <c>INamingRuleEngine</c>.
   ///     Number and Name pipelines are independent; increment applies to Number only.
   /// </summary>
   public sealed record NamingRule(
      string? NumberPrefix,
      string? NumberSuffix,
      string? NumberFind,
      string? NumberReplace,
      int NumberIncrementStart,
      int NumberIncrementPad,
      string? NamePrefix,
      string? NameSuffix,
      string? NameFind,
      string? NameReplace)
   {
      public static NamingRule Empty { get; } = new(null, null, null, null, 0, 0, null, null, null, null);
   }
}
