namespace OwnADungeon.Animation
{
    // Direct port of src/animation/laneLayout.ts — percentage positions of
    // the hero/monster tokens along the sidescrolling raid stage's floor.
    public static class LaneLayout
    {
        public const float EntranceX = 8f;   // hero's starting position, left edge
        public const float EncounterX = 50f; // where hero + monster meet and fight, center
        public const float ExitX = 92f;      // hero's exit position / monster's entry position, right edge
        public const float FloorY = 78f;     // fixed vertical "ground line" (% of room-floor height)
    }
}
