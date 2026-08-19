using GreenSQL.Core.Store;

if (args.Length == 0)
{
    
}
else
{
    if (args[0] == "readDataFile")
    {
        var fileName = args[1];
        var streamWrapper = new StreamWrapper(new FileStream(fileName, FileMode.Open));
        var header = streamWrapper.ReadStringFixedLength(12);
        Console.WriteLine("Data header:" +header);
        var binarySerializer=new BinarySerializer(streamWrapper);
        while (streamWrapper.HasMoreData)
        {
            var e = binarySerializer.ReadSingle();
            Console.WriteLine(e);
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(e));
        }
    }
}