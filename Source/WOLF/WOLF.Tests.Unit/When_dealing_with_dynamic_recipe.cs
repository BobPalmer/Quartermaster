using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_dealing_with_dynamic_recipe
    {
        [TestMethod]
        public void Should_be_able_to_create_a_dynamic_ingredient()
        {
            var thisIngredient = DynamicIngredients.GetDynamicHarvester();
            Assert.IsNotNull(thisIngredient);
        }

        [TestMethod]
        public void Should_be_able_to_create_different_dynamic_ingredients()
        {
            var thisIngredient = DynamicIngredients.GetSolarCollector();
            Assert.IsNotNull(thisIngredient);
        }
    }
}