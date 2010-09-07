using NUnit.Framework;
using Test.Support;

namespace UEEditor.Tests
{
	[SetUpFixture]
	public class IntegrationSetup
	{
		[SetUp]
		public void SetUp()
		{
			Setup.Initialize();
		}
	}
}