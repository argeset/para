using System;

using NUnit.Framework;
using Para.Server.Business.Manager;
using Para.Server.Contract.Argument;
using Para.Server.Contract.Enum;
using Para.Server.Contract.Response;

namespace Para.Server.Business.UnitTest
{
    [TestFixture]
    public class ParaServiseTests
    {
        ParaService _service;

        [SetUp]
        public void Setup()
        {
            _service = new ParaService(new ParaManager());
        }

        [Test]
        public void SaveValue_SavesValue()
        {
            _service.SaveValue();
        }

        [Test]
        public void GetValue_Returns_ResponseWithMessageAndValue()
        {
            var argument = new GetValueArgument();

            var response = _service.GetValue(argument);

            Assert.AreEqual(response.Message, ResponseMessage.Ok);
            Assert.IsNotNull(response.Value);
            Assert.Greater(response.Value, 0);
        }

        [Test]
        public void ConvertValue_Returns_NotNullResponse()
        {
            var argument = new ConvertValueArgument();

            var response = _service.ConvertValue(argument);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<Response>(response);
        }

        [Test]
        public void GetValue_Returns_NotNullResponse()
        {
            var argument = new GetValueArgument();

            var response = _service.GetValue(argument);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<Response>(response);
        }

        [Test]
        public void GetValueArgument_SetsDefaultValues_InConstructor()
        {
            var argument = new GetValueArgument();

            AssertDefaultArgumentValues(argument);
        }

        [Test]
        public void ConvertValueArgument_SetsDefaultValues_InConstructor()
        {
            var argument = new ConvertValueArgument();

            AssertDefaultArgumentValues(argument);
        }

        private static void AssertDefaultArgumentValues(BaseArgument argument)
        {
            Assert.AreEqual(argument.Source, Currency.TL);
            Assert.AreEqual(argument.Target, Currency.USD);
            Assert.AreEqual(argument.Type, CurrencyValueType.Banknote);
            Assert.AreEqual(argument.Time, DateTime.Today.ToString("yyyyMMdd"));
            Assert.AreEqual(argument.ValueSource, CurrencyValueSource.TCMB);
        }
    }
}
