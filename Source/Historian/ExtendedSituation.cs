namespace KSEA.Historian
{
    public enum ExtendedSituation
    {
        Default = 0,
        Landed = 1,
        Splashed = 2,
        Prelaunch = 4,
        Flying = 8,
        SubOrbital = 16,
        Orbiting = 32,
        Escaping = 64,
        Docked = 128,
        RagDolled = 256,
        Climbing = 512
    }
}