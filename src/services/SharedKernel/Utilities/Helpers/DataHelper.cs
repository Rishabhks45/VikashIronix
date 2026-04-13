using System.Reflection;
using System.Security.Cryptography;
using System.Linq;

namespace SharedKernel.Utilities.Helpers;

public class Helper
{
    public static string GetDisplayName<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
        if (memberInfo.Length > 0)
        {
            var displayAttr = memberInfo[0].GetCustomAttribute<DisplayAttribute>();
            if (displayAttr != null)
            {
                return displayAttr.Name ?? string.Empty;
            }
        }

        return enumValue.ToString();
    }

    public static TEnum GetEnumFromDisplayName<TEnum>(string displayName) where TEnum : struct, Enum
    {
        foreach (var field in typeof(TEnum).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
            {
                if (attribute.Name == displayName)
                    return (TEnum)field.GetValue(null)!;
            }
            else
            {
                if (field.Name == displayName)
                    return (TEnum)field.GetValue(null)!;
            }
        }
        throw new ArgumentException($"'{displayName}' is not a valid display name for enum {typeof(TEnum).Name}");
    }
}
public static class EnumExtensions
{
    /// <summary>
    /// Gets the Display Name attribute value for an enum value.
    /// Falls back to the enum name if no Display attribute is found.
    /// </summary>
    public static string GetDisplayName(this Enum enumValue)
    {
        var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

        if (memberInfo != null)
        {
            var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                return displayAttribute.Name ?? enumValue.ToString();
            }
        }

        return enumValue.ToString();
    }

    /// <summary>
    /// Gets all enum values with their display names as a dictionary.
    /// </summary>
    public static Dictionary<TEnum, string> GetAllWithDisplayNames<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .ToDictionary(e => e, e => e.GetDisplayName());
    }

    /// <summary>
    /// Gets all enum values as a list of KeyValuePairs (useful for dropdowns).
    /// </summary>
    public static List<KeyValuePair<TEnum, string>> GetDropdownItems<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => new KeyValuePair<TEnum, string>(e, e.GetDisplayName()))
            .ToList();
    }
}

public class TimezoneOption
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string WindowsId { get; set; } = string.Empty;
}

public static class TimezoneHelper
{
    public static List<TimezoneOption> GetAvailableTimezones()
    {
        return new List<TimezoneOption>
        {
            new TimezoneOption
            {
                Id = "America/New_York",
                DisplayName = "EST - Eastern Time",
                ShortName = "EST",
                WindowsId = "Eastern Standard Time"
            },
            new TimezoneOption
            {
                Id = "America/Chicago",
                DisplayName = "CST - Central Time",
                ShortName = "CST",
                WindowsId = "Central Standard Time"
            },
            new TimezoneOption
            {
                Id = "America/Denver",
                DisplayName = "MST - Mountain Time",
                ShortName = "MST",
                WindowsId = "Mountain Standard Time"
            },
            new TimezoneOption
            {
                Id = "America/Los_Angeles",
                DisplayName = "PST - Pacific Time",
                ShortName = "PST",
                WindowsId = "Pacific Standard Time"
            },
            new TimezoneOption
            {
                Id = "Asia/Kolkata",
                DisplayName = "IST - India Standard Time",
                ShortName = "IST",
                WindowsId = "India Standard Time"
            }
        };
    }

    public static string FormatWithTimezone(DateTime? utcDate, string timezoneId)
    {
        if (!utcDate.HasValue)
            return string.Empty;

        try
        {
            var tzOption = GetAvailableTimezones()
                .FirstOrDefault(t => t.Id == timezoneId);

            TimeZoneInfo tz;

            if (tzOption != null)
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(tzOption.WindowsId);
            }
            else
            {
                // fallback: maybe already Windows ID
                tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            }

            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDate.Value, tz);

            // Short abbreviation banane ke liye
            string abbreviation = string.Join("",
                tz.StandardName
                  .Split(' ')
                  .Select(word => char.ToUpper(word[0]))
            );

            return $"{localTime:MM/dd/yyyy hh:mm tt} {abbreviation}";
        }
        catch
        {
            return utcDate.Value.ToString("MM/dd/yyyy hh:mm tt");
        }
    }
}

