using BinarySerializer;

namespace Tests
{
	public class Level
	{
		[Order(0)]
		public string Name { get; set; }
		[Order(1)]
		public int Value { get; set; }
	}

	public class MethodItem
	{
		public MethodItem() => Parameters = [];
		[Order(0)]
		public string Name { get; set; }
		[Order(1)]
		public List<string> Parameters { get; set; }
	}

	public class StackFrameItem
	{
		[Order(0)]
		public string ClassName { get; set; }
		[Order(1)]
		public string FileName { get; set; }
		[Order(2)]
		public string LineNumber { get; set; }
		[Order(3)]
		public MethodItem Method { get; set; }
	}

	public class LocationInfo
	{
		public LocationInfo() => StackFrames = [];
		[Order(0)]
		public string ClassName { get; set; }
		[Order(1)]
		public string FileName { get; set; }
		[Order(2)]
		public string LineNumber { get; set; }
		[Order(3)]
		public string MethodName { get; set; }
		[Order(4)]
		public List<StackFrameItem> StackFrames { get; set; }
	}

	public class LoggingEvent
	{
		public LoggingEvent() => Properties = [];
		[Order(0)]
		public Level Level { get; set; }
		[Order(1)]
		public DateTime TimeStamp { get; set; }
		[Order(2)]
		public string LoggerName { get; set; }
		[Order(3)]
		public LocationInfo LocationInformation { get; set; }
		[Order(4)]
		public object MessageObject { get; set; }
		
		[Order(5)]
		public Dictionary<string, object> Properties { get; set; }
	}

	public class MessageObjectTest
	{
		[Order(0)]
		public string Message { get; set; }
		[Order(1)]
		public int Id { get; set; }
	}
}
