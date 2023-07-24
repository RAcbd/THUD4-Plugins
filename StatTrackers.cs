namespace T4.Plugins.Raff;

public class StatTrackers : BasePlugin, IGameUserInterfacePainter
{
    public ComponentCollection<LabelComponent, IStatTracker> Components { get; } = new();

    public override string GetDescription() => Services.Translation.Translate(this, "show farming statistics at the top left corner");

    public override void Load()
    {
        base.Load();

        var w = 0.080f;
        var h = 0.020f;

        var sessionTracker = Services.Statistics.AllStatTrackers.FirstOrDefault(x => x.Id == "session");
        if (sessionTracker != null)
        {
            // Calculate the X position increment based on label width
            var xOffset = Services.Game.WindowWidth * w;

            CreateLabelForTotalTimeTracker(sessionTracker,
                () => new System.Drawing.RectangleF(Services.Game.WindowWidth - (Services.Game.WindowWidth * w * 3), Services.Game.WindowHeight - (Services.Game.WindowHeight * h * 2), Services.Game.WindowWidth * w, Services.Game.WindowHeight * h));

            CreateLabelForPlayTimeTracker(sessionTracker,
                () => new System.Drawing.RectangleF(Services.Game.WindowWidth - (Services.Game.WindowWidth * w * 2), Services.Game.WindowHeight - (Services.Game.WindowHeight * h * 2), Services.Game.WindowWidth * w, Services.Game.WindowHeight * h));

            CreateLabelForTownTimeTracker(sessionTracker,
                () => new System.Drawing.RectangleF(Services.Game.WindowWidth - (Services.Game.WindowWidth * w), Services.Game.WindowHeight - (Services.Game.WindowHeight * h * 2), Services.Game.WindowWidth * w, Services.Game.WindowHeight * h));

            CreateLabelForTracker(sessionTracker,
                () => new System.Drawing.RectangleF(Services.Game.WindowWidth - (Services.Game.WindowWidth * w * 3), Services.Game.WindowHeight - (Services.Game.WindowHeight * h), Services.Game.WindowWidth * w, Services.Game.WindowHeight * h));

            CreateLabelForGoldTracker(sessionTracker,
                () => new System.Drawing.RectangleF(Services.Game.WindowWidth - (Services.Game.WindowWidth * w * 2), Services.Game.WindowHeight - (Services.Game.WindowHeight * h), Services.Game.WindowWidth * w, Services.Game.WindowHeight * h));

            CreateLabelForTitle(sessionTracker,
                () => new System.Drawing.RectangleF(Services.Game.WindowWidth - (Services.Game.WindowWidth * w), Services.Game.WindowHeight - (Services.Game.WindowHeight * h), Services.Game.WindowWidth * w, Services.Game.WindowHeight * h));
        }
    }

    private LabelComponent CreateLabelForTitle(IStatTracker expTracker, Func<System.Drawing.RectangleF> placementCalculator)
    {
        var titleComponent = new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = placementCalculator,
            },
            NormalOpacity = 1.0f,
            HighlightTestFunc = titleComponent => titleComponent.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => $"{expTracker.Title}",
            HintFunc = () => "Session",
        };

        Components.Add(expTracker, titleComponent);

        return titleComponent;
    }

    private LabelComponent CreateLabelForTracker(IStatTracker expTracker, Func<System.Drawing.RectangleF> placementCalculator)
    {
        var expComponent = new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = placementCalculator,
            },
            NormalOpacity = 1.0f,
            HighlightTestFunc = expComponent => expComponent.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => $"{ValueFormatter.ValueToString(expTracker.GainedExperiencePerHour, ValueFormat.ShortNumber)}",
            HintFunc = () => "Exp / hr",
        };

        Components.Add(expTracker, expComponent);

        return expComponent;
    }

    private LabelComponent CreateLabelForGoldTracker(IStatTracker goldTracker, Func<System.Drawing.RectangleF> placementCalculator)
    {
        var goldComponent = new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = placementCalculator,
            },
            NormalOpacity = 1.0f,
            HighlightTestFunc = goldComponent => goldComponent.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => $"{ValueFormatter.ValueToString(goldTracker.GainedGoldPerHour, ValueFormat.ShortNumber)}",
            HintFunc = () => "Gold / hr",
        };

        Components.Add(goldTracker, goldComponent);

        return goldComponent;
    }

    private LabelComponent CreateLabelForTotalTimeTracker(IStatTracker totalTimeTracker, Func<System.Drawing.RectangleF> placementCalculator)
    {
        var totalTimeComponent = new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = placementCalculator,
            },
            NormalOpacity = 1.0f,
            HighlightTestFunc = totalTimeComponent => totalTimeComponent.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => $"{ValueFormatter.ValueToString(totalTimeTracker.ElapsedMilliseconds, ValueFormat.Time)}",
            HintFunc = () => "Total Timer",
        };

        Components.Add(totalTimeTracker, totalTimeComponent);

        return totalTimeComponent;
    }

    private LabelComponent CreateLabelForPlayTimeTracker(IStatTracker totalPlayTimeTracker, Func<System.Drawing.RectangleF> placementCalculator)
    {
        var totalPlayTimeComponent = new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = placementCalculator,
            },
            NormalOpacity = 1.0f,
            HighlightTestFunc = totalPlayTimeComponent => totalPlayTimeComponent.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => $"{ValueFormatter.ValueToString(totalPlayTimeTracker.PlayElapsedMilliseconds, ValueFormat.Time)}",
            HintFunc = () => "Total Playing Time",
        };

        Components.Add(totalPlayTimeTracker, totalPlayTimeComponent);

        return totalPlayTimeComponent;
    }

    private LabelComponent CreateLabelForTownTimeTracker(IStatTracker totalTownTimeTracker, Func<System.Drawing.RectangleF> placementCalculator)
    {
        var totalTownTimeComponent = new LabelComponent()
        {
            Placement = new DynamicComponentPlacement()
            {
                Calculator = placementCalculator,
            },
            NormalOpacity = 1.0f,
            HighlightTestFunc = totalTownTimeComponent => totalTownTimeComponent.HitTest(Services.Game.CursorX, Services.Game.CursorY),
            TextFunc = () => $"{ValueFormatter.ValueToString(totalTownTimeTracker.TownElapsedMilliseconds, ValueFormat.Time)}",
            HintFunc = () => "Total Town Time",
        };

        Components.Add(totalTownTimeTracker, totalTownTimeComponent);

        return totalTownTimeComponent;
    }

    public void PaintGameUserInterface(GameUserInterfaceLayer layer)
    {
        if (layer != GameUserInterfaceLayer.OverActionBar)
            return;

        if (Services.UserInterface.InventoryControl.Visible)
            return;

        Components.PaintGameUserInterface();
    }
}