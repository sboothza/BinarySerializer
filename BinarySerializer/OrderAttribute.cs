using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class OrderAttribute : Attribute
	{
		public int Order { get; set; }
		public bool Ignore { get; set; }

		public OrderAttribute(int order, bool ignore = false)
		{
			Order = order;
			Ignore = ignore;
		}
	}
}
