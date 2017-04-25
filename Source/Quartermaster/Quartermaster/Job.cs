namespace Quartermaster
{
    public class Job
    {
        private Recipe _recipe;
        private double _lastUpdate = -1;

        public double LastUpdate
        {
            get { return _lastUpdate; }
            private set { _lastUpdate = value; }
        }

        public Recipe JobRecipe
        {
            get { return _recipe; }
            set { _recipe = value; }
        }

        public bool HasRecipe()
        {
            return (JobRecipe != null);
        }

        public void SetLastUpdate(double time)
        {
            LastUpdate = time;
        }
    }
}