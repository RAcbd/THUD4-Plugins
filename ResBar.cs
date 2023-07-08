namespace T4.sllh.Raff;

public class ResourceBars : BasePlugin, IGameWorldPainter
{
    public Feature CurrentValue { get; private set; }
    public bool IsResourceEnabled { get; private set; } = true;
    public float Scale { get; private set; } = 1f;
    public float VerticalOffset { get; private set; } = 0f;
    public float HorizontalOffset { get; private set; } = 0f;

    public float Width { get; private set; } = 300f;
    public float Height { get; private set; } = 20f;
    public float Divider { get; private set; } = 2f;

    public float HorizontalPadding { get; private set; } = 2f;
    public float VerticalPadding { get; private set; } = 2f;

    public IFillStyle HPColor { get; private set; } = Services.Render.GetFillStyle(200, 205, 15, 15);
    public IFillStyle ResourceColor { get; private set; } = Services.Render.GetFillStyle(200, 15, 196, 205);

    public IFont CurrentFont = Services.Render.GetFont(255, 210, 210, 210, fontFamily: "times new roman", bold: false, size: 11, shadowMode: FontShadowMode.None);
    public float FontSize { get; private set; } = 11f;
    public float HpTextVerticalOffset { get; private set; } = 0f;

    public bool ShowHPPercentage { get; private set; } = true;
    public bool ShowResourcePercentage { get; private set; } = true;
    public bool ShowHPText { get; private set; } = true;
    public bool ShowResourceText { get; private set; } = true;

    private ITexture _hpBackground;
    private ITexture _resourceBackground;

    public float CurrentHP { get; private set; }

    public override string GetDescription() =>
        Services.Translation.Translate(this, "Displays player's HP and Resource bars");

    public override void Load()
    {
        base.Load();
        CurrentValue = new Feature()
        {
            Plugin = this,
            NameOf = nameof(CurrentValue),
            DisplayName = () => Services.Translation.Translate(this, "Config"),
            Resources = new()
            {
                new BooleanFeatureResource()
                {
                    NameOf = nameof(IsResourceEnabled),
                    DisplayText = () => Services.Translation.Translate(this, "Show resource bar"),
                    Getter = () => IsResourceEnabled,
                    Setter = newValue => IsResourceEnabled = newValue,
                },
                new BooleanFeatureResource()
                {
                    NameOf = nameof(ShowHPText),
                    DisplayText = () => Services.Translation.Translate(this, "Show HP text"),
                    Getter = () => ShowHPText,
                    Setter = newValue => ShowHPText = newValue,
                },
                new BooleanFeatureResource()
                {
                    NameOf = nameof(ShowResourceText),
                    DisplayText = () => Services.Translation.Translate(this, "Show resource text"),
                    Getter = () => ShowResourceText,
                    Setter = newValue => ShowResourceText = newValue,
                },
                new BooleanFeatureResource()
                {
                    NameOf = nameof(ShowHPPercentage),
                    DisplayText = () => Services.Translation.Translate(this, "Show HP percentage"),
                    Getter = () => ShowHPPercentage,
                    Setter = newValue => ShowHPPercentage = newValue,
                },
                new BooleanFeatureResource()
                {
                    NameOf = nameof(ShowResourcePercentage),
                    DisplayText = () => Services.Translation.Translate(this, "Show resource percentage"),
                    Getter = () => ShowResourcePercentage,
                    Setter = newValue => ShowResourcePercentage = newValue,
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(Scale),
                    DisplayText = () => Services.Translation.Translate(this, "Scale"),
                    MinValue = 0.5f,
                    MaxValue = 2f,
                    Getter = () => Scale,
                    Setter = newValue =>
                    {
                        Scale = newValue;
                        this.ReCalc();
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(FontSize),
                    DisplayText = () => Services.Translation.Translate(this, "Font Size"),
                    MinValue = 6,
                    MaxValue = 24,
                    Getter = () => FontSize,
                    Setter = newValue =>
                    {
                        FontSize = newValue;
                        this.ReCalc();
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(HpTextVerticalOffset),
                    DisplayText = () => Services.Translation.Translate(this, "HP/Resource Text Vertical offset"),
                    MinValue = -50,
                    MaxValue = 50,
                    Getter = () => HpTextVerticalOffset,
                    Setter = newValue =>
                    {
                        HpTextVerticalOffset = newValue;
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(VerticalOffset),
                    DisplayText = () => Services.Translation.Translate(this, "Vertical offset"),
                    MinValue = -500,
                    MaxValue = 500,
                    Getter = () => VerticalOffset,
                    Setter = newValue =>
                    {
                        VerticalOffset = newValue;
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(HorizontalOffset),
                    DisplayText = () => Services.Translation.Translate(this, "Horizontal offset"),
                    MinValue = -500,
                    MaxValue = 500,
                    Getter = () => HorizontalOffset,
                    Setter = newValue =>
                    {
                        HorizontalOffset = newValue;
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(Width),
                    DisplayText = () => Services.Translation.Translate(this, "Bar width"),
                    MinValue = 100,
                    MaxValue = 500,
                    Getter = () => Width,
                    Setter = newValue =>
                    {
                        Width = newValue;
                        this.ReCalc();
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(Height),
                    DisplayText = () => Services.Translation.Translate(this, "Bar height"),
                    MinValue = 10,
                    MaxValue = 100,
                    Getter = () => Height,
                    Setter = newValue =>
                    {
                        Height = newValue;
                        this.ReCalc();
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(Divider),
                    DisplayText = () => Services.Translation.Translate(this, "Divider height"),
                    MinValue = 0,
                    MaxValue = 10,
                    Getter = () => Divider,
                    Setter = newValue =>
                    {
                        Divider = newValue;
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(HorizontalPadding),
                    DisplayText = () => Services.Translation.Translate(this, "Horizontal bar padding"),
                    MinValue = 0,
                    MaxValue = 5,
                    Getter = () => HorizontalPadding,
                    Setter = newValue =>
                    {
                        HorizontalPadding = newValue;
                        this.ReCalc();
                    },
                },
                new FloatFeatureResource()
                {
                    NameOf = nameof(VerticalPadding),
                    DisplayText = () => Services.Translation.Translate(this, "Vertical bar padding"),
                    MinValue = 0,
                    MaxValue = 5,
                    Getter = () => VerticalPadding,
                    Setter = newValue =>
                    {
                        VerticalPadding = newValue;
                        this.ReCalc();
                    },
                },
                new FillStyleFeatureResource()
                {
                    NameOf = nameof(HPColor),
                    DisplayText = () => Services.Translation.Translate(this, "HP color"),
                    FillStyle = this.HPColor,
                },
                new FillStyleFeatureResource()
                {
                    NameOf = nameof(ResourceColor),
                    DisplayText = () => Services.Translation.Translate(this, "Resource color"),
                    FillStyle = this.ResourceColor,
                },
            },
        };

        Services.Customization.RegisterFeature(CurrentValue);
        this.ReCalc();

        this._hpBackground = Services.Render.GetTexture(SupportedTextureId.UIButtonLight_005);
        this._resourceBackground = Services.Render.GetTexture(SupportedTextureId.UIButtonLight_005);
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