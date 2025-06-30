using TMPro;
using UnityEngine;

public class DisplayFinalScore : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI ScoreDisplay;

    // Update is called once per frame
    private bool isPlayerFirstTimeInTrigger = true;
    private float displayScore = 0;
    private void Awake()
    {
        isPlayerFirstTimeInTrigger = true;
    }
    void Update()
    {
        Debug.Log(isPlayerFirstTimeInTrigger);
        if (Physics.CheckSphere(transform.position, 20f, 1 << LayerMask.NameToLayer("Player"))) {
            if (isPlayerFirstTimeInTrigger) {
                displayScore = Mathf.Round(Score.FinalScore);
                ScoreDisplay.text = $"{Grade(displayScore / Score.PlayTime)}\n" +
                $"S/T: {displayScore}/{Mathf.Round(Score.PlayTime)}s\n" +
                $"Coins: {BananaCoin.CollectedCoins}/{BananaCoin.CoinsMaximumInLevel}";
                Score.FinalLevelScore = displayScore;
                isPlayerFirstTimeInTrigger = false;
            }

        }
    }

    private char Grade(float score)
    {
        float maxScore = 9000;
        float maxTime = 150f;
        var factor = maxScore / maxTime;
        if (score >= factor) return 'S';
        if (score >= factor * 0.8f) return 'A';
        if (score >= factor * 0.7f) return 'B';
        if (score >= factor * 0.6f) return 'C';
        if (score >= factor * 0.5f) return 'D';
        return 'F';
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
}
