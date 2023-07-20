namespace T4.Plugins.DAV.Raff;

public class InventoryMaterials : BasePlugin, IGameUserInterfacePainter
{
    public Feature TableSetting { get; private set; }

    public float SizeMultiplier { get; set; } = 0.50f;
    public IFont TextFont { get; set; } = Services.Render.GetFont(255, 255, 220, 220, fontFamily: "verdana", size: 7, shadowMode: FontShadowMode.Heavy);
    public float VerticalOffset { get; set; } = -7.3f;
    public float HorizontalOffset { get; set; } = 0.040f;

    public ITexture Separator { get; } = Services.Render.GetTexture(SupportedTextureId.UICurrency06_996654672, 255);
    public IUIControl GamblingCurrencyValueTextControl { get; } = Services.UserInterface.RegisterControl("inventory_dialog_mainPage.InventoryContainer.Inventory_Currency_Gambling.Currencies_dynamictext");

    private ITexture bgIcon { get; set; } = Services.Render.GetTexture(SupportedTextureId.UIButtonLight_1449803639);
    private List<MaterialData> matList { get; set; } = new List<MaterialData>();

    public void PaintGameUserInterface(GameUserInterfaceLayer layer)
    {
        if (layer != GameUserInterfaceLayer.OverPanels)
            return;
        if (!Services.UserInterface.InventoryAreaControl.Visible)
            return;

        var rect = Services.UserInterface.GetInventorySlotControl(0, 0, 2);
        var w = rect.Width * SizeMultiplier;
        var h = rect.Height * SizeMultiplier;

        var inventoryLeft = Services.UserInterface.InventoryAreaControl.Left;
        var inventoryTop = Services.UserInterface.InventoryAreaControl.Top;
        var inventoryWidth = Services.UserInterface.InventoryAreaControl.Width;
        var inventoryHeight = Services.UserInterface.InventoryAreaControl.Height;

        var x = inventoryLeft + w + HorizontalOffset * inventoryWidth;
        var y = inventoryTop + inventoryHeight - h * (0.8f + VerticalOffset);

        foreach (var mat in matList)
        {
            mat.DrawCount(bgIcon, TextFont, x, y, w, h);
            x += w + w * 1.5f;
        }
    }

    public class MaterialData
    {
        public string Name { get; private set; }
        public CurrencyType Currency { get; private set; }
        public ITexture Icon { get; private set; }

        private int xref { get; set; }
        private int yref { get; set; }

        public MaterialData(CurrencyType currency, ItemSnoId snoId, uint iconId, int classType, int classRank)
        {
            var material = Services.GameData.GetItemSno(snoId);
            Name = material.NameLocalized;
            Currency = currency;
            Icon = Services.Render.GetTexture(iconId);
            xref = classRank;
            yref = classType;
        }

        public void DrawCount(ITexture bgTexture, IFont font, float x, float y, float w, float h)
        {
            x -= w * xref;
            y -= h * yref;
            bgTexture.Draw(x, y, w, h);
            Icon.Draw(x, y, w, h);

            if (Services.Game.MyPlayer.Currencies.TryGetValue(Currency, out var count))
            {
                string countText;
                if (count >= 10000)
                    countText = (count / 1000) + "k";
                else if (count >= 1000)
                    countText = (count / 1000f).ToString("0.0") + "k";
                else
                    countText = count.ToString("#,##0");

                var tl = font.GetTextLayout(countText);
                tl.DrawText(x + w - tl.Width, y + h - tl.Height);
            }

            if (Services.Game.CursorInsideRect(x, y, w, h))
                Services.Hint.SetHint(Name);
        }

    }

    public override string GetDescription() => Services.Translation.Translate(this, "display material summary at inventory");

