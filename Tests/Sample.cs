using BinarySerializer;
namespace Tests;

public class Sample : IBinarySerializable
{
    [BinarySerializer.Order(1)]
    public int IntProperty { get; set; }

    [BinarySerializer.Order(2)]
    public long LongProperty { get; set; }

    [BinarySerializer.Order(3)]
    public string StringProperty { get; set; }

    [BinarySerializer.Order(4)]
    public DateTime DateTimeProperty { get; set; }

    public void Serialize(TypedBinaryStream stream)
    {
        stream.WritePrimitive(IntProperty);
        stream.WritePrimitive(LongProperty);
        stream.WritePrimitive(StringProperty);
        stream.WritePrimitive(DateTimeProperty);
    }

    public void Deserialize(TypedBinaryStream stream)
    {
        IntProperty = stream.Read<int>();
        LongProperty = stream.Read<long>();
        StringProperty = stream.Read<string>();
        DateTimeProperty = stream.Read<DateTime>();
    }
}