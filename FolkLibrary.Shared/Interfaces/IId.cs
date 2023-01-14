using MongoDB.Bson.Serialization;

namespace FolkLibrary.Interfaces;

public interface IId<T> : IEquatable<T>, IComparable<T>
{
    //private static Func<T> NewFunc { get; } = typeof(T).GetMethod("New", BindingFlags.Static | BindingFlags.Public)!.CreateDelegate<Func<T>>();
    //private static ConstructorInfo CtorFunc { get; } = typeof(T).GetConstructor(new Type[] { typeof(Guid) })!;

    Guid Value { get; }

    static abstract T New();
    //    {
    //#if NET7_0_OR_GREATER
    //        throw new InvalidOperationException("Replace with static abstract interface member");
    //#endif
    //        return NewFunc.Invoke();
    //    }

    static abstract T New(Guid guid);
    //    {
    //#if NET7_0_OR_GREATER
    //        throw new InvalidOperationException("Replace with static abstract interface member");
    //#endif
    //        return (T)CtorFunc.Invoke(new object[] { guid });
    //    }
}

public sealed class IIdBsonSerializer<T> : IBsonSerializer<T> where T : IId<T>
{
    public Type ValueType { get => typeof(T); }

    public T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => T.New(new Guid(context.Reader.ReadString()));
    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => Deserialize(context, args);
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value) => context.Writer.WriteString(value.Value.ToString());
    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value) => Serialize(context, args, (T)value);
}
