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
            _ => throw new ArgumentException($"Invalid data type: {x}")
        };
    }

    public static object GetDefault(DataType x)
    {
       return x switch
       {
            DataType.Text => string.Empty,
            DataType.Integer => 0,
            _ => throw new ArgumentException($"Invalid data type: {x}")
       };
    }
}