using MyRevitAIApp.DuplicateSheet.Models;
using MyRevitAIApp.DuplicateSheet.Services;
using Xunit;

namespace MyRevitAIApp.Tests.DuplicateSheet
{
   public class NamingRuleEngineTests
   {
      private readonly INamingRuleEngine _sut = new NamingRuleEngine();

      private static HashSet<string> Taken(params string[] items) => new(items);

      [Fact]
      public void Apply_EmptyRule_ReturnsOriginal()
      {
         var (num, name, collided) = _sut.Apply("A101", "Ground Floor", 0, NamingRule.Empty, Taken());
         Assert.Equal("A101", num);
         Assert.Equal("Ground Floor", name);
         Assert.False(collided);
      }

      [Fact]
      public void Apply_NumberPrefix_PrependsToNumber()
      {
         var rule = NamingRule.Empty with { NumberPrefix = "COPY_" };
         var (num, _, _) = _sut.Apply("A101", "X", 0, rule, Taken());
         Assert.Equal("COPY_A101", num);
      }

      [Fact]
      public void Apply_NumberSuffix_AppendsToNumber()
      {
         var rule = NamingRule.Empty with { NumberSuffix = "_v2" };
         var (num, _, _) = _sut.Apply("A101", "X", 0, rule, Taken());
         Assert.Equal("A101_v2", num);
      }

      [Fact]
      public void Apply_NumberFindReplace_ReplacesSubstring()
      {
         var rule = NamingRule.Empty with { NumberFind = "A1", NumberReplace = "B2" };
         var (num, _, _) = _sut.Apply("A101", "X", 0, rule, Taken());
         Assert.Equal("B201", num);
      }

      [Fact]
      public void Apply_NumberFindReplaceWithNullReplace_RemovesFind()
      {
         var rule = NamingRule.Empty with { NumberFind = "PRE-", NumberReplace = null };
         var (num, _, _) = _sut.Apply("PRE-A101", "X", 0, rule, Taken());
         Assert.Equal("A101", num);
      }

      [Fact]
      public void Apply_NumberIncrement_AppendsIndexedNumber()
      {
         var rule = NamingRule.Empty with { NumberIncrementStart = 10, NumberIncrementPad = 0 };
         var (num0, _, _) = _sut.Apply("A", "X", 0, rule, Taken());
         var (num1, _, _) = _sut.Apply("A", "X", 1, rule, Taken());
         Assert.Equal("A10", num0);
         Assert.Equal("A11", num1);
      }

      [Fact]
      public void Apply_NumberIncrement_PadsToWidth()
      {
         var rule = NamingRule.Empty with { NumberIncrementStart = 1, NumberIncrementPad = 3 };
         var (num, _, _) = _sut.Apply("A", "X", 0, rule, Taken());
         Assert.Equal("A001", num);
      }

      [Fact]
      public void Apply_AllNumberRules_AppliesInOrder()
      {
         // Pipeline: FindReplace → Prefix/Suffix → Increment
         var rule = NamingRule.Empty with
         {
            NumberFind = "A1",
            NumberReplace = "B2",
            NumberPrefix = "COPY_",
            NumberSuffix = "_x",
            NumberIncrementStart = 5,
            NumberIncrementPad = 2
         };
         var (num, _, _) = _sut.Apply("A101", "X", 0, rule, Taken());
         // Find→Replace: A101 → B201
         // Prefix/Suffix: COPY_B201_x
         // Increment (5 + 0 = 5, pad 2): COPY_B201_x05
         Assert.Equal("COPY_B201_x05", num);
      }

      [Fact]
      public void Apply_NameRules_TransformIndependently()
      {
         var rule = NamingRule.Empty with
         {
            NamePrefix = "[NEW] ",
            NameSuffix = " - Copy",
            NameFind = "Floor",
            NameReplace = "Storey"
         };
         var (_, name, _) = _sut.Apply("A", "Ground Floor", 0, rule, Taken());
         // Find→Replace: "Ground Storey"
         // Prefix/Suffix: "[NEW] Ground Storey - Copy"
         Assert.Equal("[NEW] Ground Storey - Copy", name);
      }

      [Fact]
      public void Apply_NameFindNoMatch_NameUnchanged()
      {
         var rule = NamingRule.Empty with { NameFind = "XYZ", NameReplace = "ABC" };
         var (_, name, _) = _sut.Apply("A", "Ground Floor", 0, rule, Taken());
         Assert.Equal("Ground Floor", name);
      }

      [Fact]
      public void Apply_NumberCollision_AutoIncrementsParen()
      {
         var rule = NamingRule.Empty with { NumberPrefix = "COPY_" };
         var (num, _, collided) = _sut.Apply("A101", "X", 0, rule, Taken("COPY_A101"));
         Assert.Equal("COPY_A101 (2)", num);
         Assert.True(collided);
      }

      [Fact]
      public void Apply_MultipleCollisions_FindsFreeNumber()
      {
         var rule = NamingRule.Empty with { NumberPrefix = "COPY_" };
         var (num, _, collided) = _sut.Apply(
            "A101", "X", 0, rule,
            Taken("COPY_A101", "COPY_A101 (2)", "COPY_A101 (3)"));
         Assert.Equal("COPY_A101 (4)", num);
         Assert.True(collided);
      }

      [Fact]
      public void Apply_NumberOnlyRule_NameUnchanged()
      {
         var rule = NamingRule.Empty with { NumberPrefix = "X_" };
         var (num, name, _) = _sut.Apply("A101", "Ground Floor", 0, rule, Taken());
         Assert.Equal("X_A101", num);
         Assert.Equal("Ground Floor", name);
      }

      [Fact]
      public void Apply_NoCollisionFlag_WhenNumberFree()
      {
         var rule = NamingRule.Empty with { NumberPrefix = "FRESH_" };
         var (_, _, collided) = _sut.Apply("A101", "X", 0, rule, Taken("OTHER_A101", "A102"));
         Assert.False(collided);
      }
   }
}
