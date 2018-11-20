using WOLF.Tests.Unit.Mocks;
using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_exploring_recipes
    {
        [Fact]
        public void Should_have_a_list_of_inputs_by_default()
        {
            var recipe = new Recipe();

            Assert.NotNull(recipe.InputIngredients);
        }

        [Fact]
        public void Should_have_a_list_of_outputs_by_default()
        {
            var recipe = new Recipe();

            Assert.NotNull(recipe.OutputIngredients);
        }
    }
}
