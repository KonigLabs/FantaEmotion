using System;
using System.IO;
using System.Monads;
using System.Xml.Serialization;

namespace KonigLabs.FantaEmotion.Common.Extensions
{
    public static class SerializingExtensions
    {
        public static TObject Deserialize<TObject>(this byte[] source)
        {
            return source.Return(src =>
            {
                var serializer = new XmlSerializer(typeof (TObject));
                var result = default(TObject);
                using (var stream = new MemoryStream(source))
                {
                    try
                    {
                        result = (TObject)serializer.Deserialize(stream);
                    }
                    catch (Exception)
                    {
                    }
                }

                return result;
            }, default(TObject));
        }

        public static byte[] Serialize<TObject>(this TObject source) where TObject : class
        {
            return source.Return(src =>
            {
                var serializer = new XmlSerializer(typeof(TObject));
                byte[] buffer = null;
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        serializer.Serialize(stream, source);
                        buffer = stream.ToArray();
                    }
                    catch (Exception)
                    {
                    }
                }

                return buffer;
            }, null);
        }
    }
}
