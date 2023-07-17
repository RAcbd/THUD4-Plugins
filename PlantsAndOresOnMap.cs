﻿using System.Numerics;

namespace T4.Plugins.Raff;

public class PlantsAndOresOnMapWithSlider : BasePlugin, IGameWorldPainter
{
    public ITexture Icon { get; } = Services.Render.GetTexture(SupportedTextureId.UIMinimapIcons_109, 255);
    public ITexture GroundBorderTexture { get; } = Services.Render.GetTexture(SupportedTextureId.UIConsoleIcons_119, 160);
    public float IconSize { get; set; } = 4.5f;
    public float GroundIconSize { get; set; } = 8.0f;

    private readonly Dictionary<ActorSnoId, ITexture> _textureMap = new()
    {
        [ActorSnoId.HarvestNode_Herb_Common_Gallowvine] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_004),
        [ActorSnoId.HarvestNode_Herb_Common_Gallowvine_PROLOGUE] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_004),
        [ActorSnoId.HarvestNode_Herb_Frac_Biteberry] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_001),
        [ActorSnoId.HarvestNode_Herb_Frac_Biteberry_PROLOGUE] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_001),
        [ActorSnoId.HarvestNode_Herb_Hawe_Blightshade] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_002),
        [ActorSnoId.HarvestNode_Herb_Kehj_Lifesbane] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_005),
        [ActorSnoId.HarvestNode_Herb_Rare_Angelbreath] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_000),
        [ActorSnoId.HarvestNode_Herb_Rare_FiendRose] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_003),
        [ActorSnoId.HarvestNode_Herb_Scos_HowlerMoss] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_032),
        [ActorSnoId.HarvestNode_Herb_Step_Reddamine] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_006),
        [ActorSnoId.HarvestNode_Ore_Global_Common] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_018),
        [ActorSnoId.HarvestNode_Ore_Global_Common_PROLOGUE] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_018),
        [ActorSnoId.HarvestNode_Ore_Global_Rare] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_023),
        [ActorSnoId.USZ_HarvestNode_Ore_UberSubzone_001_Dyn] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_022),
    };

    private Dictionary<ActorSnoId, bool> ActorHashset;

    public Feature IconOnMap { get; private set; }

    public override string GetDescription() => Services.Translation.Translate(this, "show the nearby herbs/ores on the map");
    public override PluginCategory Category => PluginCategory.Loot;

    public override void Load()
    {
        base.Load();
        ActorHashset = _textureMap.ToDictionary(x => x.Key, x => true);

        IconOnMap = new Feature()
        {
            Plugin = this,
            NameOf = nameof(IconOnMap),
            DisplayName = () => Services.Translation.Translate(this, "icon on map"),
            Resources = new()
            {
                new FloatFeatureResource()
                {
                    NameOf = nameof(IconSize),
                    DisplayText = () => Services.Translation.Translate(this, "icon size"),
                    MinValue = 1.0f,
                    MaxValue = 11.0f,
                    Getter = () => IconSize,
                    Setter = newValue => IconSize = newValue,
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(GroundIconSize),
                    DisplayText = () => Services.Translation.Translate(this, "ground size"),
                    MinValue = 1.0f,
                    MaxValue = 8.0f,
                    Getter = () => GroundIconSize,
                    Setter = newValue => GroundIconSize = newValue,
                }
            },
        };

        foreach (var kvp in _textureMap)
        {
            var actorSnoId = kvp.Key;
            IconOnMap.Resources.Add(new BooleanFeatureResource()
            {
                NameOf = actorSnoId.ToString(),
                DisplayText = () => Services.GameData.GetActorSno(actorSnoId).NameLocalized,
                Getter = () => ActorHashset.TryGetValue(actorSnoId, out var enabled) && enabled,
                Setter = newValue => ActorHashset[actorSnoId] = newValue,
            });
        }

        Services.Customization.RegisterFeature(IconOnMap);
    }

    public void PaintGameWorld(GameWorldLayer layer)
    {
        if (layer == GameWorldLayer.Ground)
        {
            var actors = Services.Game.GizmoActors.Where(x => ActorHashset.TryGetValue(x.ActorSno.SnoId, out var enabled) && enabled);

            var size = Services.Game.WindowHeight / 100f * GroundIconSize;
            foreach (var actor in actors)
            {
                if (!actor.Coordinate.IsOnScreen)
                    continue;

                GroundBorderTexture.Draw(actor.Coordinate.ScreenX - (size * 0.6f), actor.Coordinate.ScreenY - (size * 0.1f), size * 1.2f, size * 1.2f, sharpen: false);
                _textureMap[actor.ActorSno.SnoId].Draw(actor.Coordinate.ScreenX - (size / 2), actor.Coordinate.ScreenY, size, size, sharpen: false);
            }
        }

        if (layer == GameWorldLayer.Map && IconOnMap.Enabled)
        {
            var actors = Services.Game.GizmoActors.Where(x => ActorHashset.TryGetValue(x.ActorSno.SnoId, out var enabled) && enabled);

            var size = Services.Game.WindowHeight / 100f * IconSize;
            foreach (var actor in actors)
            {
                if (!actor.Coordinate.IsOnMap)
                    continue;

                Icon.Draw(actor.Coordinate.MapX - (size / 2), actor.Coordinate.MapY - (size / 2), size, size, sharpen: false);
            }
        }
    }
}