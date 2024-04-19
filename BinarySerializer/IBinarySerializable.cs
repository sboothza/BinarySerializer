namespace BinarySerializer;

public interface IBinarySerializable
{
    void Serialize(TypedBinaryStream stream);
    void Deserialize(TypedBinaryStream stream);
}