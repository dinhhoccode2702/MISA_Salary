# Copilot Instructions

## Project Guidelines
- Dapper ORM mapping issue with snake_case vs PascalCase: MISA project uses snake_case column names in MySQL (e.g., organization_id) but PascalCase properties in C# (e.g., OrganizationId). Default Dapper mapping (DefaultTypeMap.MatchNamesWithUnderscores) is required for seamless mapping. However, automatic SQL generation in BaseRepository needs explicit logic to convert C# property names to snake_case when building INSERT/UPDATE queries.