using UnityEngine;

// The first screen. The logo drops in, the tagline fades up, and "tap to start" breathes
// so it is obvious that the whole screen is a button.
public class TitleScreen : UIScreen
{
    [SerializeField] RectTransform logo;
    [SerializeField] CanvasGroup tagline;
    [SerializeField] CanvasGroup prompt;
    [SerializeField] RectTransform planet;
    [SerializeField] RectTransform comet;
    [SerializeField] CanvasGroup cometGroup;

    Vector2 logoHome;
    Vector2 planetHome;
    Vector2 cometStart;

    protected override void Awake()
    {
        base.Awake();
        logoHome = logo.anchoredPosition;
        planetHome = planet.anchoredPosition;
        cometStart = comet.anchoredPosition;
    }

    public override void OnOpen()
    {
        // the logo falls in and overshoots a little, like it landed
        var logoGroup = UIAnim.GroupOf(logo);
        logoGroup.alpha = 0f;
        logo.anchoredPosition = logoHome + new Vector2(0f, 260f);
        Tween.Move(logo, logoHome, 0.8f, Ease.OutBack);
        Tween.Fade(logoGroup, 1f, 0.4f, Ease.OutQuad);

        tagline.alpha = 0f;
        Tween.Fade(tagline, 1f, 0.6f, Ease.OutQuad, 0.55f);

        // the prompt fades in, then pulses for as long as the screen is open
        prompt.alpha = 0f;
        Tween.Fade(prompt, 1f, 0.5f, Ease.OutQuad, 1.0f, () =>
        {
            Tween.Loop(prompt, "pulse", 0.9f, Ease.InOutSine, k => prompt.alpha = Mathf.Lerp(1f, 0.35f, k));
        });

        // the planet floats up and down, slowly
        Tween.Loop(planet, "float", 2.4f, Ease.InOutSine,
            k => planet.anchoredPosition = planetHome + new Vector2(0f, Mathf.Lerp(-14f, 14f, k)));

        LaunchComet(1.2f);
    }

    public override void OnClose()
    {
        Tween.Cancel(planet, "float");
        Tween.Cancel(prompt, "pulse");
        Tween.Cancel(comet, "fly");
    }

    // a comet crosses the sky now and then
    void LaunchComet(float delay)
    {
        cometGroup.alpha = 0f;
        Vector2 end = cometStart + new Vector2(-1500f, -900f);
        Tween.Value(comet, "fly", 1.6f, Ease.OutQuad, k =>
        {
            comet.anchoredPosition = Vector2.Lerp(cometStart, end, k);
            cometGroup.alpha = Mathf.Sin(k * Mathf.PI);
        }, () => LaunchComet(3.5f), delay);
    }

    public override bool HandleBack()
    {
        Application.Quit();
        return true;
    }
}
