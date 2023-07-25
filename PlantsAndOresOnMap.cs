using System.Numerics;

namespace T4.Plugins.Raff
{
    public class PlantsAndOresOnMapWithSlider : BasePlugin, IGameWorldPainter
    {
        public ITexture Icon { get; } = Services.Render.GetTexture(SupportedTextureId.UIMinimapIcons_3008900674, 255);
        public ITexture GroundBorderTexture { get; } = Services.Render.GetTexture(SupportedTextureId.UIConsoleIcons_380863977, 160);

        public float IconSize { get; set; } = 4.5f;
        public float GroundIconSize { get; set; } = 8.0f;

        public bool IsGroundIconSizeEnabled { get; set; } = true;

        private readonly Dictionary<ActorSnoId, ITexture> _textureMap = new()
        {
            [ActorSnoId.HarvestNode_Herb_Common_Gallowvine] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_2828026198),
            [ActorSnoId.HarvestNode_Herb_Common_Gallowvine_PROLOGUE] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_2828026198),
            [ActorSnoId.HarvestNode_Herb_Frac_Biteberry] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_596336454),
            [ActorSnoId.HarvestNode_Herb_Frac_Biteberry_PROLOGUE] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_596336454),
            [ActorSnoId.HarvestNode_Herb_Hawe_Blightshade] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_1295260733),
            [ActorSnoId.HarvestNode_Herb_Kehj_Lifesbane] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_3260105159),
            [ActorSnoId.HarvestNode_Herb_Rare_Angelbreath] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_3636713627),
            [ActorSnoId.HarvestNode_Herb_Rare_FiendRose] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_789225021),
            [ActorSnoId.HarvestNode_Herb_Scos_HowlerMoss] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_3935934865),
            [ActorSnoId.HarvestNode_Herb_Step_Reddamine] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_702554951),
            [ActorSnoId.HarvestNode_Ore_Global_Common] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_1744130859),
            [ActorSnoId.HarvestNode_Ore_Global_Common_PROLOGUE] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_1744130859),
            [ActorSnoId.HarvestNode_Ore_Global_Rare] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_2822254805),
            [ActorSnoId.USZ_HarvestNode_Ore_UberSubzone_001_Dyn] = Services.Render.GetTexture(SupportedTextureId.InventoryMaterials_1769453156),
        };

        private Dictionary<ActorSnoId, bool> ActorHashset;

        public Feature IconOnMap { get; private set; }

        public PlantsAndOresOnMapWithSlider()
            : base(PluginCategory.Loot, "show the nearby herbs/ores on the map")
        {
            ActorHashset = _textureMap.ToDictionary(x => x.Key, x => true);

            IconOnMap = new Feature()
            {
                Plugin = this,
                NameOf = nameof(IconOnMap),
                DisplayName = () => "icon on map",
                Resources = new()
                {
                    new FloatFeatureResource()
                    {
                        NameOf = nameof(IconSize),
                        DisplayText = "icon size",
                        MinValue = 1.0f,
                        MaxValue = 11.0f,
                        Getter = () => IconSize,
                        Setter = newValue => IconSize = newValue,
                    },
                    new BooleanFeatureResource()
                    {
                        NameOf = nameof(IsGroundIconSizeEnabled),
                        DisplayText = "ground size enabled",
                        Getter = () => IsGroundIconSizeEnabled,
                        Setter = newValue => IsGroundIconSizeEnabled = newValue,
                    },
                    new FloatFeatureResource()
                    {
                        NameOf = nameof(GroundIconSize),
                        DisplayText = "ground size",
                        MinValue = 1.0f,
                        MaxValue = 8.0f,
                        Getter = () => GroundIconSize,
                        Setter = newValue => GroundIconSize = newValue,
                    },
                },
            };

            foreach (var kvp in _textureMap)
            {
                var actorSnoId = kvp.Key;
                IconOnMap.Resources.Add(new BooleanFeatureResource()
                {
                    NameOf = actorSnoId.ToString(),
                    DisplayText = Services.GameData.GetActorSno(actorSnoId).NameLocalized,
                    Getter = () => ActorHashset.TryGetValue(actorSnoId, out var enabled) && enabled,
                    Setter = newValue => ActorHashset[actorSnoId] = newValue,
                });
            }
        }

        public void PaintGameWorld(GameWorldLayer layer)
        {
            if (layer == GameWorldLayer.Ground && IsGroundIconSizeEnabled)
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
}
