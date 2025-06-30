using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Score : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI ScoreValue;
    [SerializeField] private TextMeshProUGUI SpeedValue;
    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private TextMeshProUGUI CoinsText;
    [SerializeField] private TextMeshProUGUI AirTimeText;
    [SerializeField] private TextMeshProUGUI WallRunText;
    public static float PlayTime;


    private float speed;
    public static float totalDist = 0;
    public static float timeInAir = 0;
    public static float FinalScore = 0;
    public static float FinalLevelScore = 0;
    private void Update()
    {
        PlayTime += Time.deltaTime;
        UpdateTime();
        UpdateSpeed();
        CalculateScore();
        if (Grappling.IsGrappling && !PublicConstants.Pmh.IsGrounded(5f)) {
            var RoundedAirTime = Mathf.Round(timeInAir);
            if (RoundedAirTime < 60) {
                AirTimeText.text = "+Air Time:\n " + RoundedAirTime + "s";
            }
            else
                AirTimeText.text = $"+Air Time:\n {Mathf.Round(RoundedAirTime / 60)}m {RoundedAirTime % 60}s";
        }
        else
            AirTimeText.text = "";

        //if (PublicConstants.Pmh.isWallRunning)
        //    WallRunText.enabled = true;
        //else
        //    WallRunText.enabled = false;

        CoinsText.text = "Coins: " + BananaCoin.CollectedCoins + "/" + BananaCoin.CoinsMaximumInLevel;
    }

    void UpdateTime()
    {
        var RoundedTime = Mathf.Round(Time.time);
        if (RoundedTime < 60) {
            TimeText.text = "Time: " + RoundedTime + "s";
        }
        else
            TimeText.text = $"Time: {Mathf.Round(RoundedTime / 60)}m {RoundedTime % 60}s";
    }

    void UpdateSpeed()
    {
        speed = Mathf.Round(PublicConstants.Prb.linearVelocity.magnitude);
        SpeedValue.text = "+Speed: " + speed + " KM/H";
        if (speed == 0f) SpeedValue.enabled = false;
        else SpeedValue.enabled = true;
    }
    public static float Multiplier = 1f;

    void CalculateScore()
    {

        float currentMultiplier = (PublicConstants.Pmh.MovementMultiplier + 1) * 1.5f;
        if (!PublicConstants.Pmh.IsGrounded(5f)) {
            timeInAir += Time.deltaTime;
            FinalScore += timeInAir * (currentMultiplier / 2) * Time.deltaTime;
        }
        //if (currentMultiplier > Multiplier) {
        //    Multiplier = currentMultiplier;
        //}
        //if (timeInAir != 0) {
        //    FinalScore = (speed * (timeInAir * timeInAir) * Time.fixedDeltaTime * (Multiplier * Multiplier)) / ((PlayTime + 1)); //*(PlayTime / 2)
        //}
        //else
        //    FinalScore = 0;
        FinalScore -= 5 * Time.deltaTime;
        if (FinalScore < 0) FinalScore = 0f;
        Mathf.Clamp(FinalScore, 0, 99999);
        ScoreValue.text = Mathf.Round(FinalScore).ToString();
    }

    //private bool HasScorePassedMilestone(float oldScore, float newScore)
    //{
    //    int fourthDigitNew = ((int)newScore / 1000) % 10;
    //    int fourthDigitOld = ((int)oldScore / 1000) % 10;
    //    return fourthDigitNew > fourthDigitOld;
    //}
    //private IEnumerator ScoreMilestoneEffect()
    //{
    //    var oldFontSize = ScoreValue.fontSize;
    //    var oldColor = ScoreValue.color;
    //    ScoreValue.color = Color.green;
    //    ScoreValue.fontSize += 10;
    //    yield return new WaitForSeconds(0.5f);
    //    Mathf.Lerp(ScoreValue.fontSize, oldFontSize, 0.1f);
    //    Color.Lerp(ScoreValue.color, oldColor, 0.1f);
    //}
}
