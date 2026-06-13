using MyRevitAIApp.DuplicateSheet.Models;

namespace MyRevitAIApp.DuplicateSheet.Services
{
   /// <summary>
   ///     Default implementation của <see cref="INamingRuleEngine"/>.
   ///     Pipeline 5 bước (xem plan §13.2): findReplace → prefix/suffix → increment (number only)
   ///     → findReplace + prefix/suffix cho name → collision resolve append " (n)".
   /// </summary>
   public sealed class NamingRuleEngine : INamingRuleEngine
   {
      public (string newNumber, string newName, bool hasCollision) Apply(
         string srcNumber,
         string srcName,
         int indexInBatch,
         NamingRule rule,
         IReadOnlyCollection<string> takenNumbers)
      {
         var number = TransformNumber(srcNumber, indexInBatch, rule);
         var name = TransformName(srcName, rule);
         var (resolved, collided) = ResolveCollision(number, takenNumbers);
         return (resolved, name, collided);
      }

      private static string TransformNumber(string srcNumber, int indexInBatch, NamingRule rule)
      {
         var num = !string.IsNullOrEmpty(rule.NumberFind)
            ? srcNumber.Replace(rule.NumberFind!, rule.NumberReplace ?? string.Empty)
            : srcNumber;

         num = (rule.NumberPrefix ?? string.Empty) + num + (rule.NumberSuffix ?? string.Empty);

         if (rule.NumberIncrementStart > 0)
         {
            var n = rule.NumberIncrementStart + indexInBatch;
            var pad = Math.Max(0, rule.NumberIncrementPad);
            num += n.ToString().PadLeft(pad, '0');
         }

         return num;
      }

      private static string TransformName(string srcName, NamingRule rule)
      {
         var name = !string.IsNullOrEmpty(rule.NameFind)
            ? srcName.Replace(rule.NameFind!, rule.NameReplace ?? string.Empty)
            : srcName;

         return (rule.NamePrefix ?? string.Empty) + name + (rule.NameSuffix ?? string.Empty);
      }

      private static (string resolved, bool hasCollision) ResolveCollision(string number, IReadOnlyCollection<string> taken)
      {
         if (!taken.Contains(number))
         {
            return (number, false);
         }

         var k = 2;
         string candidate;
         do
         {
            candidate = number + " (" + k + ")";
            k++;
         }
         while (taken.Contains(candidate));

         return (candidate, true);
      }
   }
}
