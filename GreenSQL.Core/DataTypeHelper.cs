namespace GreenSQL.Core;

public static class DataTypeHelper
{
    public static DataType ParseDataType(string x)
    {
        return x.ToUpper() switch
        {
            "TEXT" => DataType.Text,
            _ => throw new ArgumentException($"Invalid data type: {x}")
        };
    }

    public static object GetDefault(DataType x)
    {
       return x switch
       {
            DataType.Text => string.Empty,
            _ => throw new ArgumentException($"Invalid data type: {x}")
       };
    }
}