    public override void Load()
    {
        base.Load();

        matList.Add(new MaterialData(CurrencyType.SacredLegendarySalvage, ItemSnoId.CraftingMaterial_Salvage_Sacred_Legendary, SupportedTextureId.InventoryMaterials_3500380629, 5, 1));
        matList.Add(new MaterialData(CurrencyType.RareSalvage, ItemSnoId.CraftingMaterial_Salvage_Rare, SupportedTextureId.InventoryMaterials_3796089172, 5, 2));
        matList.Add(new MaterialData(CurrencyType.LegendaryArmorSalvage, ItemSnoId.CraftingMaterial_Salvage_Legendary_Armor, SupportedTextureId.InventoryMaterials_4037344621, 5, 3));
        matList.Add(new MaterialData(CurrencyType.LegendaryJewelrySalvage, ItemSnoId.CraftingMaterial_Salvage_Legendary_Jewelry, SupportedTextureId.InventoryMaterials_2686035881, 5, 4));
        matList.Add(new MaterialData(CurrencyType.SigilSalvage, ItemSnoId.CraftingMaterial_Salvage_Nightmare_Sigil_Powder, SupportedTextureId.InventoryQuestNature_1427923640, 5, 5));
        matList.Add(new MaterialData(CurrencyType.LegendaryWeaponSalvage, ItemSnoId.CraftingMaterial_Salvage_Legendary_Weapon, SupportedTextureId.InventoryMaterials_329641371, 5, 6));
        matList.Add(new MaterialData(CurrencyType.CommonLeather, ItemSnoId.CraftingMaterial_Leather_Common, SupportedTextureId.InventoryMaterials_1079277694, 5, 7));
        matList.Add(new MaterialData(CurrencyType.RareLeather, ItemSnoId.CraftingMaterial_Leather_Rare, SupportedTextureId.InventoryMaterials_1293541336, 5, 8));
        matList.Add(new MaterialData(CurrencyType.DemonHeart, ItemSnoId.CraftingMaterial_MonsterDrop_Demon_Heart, SupportedTextureId.InventoryMaterials_2490254900, 5, 9));
        matList.Add(new MaterialData(CurrencyType.HumanTongue, ItemSnoId.CraftingMaterial_MonsterDrop_Human_Tongue, SupportedTextureId.InventoryMaterials_4187456782, 5, 10));
        matList.Add(new MaterialData(CurrencyType.UndeadDust, ItemSnoId.CraftingMaterial_MonsterDrop_Undead_Dust, SupportedTextureId.InventoryMaterials_3926628815, 5, 11));
        matList.Add(new MaterialData(CurrencyType.WildlifeBones, ItemSnoId.CraftingMaterial_MonsterDrop_Wildlife_Bones, SupportedTextureId.InventoryMaterials_500621934, 5, 12));
        matList.Add(new MaterialData(CurrencyType.CommonOre, ItemSnoId.CraftingMaterial_Ore_Common, SupportedTextureId.InventoryMaterials_1744130859, 5, 13));
        matList.Add(new MaterialData(CurrencyType.RareOre, ItemSnoId.CraftingMaterial_Ore_Rare, SupportedTextureId.InventoryMaterials_2822254805, 5, 14));
        matList.Add(new MaterialData(CurrencyType.RareScatteredPrism, ItemSnoId.CraftingMaterial_Rare_ScatteredPrism, SupportedTextureId.InventoryMaterials_1769453156, 5, 15));

        TableSetting = new Feature()
        {
            Plugin = this,
            NameOf = nameof(TableSetting),
            DisplayName = () => Services.Translation.Translate(this, "item count on tabs"),
            Resources = new() {
                new FontFeatureResource() {
                    NameOf = nameof(TextFont),
                    DisplayText = () => Services.Translation.Translate(this, "font"),
                    Font = TextFont,
                },
                new FloatFeatureResource() {
                    NameOf = nameof(SizeMultiplier),
                    DisplayText = () => Services.Translation.Translate(this, "size"),
                    MinValue = 0.5f,
                    MaxValue = 1f,
                    Getter = () => SizeMultiplier,
                    Setter = newValue => SizeMultiplier = MathF.Round(newValue, 1),
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(VerticalOffset),
                    DisplayText = () => Services.Translation.Translate(this, "vertical offset"),
                    MinValue = -100f,
                    MaxValue = 100f,
                    Getter = () => VerticalOffset,
                    Setter = newValue => VerticalOffset = newValue,
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(HorizontalOffset),
                    DisplayText = () => Services.Translation.Translate(this, "horizontal offset"),
                    MinValue = -5,
                    MaxValue = 5,
                    Getter = () => HorizontalOffset,
                    Setter = newValue => HorizontalOffset = newValue,
                }
            },
        };

        Services.Customization.RegisterFeature(TableSetting);
    }
}