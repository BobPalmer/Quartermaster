using System;
using System.Linq;

namespace WOLF
{
    /// <summary>
    /// Converts input resources to output resources.
    /// </summary>
    public class ProcessorEndpoint : AbstractResourceNetworkEndpoint
    {
        public bool IsActive { get; private set; } = true;
        public IRecipe Recipe { get; private set; }

        public ProcessorEndpoint() : base("Processor")
        {
        }

        public override double Free(string resourceName, ContractRateUnit rate)
        {
            if (!Recipe.OutputIngredients.ContainsKey(resourceName))
            {
                return 0d;
            }

            var ingredient = Recipe.OutputIngredients[resourceName];
            var recipeOutput = ContractRateConversions.Convert(ingredient.Quantity, ingredient.Rate, rate);
            var outgoing = Outgoing(resourceName, rate);

            return Math.Max(0d, recipeOutput - outgoing);
        }

        public override bool IsHonoringContracts(string resourceName)
        {
            if (!ValidateIncoming())
                return false;

            if (!Recipe.OutputIngredients.ContainsKey(resourceName))
                return true;

            var ingredient = Recipe.OutputIngredients[resourceName];
            var recipeOutput = ingredient.Quantity;
            var outgoing = Outgoing(resourceName, ingredient.Rate);

            return recipeOutput >= outgoing;
        }

        public void SetRecipe(IRecipe recipe)
        {
            Recipe = recipe;

            // Break old/invalid contracts
            var inputResources = recipe.InputIngredients.Keys;
            var inputContracts = Contracts.Where(c => c.Destination == this);
            foreach (var contract in inputContracts)
            {
                if (!inputResources.Contains(contract.ResourceName))
                {
                    contract.State = ContractState.Disposed;
                }
            }

            var outputResources = recipe.OutputIngredients.Keys;
            var outputContracts = Contracts.Where(c => c.Source == this);
            foreach (var contract in outputContracts)
            {
                if (!outputResources.Contains(contract.ResourceName))
                {
                    contract.State = ContractState.Broken;
                    continue;
                }

                var ingredient = recipe.OutputIngredients[contract.ResourceName];
                var outputQuantity = ingredient.Quantity;
                var outputRate = ingredient.Rate;
                var contractQuantity = ContractRateConversions.Convert(contract.Quantity, contract.Rate, outputRate);

                if (outputQuantity < contractQuantity)
                {
                    contract.State = ContractState.Broken;
                }
            }

            // Refresh processor and contract states
            ToggleActiveState(IsActive);
        }

        /// <summary>
        /// Toggles the state of the processor. Will attempt to reactivate broken contracts when state is set to Active.
        /// </summary>
        /// <param name="isActive"></param>
        public void ToggleActiveState(bool isActive)
        {
            IsActive = isActive;
            if (!isActive)
            {
                // Break outgoing contracts
                foreach (var contract in Contracts)
                {
                    if (contract.Source == this)
                    {
                        contract.State = ContractState.Broken;
                    }
                }
            }
            else
            {
                // Activate outgoing contracts, if incoming requirements are met
                bool requirementsMet = ValidateIncoming();
                foreach (var contract in Contracts)
                {
                    if (contract.Source == this)
                    {
                        contract.State = requirementsMet ? ContractState.Active : ContractState.Broken;
                    }
                }
            }
        }

        private bool ValidateIncoming()
        {
            foreach (var ingredient in Recipe.InputIngredients)
            {
                var resourceName = ingredient.Key;
                if (!ValidateIncoming(resourceName))
                    return false;
            }

            return true;
        }

        private bool ValidateIncoming(string resourceName)
        {
            var ingredient = Recipe.InputIngredients[resourceName];
            var incomingAmount = Contracts
                .Where(c => c.Destination == this && c.ResourceName == resourceName)
                .Sum(c => ContractRateConversions.Convert(c.Quantity, c.Rate, ingredient.Rate));

            return incomingAmount >= ingredient.Quantity;
        }
    }
}
