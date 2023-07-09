using Content.Server.Procedural;
using Content.Shared.Maps;
using Content.Shared.Procedural;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._FTL.FTLPoints.Effects;

[DataDefinition]
public sealed class SpawnDungeonEffect : FTLPointEffect
{
    [DataField("configPrototypes")]
    public List<string> ConfigPrototypes { get; } = new List<string>()
    {
        "Experiment",
        "LavaBrig",
    };

    [DataField("minSpawn")] public int MinSpawn = 1;
    [DataField("maxSpawn")] public int MaxSpawn = 2;

    public override void Effect(FTLPointEffectArgs args)
    {
        var random = IoCManager.Resolve<IRobustRandom>();
        var amountToSpawn = random.Next(MinSpawn, MaxSpawn);

        for (int i = 0; i < amountToSpawn; i++)
        {
            var dungeon = args.EntityManager.System<DungeonSystem>();
            var prototype = IoCManager.Resolve<IPrototypeManager>();

            var position = new Vector2i(random.Next(-200, 200), random.Next(-200, 200));
            var dungeonUid = args.MapManager.GetMapEntityId(args.MapId);

            if (!args.EntityManager.TryGetComponent<MapGridComponent>(dungeonUid, out var dungeonGrid))
            {
                dungeonUid = args.EntityManager.CreateEntityUninitialized(null, new EntityCoordinates(dungeonUid, position));
                dungeonGrid = args.EntityManager.EnsureComponent<MapGridComponent>(dungeonUid);
                args.EntityManager.InitializeAndStartEntity(dungeonUid, args.MapId);
            }

            var seed = new Random().Next();

            if (!prototype.TryIndex<DungeonConfigPrototype>(random.Pick(ConfigPrototypes), out var dungeonProto))
            {
                return;
            }

            dungeon.GenerateDungeon(dungeonProto, dungeonUid, dungeonGrid, position, seed);

            foreach (var tile in dungeonGrid.GetAllTiles())
            {
                var def = tile.GetContentTileDefinition();
                var newTile = new Tile(tile.Tile.TypeId, tile.Tile.Flags, random.Pick(def.PlacementVariants));
                dungeonGrid.SetTile(tile.GridIndices, newTile);
            }
        }
    }
}
