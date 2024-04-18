using System.Collections;
using System.Collections.Generic;

namespace BinarySerializer
{
	/// <summary>
	/// Format is recursive:
	/// type:byte
	/// value - dependent on type
	/// len:value - arrays or strings	
	/// </summary>
	public static class Serializer
	{
		private static void Write<T>(TypedBinaryStream stream, T original)
		{
			if (original is bool vBool)
				stream.Write(vBool ? (byte)1 : (byte)0);
			else
			{
				//get smallest int types
				if (original is byte vByte)
					stream.Write(vByte);
				else if (original is Int16 vInt16)
				{
					if ((byte)vInt16 == vInt16)
						stream.Write((byte)vInt16);
					else
						stream.Write(vInt16);
				}
				else if (original is Int32 vInt32)
				{
					if ((byte)vInt32 == vInt32)
						stream.Write((byte)vInt32);
					else if ((Int16)vInt32 == vInt32)
						stream.Write((Int16)vInt32);
					else
						stream.Write(vInt32);
				}
				else if (original is Int64 vInt64)
				{
					if ((byte)vInt64 == vInt64)
						stream.Write((byte)vInt64);
					else if ((Int16)vInt64 == vInt64)
						stream.Write((Int16)vInt64);
					else if ((Int32)vInt64 == vInt64)
						stream.Write((Int32)vInt64);
					else
						stream.Write(vInt64);
				}
				else if (original is float vFloat)
				{
					var intValue = Math.Round(vFloat);
					if (intValue.Equals(vFloat))
					{
						if ((byte)intValue == intValue)
							stream.Write((byte)intValue);
						else if ((Int16)intValue == intValue)
							stream.Write((Int16)intValue);
						else
							stream.Write(vFloat);
					}
					else
						stream.Write(vFloat);
				}
				else if (original is double vDouble)
				{
					var intValue = Math.Round(vDouble);
					if (intValue.Equals(vDouble))
					{
						if ((byte)intValue == intValue)
							stream.Write((byte)intValue);
						else if ((Int16)intValue == intValue)
							stream.Write((Int16)intValue);
						else if ((Int32)intValue == intValue)
							stream.Write((Int32)intValue);
						else
							stream.Write(vDouble);
					}
					else
						stream.Write(vDouble);
				}
				else if (original is DateTime vDateTime)
					stream.Write(vDateTime);
				else if (original is string strValue)
					stream.Write(strValue);
				else
					throw new InvalidCastException("Unknown type");
			}
		}
		
		public static byte[] Serialize(object source)
		{
			var stream = new TypedBinaryStream();
			SerializeObject(stream, source);
			return stream.Buffer;
		}

		private static void SerializeObject(TypedBinaryStream stream, object source)
		{
			if (source is null)
			{
				stream.WriteByteRaw((byte)DataType.Null);
			}
			else if (IsPrimitive(source))
			{
				Write(stream, source);
			}
			else if (source is IEnumerable enumerable && source is not string)
			{
				if (source is not IDictionary)
				{
					var list = (IList)source;
					stream.WriteByteRaw((byte)DataType.List);
					stream.WriteByteRaw((byte)list.Count);
					foreach (var item in list)
					{
						SerializeObject(stream, item);
					}
				}
				else
				{
					var dict = (IDictionary)source;
					stream.WriteByteRaw((byte)DataType.Dictionary);
					stream.WriteByteRaw((byte)(dict.Count));

					foreach (var key in dict.Keys)
					{
						SerializeObject(stream, key);
						SerializeObject(stream, dict[key]);
					}
				}
			}
			else
			{
				stream.WriteByteRaw((byte)DataType.Object);

				var properties = source.GetType()
					.GetProperties();

				var props = properties.Select(p => new
				{
					prop = p,
					order = ((OrderAttribute)p.GetCustomAttributes(typeof(OrderAttribute), false).FirstOrDefault())?.Order ?? 0,
					ignore = ((OrderAttribute)p.GetCustomAttributes(typeof(OrderAttribute), false).FirstOrDefault())?.Ignore ?? false
				}).Where(p => p.prop.CanRead && p.prop.CanWrite && !p.ignore)
					.OrderBy(s => s.order);

				foreach (var prop in props)
				{
					var value = prop.prop.GetValue(source);
					if (IsPrimitive(value))
						Write(stream, value);
					else
					{
						SerializeObject(stream, value);
					}
				}
			}
		}

		public static T Deserialize<T>(byte[] buffer)
		{
			var stream = new TypedBinaryStream(buffer);
			return (T)DeserializeObject(stream, typeof(T));
		}

		private static object DeserializeObject(TypedBinaryStream stream, Type type)
		{
			var dataType = (DataType)stream.ReadByte();
			if (dataType is DataType.Null)
			{
				return null;
			}
			else if (dataType is DataType.List)
			{
				var count = stream.ReadByte();
				IList list = null;
				Type itemType = null;

				if (type.IsArray)
				{
					itemType = type.GetElementType();
					list = Array.CreateInstance(itemType, count);
				}
				else if (type.IsGenericType)
				{
					itemType = type.GenericTypeArguments[0];
					var listType = type.BaseType;

					list = (IList)Activator.CreateInstance(type);
					for (var i = 0; i < count; i++)
						list.Add(null);
				}

				for (var i = 0; i < count; i++)
				{
					var item = DeserializeObject(stream, itemType);
					list[i] = item;
				}
				return list;
			}
			else if (dataType == DataType.Dictionary)
			{
				var count = stream.ReadByte();
				var dict = (IDictionary)Activator.CreateInstance(type);
				var keyType = type.GenericTypeArguments[0];
				var valueType = type.GenericTypeArguments[1];

				for (var i = 0; i < count; i++)
				{
					var key = DeserializeObject(stream, keyType);
					var value = DeserializeObject(stream, valueType);
					dict[key] = value;
				}
				return dict;
			}
			else if (dataType == DataType.Object)
			{
				object result = Activator.CreateInstance(type);

				var properties = result.GetType()
					.GetProperties();

				var props = properties.Select(p => new
				{
					prop = p,
					order = ((OrderAttribute)p.GetCustomAttributes(typeof(OrderAttribute), false).FirstOrDefault())?.Order ?? 0,
					ignore = ((OrderAttribute)p.GetCustomAttributes(typeof(OrderAttribute), false).FirstOrDefault())?.Ignore ?? false
				}).Where(p => p.prop.CanRead && p.prop.CanWrite && !p.ignore)
					.OrderBy(s => s.order);

				foreach (var prop in props)
				{
					var value = DeserializeObject(stream, prop.prop.PropertyType);
					prop.prop.SetValue(result, value);
				}

				return result;
			}
			else
			{
				return stream.Read(dataType, type);
			}
		}

		private static bool IsPrimitive(object original)
		{
			return ((original is bool vBool) || (original is byte vByte) || (original is Int16 vInt16) || (original is Int32 vInt32) || (original is Int64 vInt64) || (original is float vFloat) || (original is double vDouble) || (original is DateTime vDateTime) || (original is string strValue));
		}

	}
}
