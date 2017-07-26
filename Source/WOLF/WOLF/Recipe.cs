using System.Collections.Generic;

namespace Quartermaster
{
    public class Recipe
    {
        private List<Ingredient> _inputs;
        private List<Ingredient> _outputs;
        private List<Ingredient> _requirements;

        public List<Ingredient> Inputs
        {
            get
            {
                if (_inputs == null)
                    SetupInputs();
                return _inputs; }
        }

        public List<Ingredient> Outputs
        {
            get
            {
                if (_outputs == null)
                    SetupOutputs();
                return _outputs;
            }
        }

        public List<Ingredient> Requirements
        {
            get
            {
                if (_requirements == null)
                    SetupRequirements();
                return _requirements;
            }
        }

        private void SetupInputs()
        {
            _inputs = new List<Ingredient>();
        }
        private void SetupOutputs()
        {
            _outputs = new List<Ingredient>();
        }
        private void SetupRequirements()
        {
            _requirements = new List<Ingredient>();
        }

        public void AddInput(Ingredient i)
        {
            Inputs.Add(i);
        }
        public void AddOutput(Ingredient i)
        {
            Outputs.Add(i);
        }
        public void AddRequirement(Ingredient i)
        {
            Requirements.Add(i);
        }
    }
}