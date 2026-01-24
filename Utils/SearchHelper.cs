namespace HalaqatBackend.Utils
{
    public static class SearchHelper
    {
        /// <summary>
        /// Generates a SQL WHERE clause for searching across multiple fields
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        /// <param name="searchFields">Array of field names to search in (e.g., "first_name", "last_name", "email")</param>
        /// <param name="parameters">Dictionary to store the parameter values for the query</param>
        /// <returns>SQL WHERE clause string</returns>
        public static string BuildSearchClause(string? searchText, string[] searchFields, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(searchText) || searchFields == null || searchFields.Length == 0)
            {
                return string.Empty;
            }

            var searchConditions = searchFields
                .Select(field => $"LOWER({field}) LIKE LOWER(@SearchText)")
                .ToList();

            parameters["SearchText"] = $"%{searchText}%";

            return $"({string.Join(" OR ", searchConditions)})";
        }

        /// <summary>
        /// Normalizes search text by trimming and removing extra spaces
        /// </summary>
        public static string? NormalizeSearchText(string? searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return null;
            }

            return searchText.Trim();
        }
    }
}
