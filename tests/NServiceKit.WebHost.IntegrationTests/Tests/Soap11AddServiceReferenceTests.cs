using System;
using System.ServiceModel;
using NUnit.Framework;
using NServiceKit.Text;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
	[TestFixture]
	public class Soap11AddServiceReferenceTests
	{
		private const string EndpointUri = "http://localhost/NServiceKit.WebHost.IntegrationTests/NServiceKit/Soap11";
		private Soap11ServiceReference.ISyncReply client;

		[SetUp]
		public void OnBeforeEachTest()
		{
			//Generated proxy when using 'Add Service Reference' on the EndpointUri above.
			//Thank WCF for the config ugliness
			client = new Soap11ServiceReference.SyncReplyClient(
				new BasicHttpBinding
				{
					MaxReceivedMessageSize = int.MaxValue,
					HostNameComparisonMode = HostNameComparisonMode.StrongWildcard
				},
				new EndpointAddress(EndpointUri));
		}

		private const string TestString = "NServiceKit";

		[Test]
		public void Does_Execute_ReverseService()
		{
			var response = client.Reverse(TestString);
			var expectedValue = ReverseService.Execute(TestString);
			Assert.That(response, Is.EqualTo(expectedValue));
		}

		[Test]
		public void Does_Execute_Rot13Service()
		{
			var response = client.Rot13(TestString);
			var expectedValue = TestString.ToRot13();
			Assert.That(response, Is.EqualTo(expectedValue));
		}

		[Test]
		public void Can_Handle_Exception_from_AlwaysThrowService()
		{
			string result;
			var responseStatus = client.AlwaysThrows(out result, TestString);

			var expectedError = AlwaysThrowsService.GetErrorMessage(TestString);
			Assert.That(responseStatus.ErrorCode,
				Is.EqualTo(typeof(NotImplementedException).Name));
			Assert.That(responseStatus.Message, Is.EqualTo(expectedError));
		}

	}
}