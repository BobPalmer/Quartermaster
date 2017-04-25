﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_exploring_ResourceLinks
    {
        [TestMethod]
        public void Should_be_able_to_create_a_resource_link()
        {
            var thisLink = new ResourceLink();
            Assert.IsNotNull(thisLink);
        }

        [TestMethod]
        public void Should_have_a_source_point()
        {
            var thisLink = new ResourceLink();
            var g = "SOURCE";
            thisLink.SourceId = g;
            Assert.AreEqual(g,thisLink.SourceId);
        }

        [TestMethod]
        public void Should_have_a_destination_point()
        {
            var thisLink = new ResourceLink();
            var g = "DEST";
            thisLink.DestinationId = g;
            Assert.AreEqual(g, thisLink.DestinationId);
        }

        [TestMethod]
        public void Should_have_a_resource_name()
        {
            var thisLink = new ResourceLink();
            var r = "Ore";
            thisLink.ResourceName = r;
            Assert.AreEqual(r, thisLink.ResourceName);
        }

        [TestMethod]
        public void Should_have_a_quantity()
        {
            var thisLink = new ResourceLink();
            var q = 100;
            thisLink.Quantity = q;
            Assert.AreEqual(q, thisLink.Quantity);
        }
    }
}