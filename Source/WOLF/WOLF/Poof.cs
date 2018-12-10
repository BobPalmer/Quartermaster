namespace WOLF
{
    public static class Poof
    {
        public static void GoPoof(Vessel vessel)
        {
            foreach (var part in vessel.parts.ToArray())
            {
                part.Die();
            }
            vessel.Die();
        }
    }
}
