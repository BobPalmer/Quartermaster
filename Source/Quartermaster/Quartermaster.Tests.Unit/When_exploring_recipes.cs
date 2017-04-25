using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_exploring_recipes
    {

        [TestMethod]
        public void Should_be_able_to_create_a_new_recipe()
        {
            var thisRecipe = new Recipe();
            Assert.IsNotNull(thisRecipe);
        }


        [TestMethod]
        public void A_recipe_can_have_multiple_inputs()
        {
            var thisRecipe = new Recipe();
            var expected = 3;
            thisRecipe.AddInput(new Ingredient());
            thisRecipe.AddInput(new Ingredient());
            thisRecipe.AddInput(new Ingredient());
            var actual = thisRecipe.Inputs.Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void A_recipe_can_have_multiple_outputs()
        {
            var thisRecipe = new Recipe();
            var expected = 2;
            thisRecipe.AddOutput(new Ingredient());
            thisRecipe.AddOutput(new Ingredient());
            var actual = thisRecipe.Outputs.Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void A_recipe_can_have_multiple_requirements()
        {
            var thisRecipe = new Recipe();
            var expected = 4;
            thisRecipe.AddRequirement(new Ingredient());
            thisRecipe.AddRequirement(new Ingredient());
            thisRecipe.AddRequirement(new Ingredient());
            thisRecipe.AddRequirement(new Ingredient());
            var actual = thisRecipe.Requirements.Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void A_recipe_can_have_no_inputs()
        {
            var thisRecipe = new Recipe();
            var expected = 0;
            var actual = thisRecipe.Inputs.Count;
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void A_recipe_can_have_no_outputs()
        {
            var thisRecipe = new Recipe();
            var expected = 0;
            var actual = thisRecipe.Outputs.Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void A_recipe_can_have_no_requirements()
        {
            var thisRecipe = new Recipe();
            var expected = 0;
            var actual = thisRecipe.Requirements.Count;
            Assert.AreEqual(expected, actual);
        }

    }
}