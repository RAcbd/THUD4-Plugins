namespace T4.sllh.Raff;

public class ResourceBars : BasePlugin, IGameWorldPainter
{
    public Feature CurrentValue { get; private set; }
    public bool IsResourceEnabled { get; set; } = true;

    public float Scale { get; private set; } = 0.73f;
    public float VerticalOffset { get; private set; } = 61.702f;
    public float HorizontalOffset { get; private set; } = 0f;

    public float HorizontalPadding { get; private set; } = 2f;
    public float VerticalPadding { get; private set; } = 2f;
    public float HpTextVerticalOffset { get; private set; } = 12.128f;

    public float Width { get; private set; } = 271.915f;
    public float Height { get; private set; } = 32.979f;
    public float Divider { get; private set; } = 1.106f;

    public bool ShowHPPercentage { get; private set; } = false;
    public bool ShowResourcePercentage { get; private set; } = false;

    public bool ShowHPText { get; private set; } = true;
    public bool ShowResourceText { get; private set; } = true;

    private readonly Feature ResourceScale;
    private readonly Feature ResourceFeature;
    private readonly Feature ShowText;
    private readonly Feature ShowPercentage;

    public IFillStyle HPColor { get; private set; } = Services.Render.GetFillStyle(200, 205, 15, 15);
    public IFillStyle ResourceColor { get; private set; } = Services.Render.GetFillStyle(200, 15, 196, 205);

    public float FontSize { get; private set; } = 9f;

    private ITexture _hpBackground;
    private ITexture _resourceBackground;

    public float CurrentHP { get; private set; }

    public ResourceBars()
        : base(PluginCategory.Fight, "Displays player's HP and Resource bars")
    {
        ResourceScale = AddFeature(nameof(ResourceScale), "Settings")
            .AddFloatResource(nameof(Scale), "Scale", 0.5f, 2f,
            getter: () => Scale,
            setter: newValue =>
            {
                Scale = newValue;
                this.ReCalc();
            })
            .AddFloatResource(nameof(FontSize), "Font Size", 6, 24,
            getter: () => FontSize,
            setter: newValue =>
            {
                FontSize = newValue;
                this.ReCalc();
            })
            .AddFloatResource(nameof(HpTextVerticalOffset), "HP/Resource Text Veritical Offset", -50, 50,
            getter: () => HpTextVerticalOffset,
            setter: newValue => HpTextVerticalOffset = newValue)
            .AddFloatResource(nameof(VerticalOffset), "Vertical Offset", -500, 500,
            getter: () => VerticalOffset,
            setter: newValue => VerticalOffset = newValue)
            .AddFloatResource(nameof(HorizontalOffset), "Horizontal Offset", -500, 500,
            getter: () => HorizontalOffset,
            setter: newValue => HorizontalOffset = newValue)
            .AddFloatResource(nameof(Width), "Bar Width", 100, 500,
            getter: () => Width,
            setter: newValue =>
            {
                Width = newValue;
                this.ReCalc();
            })
            .AddFloatResource(nameof(Height), "Bar Height", 10, 100,
            getter: () => Height,
            setter: newValue =>
            {
                Height = newValue;
                this.ReCalc();
            })
            .AddFloatResource(nameof(Divider), "Divider Height", 0, 10,
            getter: () => Divider,
            setter: newValue => Divider = newValue)
            .AddFloatResource(nameof(HorizontalPadding), "Horizontal Bar Padding", 0, 5,
            getter: () => HorizontalPadding,
            setter: newValue =>
            {
                HorizontalPadding = newValue;
                this.ReCalc();
            })
            .AddFloatResource(nameof(VerticalPadding), "Vertical Bar Padding", 0, 5,
            getter: () => VerticalPadding,
            setter: newValue =>
            {
                VerticalPadding = newValue;
                this.ReCalc();
            })
            .AddFillStyleResource(nameof(HPColor), HPColor, "HP Color")
            .AddFillStyleResource(nameof(ResourceColor), ResourceColor, "Resource Color");

        ResourceFeature = AddFeature(nameof(ResourceFeature), "show Resource bar")
            .AddBooleanResource(nameof(IsResourceEnabled), "show resource bar",
            getter: () => IsResourceEnabled,
            setter: newValue => IsResourceEnabled = newValue);

        ShowText = AddFeature(nameof(ShowText), "Enable/Disable text")
           .AddBooleanResource(nameof(ShowHPText), "show hp text",
           getter: () => ShowHPText,
           setter: newValue => ShowHPText = newValue)
           .AddBooleanResource(nameof(ShowResourceText), "show resource text",
           getter: () => ShowResourceText,
           setter: newValue => ShowResourceText = newValue);

        ShowPercentage = AddFeature(nameof(ShowPercentage), "Enable/Disable percentage")
           .AddBooleanResource(nameof(ShowHPPercentage), "show hp percentage",
           getter: () => ShowHPPercentage,
           setter: newValue => ShowHPPercentage = newValue)
           .AddBooleanResource(nameof(ShowResourcePercentage), "show resource percentage",
           getter: () => ShowResourcePercentage,
           setter: newValue => ShowResourcePercentage = newValue);

        this.ReCalc();

        this._hpBackground = Services.Render.GetTexture(SupportedTextureId.UIButtonLight_134092211);
        this._resourceBackground = Services.Render.GetTexture(SupportedTextureId.UIButtonLight_134092211);
    }

