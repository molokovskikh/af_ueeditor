using Common.MySql;
using NUnit.Framework;

namespace UEEditor.Tests
{
	[TestFixture]
	public class HumanNameFixture
	{
		[Test]
		public void GetHumanName()
		{
			With.Connection(c => {
				var name = Updater.GetHumanName(c, "test1231423");
				Assert.That(name, Is.Empty);
			});
		}
	}
}