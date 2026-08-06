using System.Net.Mail;
using System.Text;
using Naswood.Modules.Platform.Domain.Users;

namespace Naswood.Modules.Platform.Application.Users;

public static class UserPasswordPolicy
{
    public static bool IsValid(string password) =>
        !string.IsNullOrEmpty(password) &&
        password.Length >= 12 &&
        password.Any(char.IsUpper) &&
        password.Any(char.IsLower) &&
        password.Any(char.IsDigit) &&
        password.Any(ch => !char.IsLetterOrDigit(ch));
}

public static class UserEmailValidator
{
    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            _ = new MailAddress(email.Trim());
            return email.Contains('@', StringComparison.Ordinal);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

public static class UserCsvMapper
{
    public const string Header =
        "employeeNumber,username,firstName,lastName,email,password,company,plant,department,position,roles";

    public static string BuildExport(IEnumerable<Domain.Authentication.AuthUser> users)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Header);
        foreach (var user in users)
        {
            sb.Append(Escape(user.EmployeeNumber));
            sb.Append(',');
            sb.Append(Escape(user.Username));
            sb.Append(',');
            sb.Append(Escape(user.FirstName));
            sb.Append(',');
            sb.Append(Escape(user.LastName));
            sb.Append(',');
            sb.Append(Escape(user.Email));
            sb.Append(',');
            sb.Append(Escape(string.Empty)); // password never exported
            sb.Append(',');
            sb.Append(Escape(string.Join('|', user.CompanyIds)));
            sb.Append(',');
            sb.Append(Escape(string.Join('|', user.PlantIds)));
            sb.Append(',');
            sb.Append(Escape(user.DepartmentCode));
            sb.Append(',');
            sb.Append(Escape(user.PositionCode));
            sb.Append(',');
            sb.Append(Escape(string.Join('|', user.Roles)));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static IReadOnlyList<UserCsvRow> Parse(string csvContent)
    {
        var lines = csvContent
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        if (lines.Count == 0)
        {
            return [];
        }

        var start = 0;
        if (lines[0].Contains("employeeNumber", StringComparison.OrdinalIgnoreCase) ||
            lines[0].Contains("username", StringComparison.OrdinalIgnoreCase))
        {
            start = 1;
        }

        var rows = new List<UserCsvRow>();
        for (var i = start; i < lines.Count; i++)
        {
            var cols = SplitCsvLine(lines[i]);
            if (cols.Count < 8)
            {
                throw new FormatException($"Row {i + 1} is incomplete.");
            }

            rows.Add(new UserCsvRow(
                EmployeeNumber: cols[0],
                Username: cols[1],
                FirstName: cols[2],
                LastName: cols[3],
                Email: cols[4],
                Password: cols[5],
                Company: cols[6],
                Plant: cols[7],
                Department: cols.ElementAtOrDefault(8),
                Position: cols.ElementAtOrDefault(9),
                Roles: cols.ElementAtOrDefault(10) ?? "ReadOnly",
                LineNumber: i + 1));
        }

        return rows;
    }

    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;
        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }

                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                result.Add(current.ToString().Trim());
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        result.Add(current.ToString().Trim());
        return result;
    }

    private static string Escape(string? value)
    {
        var text = value ?? string.Empty;
        if (text.Contains(',') || text.Contains('"') || text.Contains('\n'))
        {
            return $"\"{text.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
        }

        return text;
    }
}

public sealed record UserCsvRow(
    string EmployeeNumber,
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Company,
    string Plant,
    string? Department,
    string? Position,
    string Roles,
    int LineNumber);

public static class OrganizationCatalogSeed
{
    public static IReadOnlyList<Domain.Organization.CompanyReference> CreateCompanies() =>
    [
        Domain.Organization.CompanyReference.Create("COMP-001", "Naswood"),
        Domain.Organization.CompanyReference.Create("NASWOOD", "Naswood")
    ];

    public static IReadOnlyList<Domain.Organization.PlantReference> CreatePlants() =>
    [
        Domain.Organization.PlantReference.Create("PLANT-001", "Primary Plant", "COMP-001"),
        Domain.Organization.PlantReference.Create("BUCAK", "Bucak Plant", "NASWOOD")
    ];

    public static IReadOnlyList<Domain.Organization.DepartmentReference> CreateDepartments() =>
    [
        Domain.Organization.DepartmentReference.Create("PURCHASING", "Purchasing"),
        Domain.Organization.DepartmentReference.Create("INVENTORY", "Inventory"),
        Domain.Organization.DepartmentReference.Create("PRODUCTION", "Production"),
        Domain.Organization.DepartmentReference.Create("QUALITY", "Quality"),
        Domain.Organization.DepartmentReference.Create("MAINTENANCE", "Maintenance"),
        Domain.Organization.DepartmentReference.Create("FINANCE", "Finance"),
        Domain.Organization.DepartmentReference.Create("SALES", "Sales"),
        Domain.Organization.DepartmentReference.Create("HUMANRESOURCES", "Human Resources"),
        Domain.Organization.DepartmentReference.Create("EXECUTIVE", "Executive")
    ];

    public static IReadOnlyList<Domain.Organization.PositionReference> CreatePositions() =>
    [
        Domain.Organization.PositionReference.Create("BUYER", "Buyer"),
        Domain.Organization.PositionReference.Create("WAREHOUSEOPERATOR", "Warehouse Operator"),
        Domain.Organization.PositionReference.Create("PRODUCTIONPLANNER", "Production Planner"),
        Domain.Organization.PositionReference.Create("QUALITYENGINEER", "Quality Engineer"),
        Domain.Organization.PositionReference.Create("ACCOUNTANT", "Accountant"),
        Domain.Organization.PositionReference.Create("CEO", "CEO")
    ];
}

public static class UserStatusParser
{
    public static bool TryParse(string? value, out UserAccountStatus? status)
    {
        status = null;
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        if (Enum.TryParse<UserAccountStatus>(value.Trim(), ignoreCase: true, out var parsed))
        {
            status = parsed;
            return true;
        }

        return false;
    }
}
