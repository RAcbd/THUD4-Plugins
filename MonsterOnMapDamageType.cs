namespace T4.Plugins.Raff;

public class MonsterOnMapDamageType : BasePlugin, IGameWorldPainter
{
    public IFillStyle RegularFill { get; } = Services.Render.GetFillStyle(160, 255, 64, 64);
    public IFillStyle EliteFill { get; } = Services.Render.GetFillStyle(255, 64, 64, 255);
    public float RegularSize { get; set; } = 0.300f;
    public float EliteSize { get; set; } = 0.600f;
    public float FontSize { get; set; } = 6f;

    public Feature RegularMonsterOnMap { get; private set; }
    public Feature EliteMonsterOnMap { get; private set; }
    public bool ShowDamageTypeText { get; set; } = true;

    private Dictionary<MonsterAffixSnoId, ITexture> EliteAffixTextures { get; } = new();

    private Dictionary<DamageType, System.Drawing.Color> damageTypeColors = new Dictionary<DamageType, System.Drawing.Color>()
    {
        { DamageType.Cold, System.Drawing.ColorTranslator.FromHtml("#0047AB") }, // Blue
        { DamageType.Fire, System.Drawing.ColorTranslator.FromHtml("#D22B2B") }, // Red
        { DamageType.Lightning, System.Drawing.ColorTranslator.FromHtml("#FFD700") }, // Yellow
        { DamageType.Physical, System.Drawing.ColorTranslator.FromHtml("#C0C0C0") }, // Gray
        { DamageType.Poison, System.Drawing.ColorTranslator.FromHtml("#228B22") }, // Green
        { DamageType.Shadow, System.Drawing.ColorTranslator.FromHtml("#7F00FF") }, // Purple
        { DamageType.None, System.Drawing.ColorTranslator.FromHtml("#FAF9F6") }, // White
    };

    private Dictionary<DamageType, IFont> damageTypeFonts = new Dictionary<DamageType, IFont>();

    public override string GetDescription() => Services.Translation.Translate(this, "show the nearby monsters on the map");

    public override void Load()
    {
        base.Load();

        RegularMonsterOnMap = new Feature()
        {
            Plugin = this,
            NameOf = nameof(RegularMonsterOnMap),
            DisplayName = () => Services.Translation.Translate(this, "regular monster on map"),
            Resources = new()
            {
                new BooleanFeatureResource()
                {
                    NameOf = nameof(ShowDamageTypeText),
                    DisplayText = () => Services.Translation.Translate(this, "show damage type text"),
                    Getter = () => ShowDamageTypeText,
                    Setter = newValue => ShowDamageTypeText = newValue,
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(RegularSize),
                    DisplayText = () => Services.Translation.Translate(this, "size"),
                    MinValue = 0.1f,
                    MaxValue = 1.1f,
                    Getter = () => RegularSize,
                    Setter = newValue => RegularSize = newValue,
                },
            },
        };

        EliteMonsterOnMap = new Feature()
        {
            Plugin = this,
            NameOf = nameof(EliteMonsterOnMap),
            DisplayName = () => Services.Translation.Translate(this, "elite monster on map"),
            Resources = new()
            {
                new FillStyleFeatureResource()
                {
                    NameOf = nameof(EliteFill),
                    DisplayText = () => Services.Translation.Translate(this, "fill style"),
                    FillStyle = EliteFill,
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(EliteSize),
                    DisplayText = () => Services.Translation.Translate(this, "size"),
                    MinValue = 0.1f,
                    MaxValue = 1.1f,
                    Getter = () => EliteSize,
                    Setter = newValue => EliteSize = newValue,
                },
            },
        };

        Services.Customization.RegisterFeature(RegularMonsterOnMap);
        Services.Customization.RegisterFeature(EliteMonsterOnMap);

        InitializeDamageTypeFonts();
    }

    private void InitializeDamageTypeFonts()
    {
        foreach (var kvp in damageTypeColors)
        {
            var damageType = kvp.Key;
            var color = kvp.Value;
            var font = Services.Render.GetFont(color.A, color.R, color.G, color.B, fontFamily: "verdana", bold: false, italic: false, size: FontSize, shadowMode: FontShadowMode.None);

            damageTypeFonts[damageType] = font;
        }
    }

