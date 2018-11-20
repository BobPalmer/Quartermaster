using WOLF.Tests.Unit.Mocks;
using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_exploring_processors
    {
        [Fact]
        public void Should_be_active_by_default()
        {
            var processor = new ProcessorEndpoint();

            Assert.True(processor.IsActive);
        }

        [Fact]
        public void Should_have_no_recipe_by_default()
        {
            var processor = new ProcessorEndpoint();

            Assert.Null(processor.Recipe);
        }

        [Fact]
        public void Can_set_a_recipe()
        {
            var processor = new ProcessorEndpoint();
            var recipe = new Recipe();

            processor.SetRecipe(recipe);

            Assert.Equal(recipe, processor.Recipe);
        }

        [Fact]
        public void Can_change_recipes()
        {
            var processor = new ProcessorEndpoint();
            var oldRecipe = new Recipe();
            processor.SetRecipe(oldRecipe);

            var newRecipe = new Recipe();
            processor.SetRecipe(newRecipe);

            Assert.Equal(newRecipe, processor.Recipe);
        }

        [Fact]
        public void Changing_recipes_should_break_invalid_contracts()
        {
            var endpoint0 = new MockEndpoint();
            var processor = new ProcessorEndpoint();
            var oldRecipe = new MockMineralFertilizerRecipe();
            processor.SetRecipe(oldRecipe);
            var ecContract = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "ElectricCharge",
                Quantity = 100d,
                Rate = ContractRateUnit.PerMinute
            };
            var mineralContract = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "Minerals",
                Quantity = 20d,
                Rate = ContractRateUnit.PerMinute
            };
            var machineryContract = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "Machinery",
                Quantity = 0.02d,
                Rate = ContractRateUnit.PerDay
            };
            var fertilizerContract = new Contract
            {
                Source = processor,
                Destination = endpoint0,
                ResourceName = "Fertilizer",
                Quantity = 5d,
                Rate = ContractRateUnit.PerMinute
            };
            var recyclablesContract = new Contract
            {
                Source = processor,
                Destination = endpoint0,
                ResourceName = "Recyclables",
                Quantity = 0.02d,
                Rate = ContractRateUnit.PerDay
            };
            processor.Contracts.AddRange(new Contract[]
            {
                ecContract,
                mineralContract,
                machineryContract,
                fertilizerContract,
                recyclablesContract
            });
            var newRecipe = new MockOreFertilizerRecipe();

            processor.SetRecipe(newRecipe);

            Assert.Equal(ContractState.Active, ecContract.State);
            Assert.Equal(ContractState.Disposed, mineralContract.State);
            Assert.Equal(ContractState.Disposed, machineryContract.State);
            Assert.Equal(ContractState.Broken, fertilizerContract.State);
            Assert.Equal(ContractState.Broken, recyclablesContract.State);
        }

        [Theory]
        [InlineData(true, ContractState.Active)]
        [InlineData(false, ContractState.Broken)]
        public void Changing_recipes_should_retain_valid_contracts(bool hasGypsum, ContractState expectedOutputState)
        {
            var endpoint0 = new MockEndpoint();
            var processor = new ProcessorEndpoint();
            var oldRecipe = new MockMineralFertilizerRecipe();
            processor.SetRecipe(oldRecipe);
            var ecContract = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "ElectricCharge",
                Quantity = 100d,
                Rate = ContractRateUnit.PerMinute
            };
            var mineralContract = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "Minerals",
                Quantity = 20d,
                Rate = ContractRateUnit.PerMinute
            };
            var machineryContract = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "Machinery",
                Quantity = 0.02d,
                Rate = ContractRateUnit.PerDay
            };
            var fertilizerContract = new Contract
            {
                Source = processor,
                Destination = endpoint0,
                ResourceName = "Fertilizer",
                Quantity = 5d,
                Rate = ContractRateUnit.PerMinute
            };
            var recyclablesContract = new Contract
            {
                Source = processor,
                Destination = endpoint0,
                ResourceName = "Recyclables",
                Quantity = 0.02d,
                Rate = ContractRateUnit.PerDay
            };
            var gypsumContract = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "Gypsum",
                Quantity = 20d,
                Rate = ContractRateUnit.PerMinute
            };
            processor.Contracts.AddRange(new Contract[]
            {
                ecContract,
                mineralContract,
                machineryContract,
                fertilizerContract,
                recyclablesContract
            });
            if (hasGypsum)
                processor.Contracts.Add(gypsumContract);
            var newRecipe = new MockGypsumFertilizerRecipe();

            processor.SetRecipe(newRecipe);

            Assert.Equal(ContractState.Active, ecContract.State);
            Assert.Equal(ContractState.Disposed, mineralContract.State);
            Assert.Equal(ContractState.Active, machineryContract.State);
            Assert.Equal(expectedOutputState, fertilizerContract.State);
            Assert.Equal(expectedOutputState, recyclablesContract.State);
        }

        [Fact]
        public void Should_not_be_operational_if_recipe_ingredient_contracts_are_missing()
        {
            var endpointA = new MockEndpoint();
            var processor = new ProcessorEndpoint();
            processor.SetRecipe(new MockFuelRecipe());
            var resource = "LiquidFueld";
            var contract = new Contract
            {
                Source = processor,
                Destination = endpointA,
                ResourceName = resource,
                Quantity = 3000d,
                Rate = ContractRateUnit.PerDay
            };
            processor.Contracts.Add(contract);

            var isOperational = processor.IsHonoringContracts(resource);

            Assert.False(isOperational);
        }

        [Fact]
        public void Outgoing_contracts_should_be_broken_if_processor_is_inactive()
        {
            var endpointA = new MockEndpoint();
            var processor = new ProcessorEndpoint();
            processor.SetRecipe(new MockSolarPanelRecipe());
            var contract = new Contract
            {
                Source = processor,
                Destination = endpointA,
                Quantity = 30000d,
                Rate = ContractRateUnit.PerDay
            };
            processor.Contracts.Add(contract);

            processor.ToggleActiveState(false);

            Assert.Equal(ContractState.Broken, contract.State);
        }

        [Fact]
        public void Outgoing_contracts_should_be_restored_if_processor_becomes_active()
        {
            var endpointA = new MockEndpoint();
            var processor = new ProcessorEndpoint();
            processor.SetRecipe(new MockSolarPanelRecipe());
            var contract = new Contract
            {
                Source = processor,
                Destination = endpointA,
                ResourceName = "ElectricCharge",
                Quantity = 30000d,
                Rate = ContractRateUnit.PerDay
            };
            processor.Contracts.Add(contract);

            processor.ToggleActiveState(false);
            processor.ToggleActiveState(true);

            Assert.Equal(ContractState.Active, contract.State);
        }

        [Fact]
        public void Outgoing_contracts_should_only_be_restored_if_input_requirements_are_met()
        {
            var endpoint0 = new MockEndpoint();
            var endpointA = new MockEndpoint();
            var processor = new ProcessorEndpoint();
            processor.SetRecipe(new MockSolarPanelRecipe());
            var inputContractA = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "ElectricCharge",
                Quantity = 100d,
                Rate = ContractRateUnit.PerMinute
            };
            var inputContractB = new Contract
            {
                Source = endpoint0,
                Destination = processor,
                ResourceName = "Ore",
                Quantity = 20d,
                Rate = ContractRateUnit.PerMinute
            };
            var outputContract = new Contract
            {
                Source = processor,
                Destination = endpointA,
                ResourceName = "ElectricCharge",
                Quantity = 30000d,
                Rate = ContractRateUnit.PerDay
            };
            processor.Contracts.Add(outputContract);

            processor.ToggleActiveState(false);
            processor.ToggleActiveState(true);

            Assert.Equal(ContractState.Active, outputContract.State);
        }

        [Fact]
        public void Outgoing_contracts_should_not_be_restored_if_input_requirements_not_met()
        {
            var endpointA = new MockEndpoint();
            var processor = new ProcessorEndpoint();
            processor.SetRecipe(new MockFuelRecipe());
            var contract = new Contract
            {
                Source = processor,
                Destination = endpointA,
                ResourceName = "LiquidFuel",
                Quantity = 3000d,
                Rate = ContractRateUnit.PerDay
            };
            processor.Contracts.Add(contract);

            processor.ToggleActiveState(false);
            processor.ToggleActiveState(true);

            Assert.Equal(ContractState.Broken, contract.State);
        }
    }
}
