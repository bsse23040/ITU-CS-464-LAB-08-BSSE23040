using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A listener: shows the hearts, the score, the coins, the way to the portal and the boost button.
public class RunHud : MonoBehaviour
{
    [SerializeField] RunController run;
    [SerializeField] ShipHealth health;
    [SerializeField] ShipBoost boost;
    [SerializeField] Image[] hearts;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] GameObject progressBar;
    [SerializeField] RectTransform progressFill;      // grows to the right as the portal gets closer
    [SerializeField] RectTransform portal;
    [SerializeField] Button pauseButton;
    [SerializeField] Button boostButton;
    [SerializeField] Image boostCharge;
    [SerializeField] TMP_Text boostLabel;
    [SerializeField] Color heartFull = Color.red;
    [SerializeField] Color heartEmpty = Color.gray;

    void Awake()
    {
        pauseButton.onClick.AddListener(() => ScreenManager.Instance.Open(ScreenId.Pause));
        boostButton.onClick.AddListener(() => boost.TryActivate());
    }

    void OnEnable()
    {
        run.GameStarted += OnStarted;
        run.ScoreChanged += OnScore;
        run.CoinsChanged += OnCoins;
        run.ProgressChanged += OnProgress;
        run.GameWon += OnOver;
        run.GameLost += OnOver;
        health.Damaged += OnHearts;
        health.Healed += OnHearts;
    }

    void OnDisable()
    {
        run.GameStarted -= OnStarted;
        run.ScoreChanged -= OnScore;
        run.CoinsChanged -= OnCoins;
        run.ProgressChanged -= OnProgress;
        run.GameWon -= OnOver;
        run.GameLost -= OnOver;
        health.Damaged -= OnHearts;
        health.Healed -= OnHearts;
    }

    void OnStarted()
    {
        levelText.text = GameSession.LevelLabel;
        progressBar.SetActive(run.HasPortal);
        pauseButton.interactable = true;
        OnHearts(health.Current, health.Max);
    }

    void OnScore(int score) { scoreText.text = score.ToString("N0"); }

    void OnCoins(int coins)
    {
        coinText.text = coins.ToString();
        if (coins > 0) UIAnim.Punch(coinText.transform, 1.35f);
    }

    void OnProgress(float p)
    {
        progressFill.anchorMax = new Vector2(Mathf.Max(0.02f, p), 1f);
        // the portal itself comes into view over the last stretch
        portal.anchoredPosition = new Vector2(0f, Mathf.Lerp(420f, -430f, Mathf.InverseLerp(0.82f, 1f, p)));
    }

    // full hearts are solid and big, empty ones are dim and smaller, so it reads without colour too
    void OnHearts(int current, int max)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            bool full = i < current;
            hearts[i].color = full ? heartFull : heartEmpty;
            hearts[i].transform.localScale = Vector3.one * (full ? 1f : 0.72f);
        }
    }

    void OnOver(int score)
    {
        pauseButton.interactable = false;
        boostButton.interactable = false;
    }

    void Update()
    {
        if (run.Phase != RunController.State.Running) return;
        boostCharge.fillAmount = boost.Charge;
        boostButton.interactable = boost.Ready;
        boostLabel.text = boost.Active ? "BOOSTING" : boost.Ready ? "BOOST" : "CHARGING";
    }
}
