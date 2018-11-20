namespace WOLF.Tests.Unit.Mocks
{
    public class MockFuelRecipe : Recipe
    {
        public MockFuelRecipe()
        {
            InputIngredients.Add("ElectricCharge", new RecipeIngredient
            {
                ResourceName = "ElectricCharge",
                Quantity = 100d,
                Rate = ContractRateUnit.PerMinute
            });
            InputIngredients.Add("Ore", new RecipeIngredient
            {
                ResourceName = "Ore",
                Quantity = 20d,
                Rate = ContractRateUnit.PerMinute
            });

            OutputIngredients.Add("LiquidFuel", new RecipeIngredient
            {
                ResourceName = "LiquidFuel",
                Quantity = 9d,
                Rate = ContractRateUnit.PerMinute
            });
            OutputIngredients.Add("Oxidizer", new RecipeIngredient
            {
                ResourceName = "Oxidizer",
                Quantity = 11d,
                Rate = ContractRateUnit.PerMinute
            });
        }
    }

    public class MockSolarPanelRecipe : Recipe
    {
        public MockSolarPanelRecipe()
        {
            OutputIngredients.Add("ElectricCharge", new RecipeIngredient
            {
                ResourceName = "ElectricCharge",
                Quantity = 1.8d,
                Rate = ContractRateUnit.PerSecond
            });
        }
    }

    public class MockGypsumFertilizerRecipe : Recipe
    {
        public MockGypsumFertilizerRecipe()
        {
            InputIngredients.Add("ElectricCharge", new RecipeIngredient
            {
                ResourceName = "ElectricCharge",
                Quantity = 100d,
                Rate = ContractRateUnit.PerMinute
            });
            InputIngredients.Add("Gypsum", new RecipeIngredient
            {
                ResourceName = "Gypsum",
                Quantity = 20d,
                Rate = ContractRateUnit.PerMinute
            });
            InputIngredients.Add("Machinery", new RecipeIngredient
            {
                ResourceName = "Machinery",
                Quantity = 0.02d,
                Rate = ContractRateUnit.PerDay
            });

            OutputIngredients.Add("Fertilizer", new RecipeIngredient
            {
                ResourceName = "Fertilizer",
                Quantity = 10d,
                Rate = ContractRateUnit.PerMinute
            });
            OutputIngredients.Add("Recyclables", new RecipeIngredient
            {
                ResourceName = "Recyclables",
                Quantity = 0.02d,
                Rate = ContractRateUnit.PerDay
            });
        }
    }

    public class MockMineralFertilizerRecipe : Recipe
    {
        public MockMineralFertilizerRecipe()
        {
            InputIngredients.Add("ElectricCharge", new RecipeIngredient
            {
                ResourceName = "ElectricCharge",
                Quantity = 100d,
                Rate = ContractRateUnit.PerMinute
            });
            InputIngredients.Add("Minerals", new RecipeIngredient
            {
                ResourceName = "Minerals",
                Quantity = 20d,
                Rate = ContractRateUnit.PerMinute
            });
            InputIngredients.Add("Machinery", new RecipeIngredient
            {
                ResourceName = "Machinery",
                Quantity = 0.02d,
                Rate = ContractRateUnit.PerDay
            });

            OutputIngredients.Add("Fertilizer", new RecipeIngredient
            {
                ResourceName = "Fertilizer",
                Quantity = 5d,
                Rate = ContractRateUnit.PerMinute
            });
            OutputIngredients.Add("Recyclables", new RecipeIngredient
            {
                ResourceName = "Recyclables",
                Quantity = 0.02d,
                Rate = ContractRateUnit.PerDay
            });
        }
    }

    public class MockOreFertilizerRecipe : Recipe
    {
        public MockOreFertilizerRecipe()
        {
            InputIngredients.Add("ElectricCharge", new RecipeIngredient
            {
                ResourceName = "ElectricCharge",
                Quantity = 100d,
                Rate = ContractRateUnit.PerMinute
            });
            InputIngredients.Add("Ore", new RecipeIngredient
            {
                ResourceName = "Minerals",
                Quantity = 20d,
                Rate = ContractRateUnit.PerMinute
            });

            OutputIngredients.Add("Fertilizer", new RecipeIngredient
            {
                ResourceName = "Fertilizer",
                Quantity = 2d,
                Rate = ContractRateUnit.PerMinute
            });
        }
    }
}
