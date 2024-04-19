namespace BinarySerializer
{
    public class TypedBinaryStream : BinaryStream
    {
        public TypedBinaryStream()
        {
        }

        public TypedBinaryStream(int length) : base(length)
        {
        }

        public TypedBinaryStream(byte[] data) : base(data)
        {
        }

        public void WritePrimitive<T>(T original)
        {
            if (original is bool vBool)
                Write(vBool ? (byte)1 : (byte)0);
            else
            {
                //get smallest int types
                if (original is byte vByte)
                    Write(vByte);
                else if (original is Int16 vInt16)
                {
                    if ((byte)vInt16 == vInt16)
                        Write((byte)vInt16);
                    else
                        Write(vInt16);
                }
                else if (original is Int32 vInt32)
                {
                    if ((byte)vInt32 == vInt32)
                        Write((byte)vInt32);
                    else if ((Int16)vInt32 == vInt32)
                        Write((Int16)vInt32);
                    else
                        Write(vInt32);
                }
                else if (original is Int64 vInt64)
                {
                    if ((byte)vInt64 == vInt64)
                        Write((byte)vInt64);
                    else if ((Int16)vInt64 == vInt64)
                        Write((Int16)vInt64);
                    else if ((Int32)vInt64 == vInt64)
                        Write((Int32)vInt64);
                    else
                        Write(vInt64);
                }
                else if (original is float vFloat)
                {
                    var intValue = Math.Round(vFloat);
                    if (intValue.Equals(vFloat))
                    {
                        if ((byte)intValue == intValue)
                            Write((byte)intValue);
                        else if ((Int16)intValue == intValue)
                            Write((Int16)intValue);
                        else
                            Write(vFloat);
                    }
                    else
                        Write(vFloat);
                }
                else if (original is double vDouble)
                {
                    var intValue = Math.Round(vDouble);
                    if (intValue.Equals(vDouble))
                    {
                        if ((byte)intValue == intValue)
                            Write((byte)intValue);
                        else if ((Int16)intValue == intValue)
                            Write((Int16)intValue);
                        else if ((Int32)intValue == intValue)
                            Write((Int32)intValue);
                        else
                            Write(vDouble);
                    }
                    else
                        Write(vDouble);
                }
                else if (original is DateTime vDateTime)
                    Write(vDateTime);
                else if (original is string strValue)
                    Write(strValue);
                else
                    throw new InvalidCastException("Unknown type");
            }
        }

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