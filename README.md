# Own a Dungeon — Unity port

Migrated from the Next.js/TypeScript web game at the root of the source
repo. See the migration report for what was ported and what wasn't.

## Opening this project

1. Install Unity **2022.3 LTS** via Unity Hub (any recent 2022.3.x patch
   should work — `ProjectVersion.txt` pins `2022.3.50f1`, but Hub will
   offer to open it with whatever 2022.3.x you have installed).
2. Add this folder in Unity Hub ("Open" → select this directory).
3. First open will take a while: Unity needs to import every asset (all
   `.cs` scripts + the ~330 sprite files under `Assets/Sprites` and
   `Assets/SourceArt`) and resolve the packages in `Packages/manifest.json`
   (this pulls TextMeshPro and Newtonsoft Json from the registry — an
   internet connection is required for this first import).
4. TextMeshPro will prompt to import its **TMP Essential Resources** the
   first time a TMP component is used — accept that prompt (Window → TextMeshPro →
   Import TMP Essential Resources if it doesn't prompt automatically).
5. Open `Assets/Scenes/Main.unity` and press Play. The entire UI is built
   at runtime by `GameController` (see `Assets/Scripts/UI/GameController.cs`)
   — the scene itself only contains a camera and a light.
6. Before building a player, add `Main.unity` to **File → Build Settings →
   Scenes In Build** (this project ships without a pre-populated build
   list, since that list lives in `ProjectSettings/EditorBuildSettings.asset`,
   which — like every other `ProjectSettings/*.asset` besides
   `ProjectVersion.txt` — Unity generates with safe defaults on first
   open rather than something worth hand-authoring blind).

## This was built without access to the Unity Editor

The environment this migration was done in has no Unity install, no Editor,
and no way to reach the Unity/NuGet package registries to even
type-check the C# in isolation. Every script was written and reviewed by
hand (structure, brace balance, and cross-file references were checked with
text tooling), but nothing here has been pressed through Play mode. Treat
first boot as the real first test pass — see the migration report for the
specific spots most likely to need a tweak.
