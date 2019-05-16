namespace NekoUtils {
    public interface IDeserializer<T> {

        /// <summary>
        /// Converts an serializable object (i.e.null from JSON) into a unserializable one
        /// </summary>
        /// <returns>An object that has fields that cannot be serialized</returns>
        T Deserialize ();
    }
}