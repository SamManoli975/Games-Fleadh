using System;
using Unity.Netcode;

[Serializable]
public struct ItemStack : INetworkSerializable, IEquatable<ItemStack>
{
    public ItemType itemType;
    public int count;

    // Implement INetworkSerializable
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemType);
        serializer.SerializeValue(ref count);
    }

    // Implement IEquatable<ItemStack>
    public bool Equals(ItemStack other)
    {
        return itemType == other.itemType && count == other.count;
    }

    public ItemStack(ItemType itemType, int count)
    {
        this.itemType = itemType;
        this.count = count;
    }
}