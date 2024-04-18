using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer
{
	public enum DataType : byte
	{
		Unknown = 0,
		Bool = 1,
		Byte = 2,
		Int16 = 3,
		Int32 = 4,
		Int64 = 5,
		Float = 6,
		Double = 7,
		DateTime = 8,
		String = 9,
		Object = 10,
		List = 11,
		Null = 12,
		Dictionary=13
	}
}
