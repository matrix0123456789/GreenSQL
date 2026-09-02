namespace GreenSQL.Core;

public static class DataTypeHelper
{
    public static DataType ParseDataType(string x)
    {
        return x.ToUpper() switch
        {
            "TEXT" => DataType.Text,
            "INTEGER" => DataType.Integer,
            "INT"=> DataType.Integer,
            "FLOAT" => DataType.Float,
            "DATE"=> DataType.Date,
            "TIME"=> DataType.Time,
            "DATETIME"=> DataType.DateTime,
            _ => throw new ArgumentException($"Invalid data type: {x}")
        };
    }

    public static object GetDefault(DataType x)
    {
       return x switch
       {
            DataType.Text => string.Empty,
            DataType.Integer => 0,
            DataType.Float => 0.0,
            DataType.Date=> new DateOnly(1,1,1),
            DataType.Time=> new TimeOnly(0,0,0),
            DataType.DateTime=> new DateTime(1,1,1,0,0,0),
            _ => throw new ArgumentException($"Invalid data type: {x}")
       };
    }
}