    private float _halfW;
    private float _halfH;
    private float _scaledH;
    private float _backgroundW;
    private float _backgroundH;
    private float _foreground100Percent;
    private float _foregroundH;
    private void ReCalc()
    {
        this._halfW = (this.Width / 2f) * this.Scale;
        this._halfH = (this.Height / 2f) * this.Scale;
        this._scaledH = this.Height * this.Scale;
        this._backgroundW = this.Width * this.Scale;
        this._backgroundH = this.Height * this.Scale;
        this._foreground100Percent = (this.Width - (2f * this.HorizontalPadding)) * this.Scale;
        this._foregroundH = this._backgroundH - (2f * this.VerticalPadding);
    }

    public void PaintGameWorld(GameWorldLayer layer)
    {
        if (layer != GameWorldLayer.Ground)
        {
            return;
        }

        // Re-calc because of the possible resize.
        var centerX = Services.Game.WindowWidth / 2f;
        var centerY = Services.Game.WindowHeight / 2f;

        var hp = (100f * Services.Game.MyPlayerActor.HitpointsCur) / Services.Game.MyPlayerActor.HitpointsMax;
        this.CurrentHP = hp;

        this._hpBackground.Draw(centerX - this._halfW + this.HorizontalOffset,
            centerY - this._halfH + this.VerticalOffset,
            this._backgroundW,
            this._backgroundH);
        this.HPColor.FillRectangle(centerX - this._halfW + this.HorizontalPadding + this.HorizontalOffset,
            centerY - this._halfH + this.VerticalPadding + this.VerticalOffset,
            ((hp * this._foreground100Percent) / 100f) * (this._backgroundW - 2 * this.HorizontalPadding) / this._foreground100Percent,
            this._foregroundH);

        if (ShowHPText)
        {
            var hpPercentage = (Services.Game.MyPlayerActor.HitpointsCur * 100f) / Services.Game.MyPlayerActor.HitpointsMax;

            var hpValueString = $"{Services.Game.MyPlayerActor.HitpointsCur:F0}/{Services.Game.MyPlayerActor.HitpointsMax:F0}";

            if (ShowHPPercentage)
            {
                hpValueString += $" ({hpPercentage:F0}%)";
            }

            var maxTextWidth = this._foreground100Percent - (2 * this.HorizontalPadding);

            var fontSize = this.FontSize;
            var hpValueText = Services.Render.GetFont(255, 210, 210, 210, fontFamily: "times new roman", bold: false, size: fontSize, shadowMode: FontShadowMode.None);
            var hpValueLayout = hpValueText.GetTextLayout(hpValueString);
            var hpValueWidth = hpValueLayout.Width;
            while (hpValueWidth > maxTextWidth && fontSize > 1)
            {
                fontSize--;
                hpValueText = Services.Render.GetFont(255, 210, 210, 210, fontFamily: "times new roman", bold: false, size: fontSize, shadowMode: FontShadowMode.None);
                hpValueLayout = hpValueText.GetTextLayout(hpValueString);
                hpValueWidth = hpValueLayout.Width;
            }

            var textX = centerX - (hpValueWidth / 2f) + this.HorizontalOffset;
            var textY = centerY - this._halfH + this.VerticalOffset + this.HpTextVerticalOffset - (hpValueLayout.Height / 2f);

            hpValueLayout.DrawText(textX, textY);
        }

        if (this.IsResourceEnabled)
        {
            var resource = (100f * Services.Game.MyPlayerActor.PrimaryResourceCur) /
                           Services.Game.MyPlayerActor.PrimaryResourceMax;

            this._resourceBackground.Draw(centerX - this._halfW + this.HorizontalOffset,
                centerY - this._halfH + this.VerticalOffset + this._scaledH + this.Divider,
                this._backgroundW,
                this._backgroundH);
            this.ResourceColor.FillRectangle(centerX - this._halfW + this.HorizontalPadding + this.HorizontalOffset,
                centerY - this._halfH + this.VerticalOffset + this._scaledH + this.Divider + this.VerticalPadding,
                (resource * this._foreground100Percent) / 100f * (this._backgroundW - 2 * this.HorizontalPadding) / this._foreground100Percent,
                this._foregroundH);


            if (ShowResourceText)
            {
                // Calculate the resource value string
                var resourceValueString = $"{Services.Game.MyPlayerActor.PrimaryResourceCur:F0}/{Services.Game.MyPlayerActor.PrimaryResourceMax:F0}";

                if (ShowResourcePercentage)
                {
                    var resourcePercentage = (Services.Game.MyPlayerActor.PrimaryResourceCur * 100f) / Services.Game.MyPlayerActor.PrimaryResourceMax;
                    resourceValueString += $" ({resourcePercentage:F0}%)";
                }

                var maxResourceTextWidth = this._foreground100Percent - (2 * this.HorizontalPadding);

                var resourceFontSize = this.FontSize;
                var resourceText = Services.Render.GetFont(255, 210, 210, 210, fontFamily: "times new roman", bold: false, size: resourceFontSize, shadowMode: FontShadowMode.None);
                var resourceLayout = resourceText.GetTextLayout(resourceValueString);
                var resourceTextWidth = resourceLayout.Width;

                while (resourceTextWidth > maxResourceTextWidth && resourceFontSize > 1)
                {
                    resourceFontSize--;
                    resourceText = Services.Render.GetFont(255, 210, 210, 210, fontFamily: "times new roman", bold: false, size: resourceFontSize, shadowMode: FontShadowMode.None);
                    resourceLayout = resourceText.GetTextLayout(resourceValueString);
                    resourceTextWidth = resourceLayout.Width;
                }

                var resourceTextX = centerX - (resourceTextWidth / 2f) + this.HorizontalOffset;
                var resourceTextY = centerY - this._halfH + this.VerticalOffset + this._scaledH + this.Divider + this.HpTextVerticalOffset - (resourceLayout.Height / 2f);

                resourceLayout.DrawText(resourceTextX, resourceTextY);
            }
        }
    }
}