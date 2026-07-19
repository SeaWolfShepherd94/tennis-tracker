using System.Data;
using Dapper;

namespace TennisTracker.Data;

public sealed class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        parameter.Value = value.ToString();
    }

    public override Guid Parse(object value) => Guid.Parse((string)value);
}

public sealed class NullableGuidTypeHandler : SqlMapper.TypeHandler<Guid?>
{
    public override void SetValue(IDbDataParameter parameter, Guid? value)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        parameter.Value = value?.ToString() ?? (object)DBNull.Value;
    }

    public override Guid? Parse(object value) =>
        value is null or DBNull ? null : Guid.Parse((string)value);
}

public static class SqliteTypeHandlers
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered) return;
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.AddTypeHandler(new NullableGuidTypeHandler());
        _registered = true;
    }
}
