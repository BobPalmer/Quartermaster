using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_exploring_ingredients
    {
        [TestMethod]
        public void Should_be_able_to_create_an_ingredient()
        {
            var thisIngredient = new Ingredient();
            Assert.IsNotNull(thisIngredient);
        }

        [TestMethod]
        public void An_ingredient_has_a_resource_name()
        {
            var thisIngredient = new Ingredient();
            thisIngredient.ResourceName = "Widgets";
            var expected = "Widgets";
            var actual = thisIngredient.ResourceName;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void An_ingredient_has_a_quantity()
        {
            var thisIngredient = new Ingredient();
            thisIngredient.Quantity = 5;
            var expected = 5;
            var actual = thisIngredient.Quantity;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Ingredients_can_be_initialized_on_creation()
        {
            var thisIngredient = new Ingredient("Gizmos", 12);
            var expectedName = "Gizmos";
            var expectedQty = 12;
            var actualName = thisIngredient.ResourceName;
            var actualQty = thisIngredient.Quantity;
            Assert.AreEqual(expectedName, actualName);
            Assert.AreEqual(expectedQty, actualQty);
        }
    }
}
