using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyRevitAIApp.McpServer.Models
{
    /// <summary>
    /// Parameters for the <c>duplicate_sheets</c> MCP command. Mirrors the
    /// DuplicateSheet feature's NamingRule + SheetContentOptions so the AI can
    /// drive batch sheet duplication.
    /// </summary>
    public sealed class DuplicateSheetsParameters
    {
        /// <summary>Sheet numbers (as shown in Revit) of the sheets to duplicate.</summary>
        [JsonProperty("sourceSheetNumbers")]
        public List<string> SourceSheetNumbers { get; set; } = new List<string>();

        // --- Naming rule ---
        [JsonProperty("findText")] public string? FindText { get; set; }
        [JsonProperty("replaceText")] public string? ReplaceText { get; set; }
        [JsonProperty("prefix")] public string? Prefix { get; set; }
        [JsonProperty("suffix")] public string? Suffix { get; set; }
        [JsonProperty("useSequence")] public bool UseSequence { get; set; }
        [JsonProperty("sequenceStart")] public int SequenceStart { get; set; } = 1;
        [JsonProperty("sequencePadding")] public int SequencePadding { get; set; }

        /// <summary>JustSheet | WithDetailing | AsDependent. Defaults to JustSheet.</summary>
        [JsonProperty("mode")] public string? Mode { get; set; }

        // --- Content options ---
        [JsonProperty("copyTitleBlock")] public bool CopyTitleBlock { get; set; } = true;
        [JsonProperty("includeLegends")] public bool IncludeLegends { get; set; } = true;
        [JsonProperty("includeSchedules")] public bool IncludeSchedules { get; set; } = true;
        [JsonProperty("copySheetAnnotations")] public bool CopySheetAnnotations { get; set; } = true;
    }
}
