using System.Security.Cryptography;

using BinarySerializer;

using NUnit.Framework;

namespace Tests
{
	public class TestClass
	{
		[BinarySerializer.Order(1)]
		public int IntProperty { get; set; }
		[BinarySerializer.Order(2)]
		public long LongProperty { get; set; }
		[BinarySerializer.Order(3)]
		public string StringProperty { get; set; }
		[BinarySerializer.Order(4)]
		public DateTime DateTimeProperty { get; set; }
	}

	public class TestParent
	{
		[BinarySerializer.Order(1)]
		public int IntProperty { get; set; }
		[BinarySerializer.Order(2)]
		public long LongProperty { get; set; }
		[BinarySerializer.Order(3)]
		public string StringProperty { get; set; }
		[BinarySerializer.Order(4)]
		public DateTime DateTimeProperty { get; set; }
		[BinarySerializer.Order(5)]
		public TestClass ChildProperty { get; set; }
		[BinarySerializer.Order(6)]
		public int LastIntProperty { get; set; }
	}

	public class TestExceptionClass
	{
        public string StringProperty { get; set; }
        public Exception ExceptionProperty { get; set; }
    }

	[TestFixture]
	public class BasicTests
	{
		[Test]
		public void TestPass()
		{
			Assert.Pass();
		}

		//[Test]
		//public void BasicIOTest()
		//{
		//	var serializer = new Serializer();

		//	serializer.Write((byte)6);
		//	serializer.Write((float)7);
		//	serializer.Write((double)8);
		//	serializer.Write((Int16)9);
		//	serializer.Write((Int32)10);
		//	serializer.Write((Int64)11);
		//	serializer.Write("this is a test string");

		//	var buffer = serializer.Buffer;

		//	var newStream = new Serializer(buffer);
		//	var byte_ = newStream.Read<byte>();
		//	Assert.That(byte_ == 6, Is.True);

		//	var float_ = newStream.Read<float>();
		//	Assert.That(float_ == 7, Is.True);

		//	var double_ = newStream.Read<double>();
		//	Assert.That(double_ == 8, Is.True);

		//	var int16_ = newStream.Read<Int16>();
		//	Assert.That(int16_ == 9, Is.True);

		//	var int32_ = newStream.Read<Int32>();
		//	Assert.That(int32_ == 10, Is.True);

		//	var int64_ = newStream.Read<Int64>();
		//	Assert.That(int64_ == 11, Is.True);

		//	var newStr = newStream.Read<string>();
		//	Assert.That(newStr.Equals("this is a test string"), Is.True);
		//}

		[Test]
		public void BasicSerialize()
		{
			var oldObj = new TestClass { IntProperty = 1, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now };

			var buffer = Serializer.Serialize(oldObj);

			var newObj = Serializer.Deserialize<TestClass>(buffer);

			Assert.That(oldObj.IntProperty == newObj.IntProperty, Is.True);
			Assert.That(oldObj.LongProperty == newObj.LongProperty, Is.True);
			Assert.That(oldObj.StringProperty == newObj.StringProperty, Is.True);
			Assert.That(oldObj.DateTimeProperty == newObj.DateTimeProperty, Is.True);
		}

		[Test]
		public void HierarchySerialize()
		{			
			var oldObj = new TestParent { IntProperty = 1, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now, ChildProperty = new TestClass { IntProperty = 1, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now }, LastIntProperty = 67 };

			var buffer = Serializer.Serialize(oldObj);			
			
			var newObj = Serializer.Deserialize<TestParent>(buffer);

			Assert.That(oldObj.IntProperty == newObj.IntProperty, Is.True);
			Assert.That(oldObj.LongProperty == newObj.LongProperty, Is.True);
			Assert.That(oldObj.StringProperty == newObj.StringProperty, Is.True);
			Assert.That(oldObj.DateTimeProperty == newObj.DateTimeProperty, Is.True);
			Assert.That(oldObj.LastIntProperty == newObj.LastIntProperty, Is.True);

			Assert.That(oldObj.ChildProperty.IntProperty == newObj.ChildProperty.IntProperty, Is.True);
			Assert.That(oldObj.ChildProperty.LongProperty == newObj.ChildProperty.LongProperty, Is.True);
			Assert.That(oldObj.ChildProperty.StringProperty == newObj.ChildProperty.StringProperty, Is.True);
			Assert.That(oldObj.ChildProperty.DateTimeProperty == newObj.ChildProperty.DateTimeProperty, Is.True);
		}

		[Test]
		public void HierarchySerializeNull()
		{
			var oldObj = new TestParent { IntProperty = 1, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now, ChildProperty = null, LastIntProperty = 67 };

			var buffer = Serializer.Serialize(oldObj);

			var newObj = Serializer.Deserialize<TestParent>(buffer);

			Assert.That(oldObj.IntProperty == newObj.IntProperty, Is.True);
			Assert.That(oldObj.LongProperty == newObj.LongProperty, Is.True);
			Assert.That(oldObj.StringProperty == newObj.StringProperty, Is.True);
			Assert.That(oldObj.DateTimeProperty == newObj.DateTimeProperty, Is.True);
			Assert.That(oldObj.LastIntProperty == newObj.LastIntProperty, Is.True);

			Assert.That(oldObj.ChildProperty is null, Is.True);
		}

		[Test]
		public void TestArray()
		{
			var objs = new[] {
				new TestClass { IntProperty = 1, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now },
			new TestClass{ IntProperty = 2, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now },
			new TestClass{ IntProperty = 3, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now }
			};

			var buffer = Serializer.Serialize(objs);
			var obj = Serializer.Deserialize<TestClass[]>(buffer);
			Assert.That(obj.Length, Is.EqualTo(3));
		}

		[Test]
		public void TestList()
		{
			var objs = new List<TestClass> {
				new TestClass { IntProperty = 1, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now },
			new TestClass{ IntProperty = 2, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now },
			new TestClass{ IntProperty = 3, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now }
			};

			var buffer = Serializer.Serialize(objs);

			var obj = Serializer.Deserialize<List<TestClass>>(buffer);
			Assert.That(obj.Count, Is.EqualTo(3));
		}

		[Test]
		public void TestException()
		{
			var obj = new TestExceptionClass { StringProperty = "Test", ExceptionProperty = new InvalidCastException("Testing") };
			var buffer = Serializer.Serialize(obj);
			var newObj = Serializer.Deserialize<TestExceptionClass>(buffer);
			Assert.That(newObj.StringProperty, Is.EqualTo(obj.StringProperty));

		}
	}
}
