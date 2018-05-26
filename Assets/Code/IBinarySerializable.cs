using System.IO;

public interface IBinarySerializable
{
    void BinarySave(BinaryWriter inWriter);

    void BinaryLoad(BinaryReader inReader);
}