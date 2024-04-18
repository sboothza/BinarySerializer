using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer
{
	public class TypedBinaryStream : BinaryStream
	{
		public TypedBinaryStream() : base() { }

		public TypedBinaryStream(int length) : base(length) { }

		public TypedBinaryStream(byte[] data) : base(data) { }
		public virtual void Write(bool value)
		{
			base.Write((byte)DataType.Bool);
			base.Write(value ? (byte)1 : (byte)0);
		}
		public override void Write(byte value)
		{
			base.Write((byte)DataType.Byte);
			base.Write(value);
		}

		public override void Write(Int16 value)
		{
			base.Write((byte)DataType.Int16);
			base.Write(value);
		}

		public override void Write(Int32 value)
		{
			base.Write((byte)DataType.Int32);
			base.Write(value);
		}

		public override void Write(Int64 value)
		{
			base.Write((byte)DataType.Int64);
			base.Write(value);
		}

		public override void Write(float value)
		{
			base.Write((byte)DataType.Float);
			base.Write(value);
		}

		public override void Write(double value)
		{
			base.Write((byte)DataType.Double);
			base.Write(value);
		}

		public virtual void Write(DateTime value)
		{
			base.Write((byte)DataType.DateTime);
			base.Write(value.ToBinary());
		}

		public override void Write(string value)
		{
			base.Write((byte)DataType.String);
			base.Write((byte)value.Length);
			base.Write(value);
		}

		public virtual void WriteByteRaw(byte value)
		{
			base.Write(value);
		}		

		public virtual object Read(DataType dataType, Type type)
		{
			switch (dataType)
			{
				case DataType.Bool:
					return Convert.ChangeType(base.ReadByte() > 0, type);
				case DataType.Byte:
					return Convert.ChangeType(base.ReadByte(), type);
				case DataType.Int16:
					return Convert.ChangeType(base.ReadInt16(), type);
				case DataType.Int32:
					return Convert.ChangeType(base.ReadInt32(), type);
				case DataType.Int64:
					return Convert.ChangeType(base.ReadInt64(), type);
				case DataType.Float:
					return Convert.ChangeType(base.ReadFloat(), type);
				case DataType.Double:
					return Convert.ChangeType(base.ReadDouble(), type);
				case DataType.DateTime:
				{
					var ticks = base.ReadInt64();
					return Convert.ChangeType(DateTime.FromBinary(ticks), type);
				}
				case DataType.String:
				{
					var len = base.ReadByte();
					return Convert.ChangeType(base.ReadString(len), type);
				}
			}
			return default;
		}

		public virtual object Read(Type type)
		{
			var dataType = (DataType)base.ReadByte();
			return Read(dataType, type);
		}

		public virtual T Read<T>()
		{
			return (T)Read(typeof(T));
		}
	}
}
