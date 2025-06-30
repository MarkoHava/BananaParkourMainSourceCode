
using UnityEngine;

public class StatsHandler : MonoBehaviour {

    [SerializeField] private float _hp;
    [SerializeField] private float _stamina;
    [SerializeField] private float _staminaRegen;

    public float StaminRegenRate { get { return _staminaRegen; } set { _staminaRegen = value; } }
    public float Health { get { return _hp; } set { _hp = value; } }
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public void TakeDamage(float damage)
    {
        _hp -= damage;
        UpdateAllStats(_hp, _stamina);
    }
    /// <summary>
    /// returns true if the player has enough stamina to cast the spell, false if not
    /// </summary>
    /// <param name="cost">cost of the spell</param>
    /// <returns></returns>
    public bool StaminaCost(float cost)
    {
        if (cost > Stamina) return false;
        Stamina -= cost;
        UpdateAllStats(Health, Stamina);
        return true;
    }
    private void Start()
    {
        UpdateAllStats(_hp, Stamina);
    }
    private void Update()
    {
        if (Stamina < 100) {
            Stamina += StaminRegenRate * Time.deltaTime;
            UpdateAllStats(_hp, Stamina);
        }
        if (_hp <= 0) {
            Time.timeScale = 0f;
        }
    }
    private void UpdateAllStats(float health, float stamina)
    {
        float hpValue;
        float manaValue;

        if (health < 0) hpValue = 0;
        else hpValue = health;
        if (stamina < 0) manaValue = 0;
        else manaValue = stamina;

        //PublicConstants.Singleton.R_HpSlider.value = hpValue / 100;
        //PublicConstants.Singleton.R_HpText.text = Mathf.Round(hpValue).ToString();
        //PublicConstants.Singleton.R_StaminaSlider.value = manaValue / 100;
        //PublicConstants.Singleton.R_StaminaText.text = Mathf.Round(manaValue).ToString();
    }

}
