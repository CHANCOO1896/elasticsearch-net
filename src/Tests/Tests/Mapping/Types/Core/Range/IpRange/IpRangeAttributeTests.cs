using Elastic.Xunit.XunitPlumbing;
using Nest;

namespace Tests.Mapping.Types.Core.Range.IpRange
{
	public class IpRangeTest
	{
		[IpRange]
		public Nest.IpAddressRange Range { get; set; }
	}

	public class IpRangeAttributeTests : AttributeTestsBase<IpRangeTest>
	{
		protected override object ExpectJson => new
		{
			properties = new
			{
				range = new
				{
					type = "ip_range"
				}
			}
		};
	}
}