    public void PaintGameWorld(GameWorldLayer layer)
    {
        if (layer != GameWorldLayer.Map)
            return;

        if (!RegularMonsterOnMap.Enabled && !EliteMonsterOnMap.Enabled)
            return;

        var hostileMonsters = Services.Game.Monsters.Where(x => !x.Untargetable && (x.Team == 19 || x.Team == 20)
            && x.World.WorldSno == Services.Map.MapWorldSno
                                                                                    /*&& (x.MonsterData.ArcheType == null
                                                                                        || (x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.ambient
                                                                                            && x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.Mount
                                                                                            && x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.NPC
                                                                                            && x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.NPC_Prologue
                                                                                            && x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.Assassin_Skills
                                                                                            && x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.Barbarian_Skills
                                                                                            && x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.Druid_Skills
                                                                                            && x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.Necromancer_Skills
                                                                                            && x.MonsterData.ArcheType.MonsterFamilySnoId != MonsterFamilySnoId.Sorcerer_Skills
                                                                                            ))*/);

        if (RegularMonsterOnMap.Enabled)
        {
            var size = Services.Game.WindowHeight / 100f * RegularSize;
            foreach (var monster in hostileMonsters)
            {
                if (!monster.Coordinate.IsOnMap)
                    continue;

                System.Drawing.Color fillColor;
                if (damageTypeColors.TryGetValue(monster.MonsterData.BaseDamageType, out fillColor))
                {
                    var fillStyle = Services.Render.GetFillStyle(fillColor.A, fillColor.R, fillColor.G, fillColor.B);

                    fillStyle.FillEllipse(monster.Coordinate.MapX - (size / 2), monster.Coordinate.MapY - (size / 2), size, size, sharpen: false);
                }

                if (ShowDamageTypeText)
                {
                    var fontSize = this.FontSize;
                    var label = monster.MonsterData.BaseDamageType.ToString();

                    System.Drawing.Color textColor;
                    if (damageTypeColors.TryGetValue(monster.MonsterData.BaseDamageType, out textColor))
                    {
                        var labelValueText = damageTypeFonts[monster.MonsterData.BaseDamageType];
                        var labelLayout = labelValueText.GetTextLayout(label);
                        var labelWidth = labelLayout.Width;
                        var labelX = monster.Coordinate.MapX - (labelWidth / 2);
                        var labelY = monster.Coordinate.MapY - size + 5;

                        labelLayout.DrawText(labelX, labelY);
                    }
                }
            }
        }

        if (EliteMonsterOnMap.Enabled)
        {
            foreach (var monster in hostileMonsters)
            {
                if (!monster.Coordinate.IsOnMap)
                    continue;

                if (monster.Rarity == MonsterRarity.Elite1 || monster.Rarity == MonsterRarity.Elite2 || monster.Rarity == MonsterRarity.Elite5)
                {
                    if (monster.Affixes?.Count > 0)
                    {
                        var circleSize = Services.Game.WindowHeight / 100f * EliteSize * 1.8f;
                        EliteFill.FillEllipse(monster.Coordinate.MapX, monster.Coordinate.MapY, circleSize, circleSize, sharpen: false);
                        circleSize = Services.Game.WindowHeight / 100f * EliteSize * 3f;
                        var w = monster.Affixes.Count * circleSize;
                        for (var i = 0; i < monster.Affixes.Count; i++)
                        {
                            var affix = monster.Affixes[i];
                            if (!EliteAffixTextures.TryGetValue(affix.SnoId, out var texture))
                            {
                                texture = Services.Render.GetTexture(affix.IconTexture, 255);
                                EliteAffixTextures[affix.SnoId] = texture;
                            }

                            texture.Draw(monster.Coordinate.MapX - (w / 2) + (i * circleSize), monster.Coordinate.MapY - (circleSize / 2), circleSize, circleSize, sharpen: false);
                        }
                    }
                    else
                    {
                        var circleSize = Services.Game.WindowHeight / 100f * EliteSize;
                        EliteFill.FillEllipse(monster.Coordinate.MapX, monster.Coordinate.MapY, circleSize, circleSize, sharpen: false);
                    }
                }
            }
        }
    }
}
