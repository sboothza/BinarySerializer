using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BinarySerializer;

using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class BinaryStreamTests
	{
		[Test]
		public void TestBasicIO()
		{
			var stream = new BinaryStream();
			
			stream.Write((byte)6);
			stream.Write((float)7);
			stream.Write((double)8);
			stream.Write((Int16)9);
			stream.Write((Int32)10);
			stream.Write((Int64)11);
			var str = "this is a test string";
			stream.Write((byte)str.Length);
			stream.Write(str);

			var buffer = stream.Buffer;

			var newStream = new BinaryStream(buffer);
            var byte_ = newStream.ReadByte();
			Assert.That(byte_==6, Is.True);

			var float_ = newStream.ReadFloat();
			Assert.That(float_ == 7, Is.True);

			var double_ = newStream.ReadDouble();
			Assert.That(double_ == 8, Is.True);

			var int16_ = newStream.ReadInt16();
			Assert.That(int16_ == 9, Is.True);

			var int32_ = newStream.ReadInt32();
			Assert.That(int32_ == 10, Is.True);

			var int64_ = newStream.ReadInt64();
			Assert.That(int64_ == 11, Is.True);

			var strLen = newStream.ReadByte();
			var newStr = newStream.ReadString(strLen);
			Assert.That(str.Equals(newStr), Is.True);
		}

		[Test]
		public void TestSmarterIO()
		{
			var stream = new TypedBinaryStream();

			stream.Write((byte)6);
			stream.Write((float)7);
			stream.Write((double)8);
			stream.Write((Int16)9);
			stream.Write((Int32)10);
			stream.Write((Int64)11);
			stream.Write("this is a test string");

			var buffer = stream.Buffer;

			var newStream = new TypedBinaryStream(buffer);
			var byte_ = newStream.Read<byte>();
			Assert.That(byte_ == 6, Is.True);

			var float_ = newStream.Read<float>();
			Assert.That(float_ == 7, Is.True);

			var double_ = newStream.Read<double>();
			Assert.That(double_ == 8, Is.True);

			var int16_ = newStream.Read<Int16>();
			Assert.That(int16_ == 9, Is.True);

			var int32_ = newStream.Read<Int32>();
			Assert.That(int32_ == 10, Is.True);

			var int64_ = newStream.Read<Int64>();
			Assert.That(int64_ == 11, Is.True);

			var newStr = newStream.Read<string>();
			Assert.That(newStr.Equals("this is a test string"), Is.True);
		}
	}
}
