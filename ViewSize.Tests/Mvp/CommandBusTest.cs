using System;
using System.Collections.Generic;
using CRLFLabs.ViewSize.Mvp;
using NUnit.Framework;

namespace ViewSize.Tests.Mvp
{
    public class CommandBusTest
    {
        private CommandBus _commandBus;

        [SetUp]
        public void SetUp()
        {
            _commandBus = new CommandBus();   
        }

        [Test]
        public void Publish_NoSubscriptions_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                _commandBus.Publish("hello");
            });
        }

        [Test]
        public void Publish_Consume_Works()
        {
            string result = "original";
            _commandBus.Subscribe("hello", () =>
            {
                result = "something else";
            });

            _commandBus.Publish("hello");

            Assert.AreEqual("something else", result);
        }
    }
}
