namespace T4.Plugins.Raff;

public class PlayerStats : BasePlugin, IGameUserInterfacePainter
{
    public ComponentCollection<LabelComponent, object> Components { get; } = new();
    public override PluginCategory Category => PluginCategory.Fight;

    public override string GetDescription() => Services.Translation.Translate(this, "show key player parameters above the ");

    public IFont Font = Services.Render.GetFont(192, 255, 255, 255, size: 6.5f);

    public override void Load()
    {
        base.Load();

        Components.Add(null, new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = () => GetRectangle(0),
            },
            Font = Font,
            ExpandedHintFont = Font,
            NormalOpacity = 1.0f,
            HighlightTestFunc = component => component.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => ValueFormatter.ValueToString(Services.Game.MyPlayerActor.SheetDPS, ValueFormat.ShortNumber),
            HintFunc = () => Services.Translation.Translate(this, "Sheet DPS"),
            ExpandedHintWidthMultiplier = 5.0f,
        });

        Components.Add(null, new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = () => GetRectangle(1),
            },
            Font = Font,
            ExpandedHintFont = Font,
            NormalOpacity = 1.0f,
            HighlightTestFunc = component => component.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => ValueFormatter.ValueToString(Services.Game.MyPlayerActor.CritChance, ValueFormat.ShortNumber) + "%",
            HintFunc = () => Services.Translation.Translate(this, "Critical Strike Chance"),
            ExpandedHintWidthMultiplier = 5.0f,
        });

        Components.Add(null, new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = () => GetRectangle(2),
            },
            Font = Font,
            ExpandedHintFont = Font,
            NormalOpacity = 1.0f,
            HighlightTestFunc = component => component.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => ValueFormatter.ValueToString(Services.Game.MyPlayerActor.CritDamage, ValueFormat.ShortNumber) + "%",
            HintFunc = () => Services.Translation.Translate(this, "Critical Strike Damage"),
            ExpandedHintWidthMultiplier = 5.0f,
        });

        Components.Add(null, new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = () => GetRectangle(3),
            },
            Font = Font,
            ExpandedHintFont = Font,
            NormalOpacity = 1.0f,
            HighlightTestFunc = component => component.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => ValueFormatter.ValueToString(Services.Game.MyPlayerActor.AttackSpeed, ValueFormat.ShortNumber),
            HintFunc = () => Services.Translation.Translate(this, "Attack Speed"),
            ExpandedHintWidthMultiplier = 5.0f,
        });

        Components.Add(null, new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = () => GetRectangle(4),
            },
            Font = Font,
            ExpandedHintFont = Font,
            NormalOpacity = 1.0f,
            HighlightTestFunc = component => component.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => ValueFormatter.ValueToString(Services.Game.MyPlayerActor.AttackPower, ValueFormat.ShortNumber),
            HintFunc = () => Services.Translation.Translate(this, "Attack Power"),
            ExpandedHintWidthMultiplier = 5.0f,
        });

        Components.Add(null, new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = () => GetRectangle(5),
            },
            Font = Font,
            ExpandedHintFont = Font,
            NormalOpacity = 1.0f,
            HighlightTestFunc = component => component.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () =>
            {
                var skillDamageBonus = Services.Game.MyPlayerActor.SkillDamageBonus;
                return ValueFormatter.ValueToString(skillDamageBonus, ValueFormat.ShortNumber) + "%";
            },
            HintFunc = () => Services.Translation.Translate(this, "Increases Skill Damage"),
            ExpandedHintWidthMultiplier = 5.0f,
        });

        Components.Add(null, new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = () => GetRectangle(6),
            },
            Font = Font,
            ExpandedHintFont = Font,
            NormalOpacity = 1.0f,
            HighlightTestFunc = component => component.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => ValueFormatter.ValueToString(Services.Game.MyPlayerActor.WeaponDamage, ValueFormat.ShortNumber),
            HintFunc = () => Services.Translation.Translate(this, "Weapon Damage"),
            ExpandedHintWidthMultiplier = 5.0f,
        });
    }

    private System.Drawing.RectangleF GetRectangle(int index)
    {
        var control = Services.UserInterface.SkillButtonControls[2];
        var x = control.Left + (control.Height * 0.40f);
        var w = control.Height * 0.85f;
        var h = control.Height * 0.36f;
        var y = control.Top - (control.Height * 0.53f) - h;
        return new System.Drawing.RectangleF(x + (w * index), y, w, h);
    }

    public void PaintGameUserInterface(GameUserInterfaceLayer layer)
    {
        if (layer != GameUserInterfaceLayer.OverActionBar)
            return;

        /*if (Services.UserInterface.InventoryControl.Visible)
            return;*/

        Components.PaintGameUserInterface();
    }
}
