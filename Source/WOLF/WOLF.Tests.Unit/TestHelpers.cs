namespace Quartermaster.Tests.Unit
{
    public static class TestHelpers
    {
        public static IGameData Game => _game ?? (_game = new FakeGameInterface());
        private static FakeGameInterface _game;

        public static Recipe GetSampleRecipeA()
        {
            return new Recipe();
        }
        public static Recipe GetSampleRecipeB()
        {
            var r = new Recipe();
            var ore = new Ingredient("Ore", 100);
            var lfo = new Ingredient("LFO", 100);
            var mac = new Ingredient("Machinery", 100);
            r.AddInput(ore);
            r.AddInput(lfo);
            r.AddInput(mac);
            return r;
        }

        public static ProcessWorker GetProcessor()
        {
            return new ProcessWorker(Game, FakeNetwork.Instance, "VESSEL","PROCESSOR");
        }

        public static void LoadNetwork()
        { 
            FakeNetwork.Instance.Repo.ResetCache();
            FakeNetwork.Instance.Repo.SaveLink(new ResourceLink());
        }
    }
}