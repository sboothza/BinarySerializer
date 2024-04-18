using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using BinarySerializer;

using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

namespace Tests
{
	public class TestChild
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

	public class TestBase
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
		public List<TestChild> ChildProperty { get; set; } = new List<TestChild>();
		[BinarySerializer.Order(6)]
		public int LastIntProperty { get; set; }
	}


	[TestFixture]
	public class ComplexTests
	{
		private LoggingEvent _event;
		private int loops = 1000;

		[SetUp]
		public void Setup()
		{
			_event = new LoggingEvent
			{
				Level = new Level
				{
					Name = "ERROR",
					Value = 1000
				},
				LoggerName = "Test",
				TimeStamp = DateTime.Now,
				MessageObject = new MessageObjectTest
				{
					Message = "this is a message",
					Id = 1
				},
				LocationInformation = new LocationInfo
				{
					ClassName = "JsonTests.NewtonsoftTests",
					FileName = "c:\\temp\\file.cs",
					LineNumber = "12",
					MethodName = "Test",
					StackFrames =
					{
						new StackFrameItem
						{
							ClassName = "JsonTests.NewtonsoftTests",
							FileName = "c:\\temp\\file.cs",
							LineNumber = "12",
							Method = new MethodItem
							{
								Name = "Test",
								Parameters =
								{
									"param1",
									"param2"
								}
							}
						},
						new StackFrameItem
						{
							ClassName = "JsonTests.NewtonsoftTests",
							FileName = "c:\\temp\\file.cs",
							LineNumber = "13",
							Method = new MethodItem
							{
								Name = "Test2",
								Parameters =
								{
									"param3",
									"param4"
								}
							}
						}
					}
				},
				Properties =
				{
					{
						"key1", "value1"
					},
					{
						"key2", "value2"
					},
					{
						"key3", "value3"
					}
				}
			};
		}
		[Test]
		public void TestComplex()
		{
			var objs = new List<TestChild> {
				new TestChild { IntProperty = 1, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now },
			new TestChild{ IntProperty = 2, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now },
			new TestChild{ IntProperty = 3, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now }
			};

			var oldObj = new TestBase { IntProperty = 1, LongProperty = 2, StringProperty = "Testing...testing...", DateTimeProperty = DateTime.Now, ChildProperty = objs, LastIntProperty = 67 };

			var buffer = Serializer.Serialize(oldObj);

			var newObj = Serializer.Deserialize<TestBase>(buffer);

			Assert.That(oldObj.IntProperty == newObj.IntProperty, Is.True);
			Assert.That(oldObj.LongProperty == newObj.LongProperty, Is.True);
			Assert.That(oldObj.StringProperty == newObj.StringProperty, Is.True);
			Assert.That(oldObj.DateTimeProperty == newObj.DateTimeProperty, Is.True);
			Assert.That(oldObj.LastIntProperty == newObj.LastIntProperty, Is.True);

			Assert.That(oldObj.ChildProperty.Count, Is.EqualTo(3));
		}

		[Test]
		public void TestDictionary()
		{
			var obj = new Dictionary<string, string>();
			obj["key1"] = "value1";
			obj["key2"] = "value2";
			obj["key3"] = "value3";

			var buffer = Serializer.Serialize(obj);
			var newObj = Serializer.Deserialize<Dictionary<string, string>>(buffer);

			Assert.That(newObj.Count, Is.EqualTo(3));
			Assert.That(newObj["key2"] == "value2");
		}

		[Test]
		public void TestObjectDictionary()
		{
			var obj = new Dictionary<string, object>();
			obj["key1"] = "value1";
			obj["key2"] = "value2";
			obj["key3"] = "value3";

			var buffer = Serializer.Serialize(obj);
			var newObj = Serializer.Deserialize<Dictionary<string, object>>(buffer);

			Assert.That(newObj.Count, Is.EqualTo(3));
			Assert.That(newObj["key2"].Equals("value2"));
		}

		//206
		[Test]
		public void CustomSerialize()
		{
			var obj = _event.LocationInformation;
			var buffer = Serializer.Serialize(obj);
            Console.WriteLine(buffer.Length);
        }

		//35.61
		[Test]
		public void MeasureCustomSerialize()
		{
			var obj = _event.LocationInformation;
			var start = DateTime.Now;
			for (var i = 0; i < loops; i++)
			{
				var buffer = Serializer.Serialize(obj);
				if (buffer.Length < 0)
					throw new InvalidOperationException("broke");

			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}

		//34.99
		[Test]
		public void MeasureCustomDeserialize()
		{
			var objTemp = _event.LocationInformation;
			var buffer = Serializer.Serialize(objTemp);

			var start = DateTime.Now;
			for (var i = 0; i < loops; i++)
			{
				var obj = Serializer.Deserialize<LocationInfo>(buffer);
				if (obj.ClassName != "JsonTests.NewtonsoftTests")
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}
	}
}
