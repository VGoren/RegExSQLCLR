using Microsoft.SqlServer.Server;
using System.Collections;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.RegularExpressions;

public class RegEx
{
    // Опции: Регистронезависимость + Оптимизация под разные языки
    private const RegexOptions DefOpts = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

    [SqlFunction(IsDeterministic=true, IsPrecise=true)] public static SqlBoolean  RegexIsMatch(SqlString input, SqlString pattern)                       { if (input.IsNull || pattern.IsNull)                       return SqlBoolean.Null; bool   res = Regex.IsMatch(input.Value, pattern.Value,                    DefOpts); return res; }
    [SqlFunction(IsDeterministic=true, IsPrecise=true)] public static SqlString   RegexMatch  (SqlString input, SqlString pattern)                       { if (input.IsNull || pattern.IsNull)                       return SqlString.Null;  Match  res = Regex.Match  (input.Value, pattern.Value,                    DefOpts); return res.Success ? res.Value : SqlString.Null; }
    [SqlFunction(IsDeterministic=true, IsPrecise=true)] public static SqlString   RegexReplace(SqlString input, SqlString pattern, SqlString replacement){ if (input.IsNull || pattern.IsNull || replacement.IsNull) return SqlString.Null;  string res = Regex.Replace(input.Value, pattern.Value, replacement.Value, DefOpts); return res; }
    [SqlFunction(IsDeterministic=true, IsPrecise=true)] public static SqlString   RegexGroup  (SqlString input, SqlString pattern, SqlInt32  groupIndex) { if (input.IsNull || pattern.IsNull || groupIndex.IsNull)  return SqlString.Null;  Match  res = Regex.Match  (input.Value, pattern.Value,                    DefOpts); return (res.Success && res.Groups.Count > groupIndex) ? res.Groups[groupIndex.Value].Value : SqlString.Null; }
    [SqlFunction(IsDeterministic=true, IsPrecise=true,
                 FillRowMethodName = "Matches_FillRow",
                 TableDefinition   = "MatchIndex int"
                                 + ", Value      nvarchar(max)"
                                 + ", Length     int"
    )]                                                  public static IEnumerable RegexMatches(SqlString input, SqlString pattern)                       { if (input.IsNull || pattern.IsNull)                       return null;            var    res = Regex.Matches(input.Value, pattern.Value,                    DefOpts)
                                                                                                                                                                                                                                                               .Cast<Match>()                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                               .Select(m => new MatchResult(m));                                 return res; }
    public static void Matches_FillRow(
        object        row, // Объект из списка
        out SqlInt32  idx, // Позиция (out)
        out SqlString val, // Текст совпадения (out)
        out SqlInt32  len  // Длина (out)
    )
    {
        MatchResult m = (MatchResult)row;             // Создание строки
        idx = m.Index; val = m.Value; len = m.Length; // Инициализация строки
    }
    private class MatchResult // Контейнер для строки
    {
        public readonly int    Index;
        public readonly string Value;
        public readonly int    Length;
        public MatchResult(Match m)
        {
            Index  = m.Index;
            Value  = m.Value;
            Length = m.Length;
        }
    }
}