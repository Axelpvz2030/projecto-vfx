using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [Header("Player Elements")]
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public Image playerHealthFill;
    public Image healCooldownOverlay;
    public Image dashCooldownOverlay;
    public Image[] healChargePips;

    [Header("Boss Elements")]
    public BossHealth bossHealth;
    public Image bossHealthFill;

    private void Update()
    {
        UpdatePlayerHUD();
        UpdateBossHUD();
    }

    private void UpdatePlayerHUD()
    {
        if (playerHealth != null)
        {
            if (playerHealthFill != null)
            {
                playerHealthFill.fillAmount = playerHealth.GetHealthPercentage();
            }

            if (healCooldownOverlay != null)
            {
                healCooldownOverlay.fillAmount = playerHealth.GetHealCooldownPercentage();
            }

            int currentCharges = playerHealth.GetHealsRemaining();
            for (int i = 0; i < healChargePips.Length; i++)
            {
                if (healChargePips[i] != null)
                {
                    healChargePips[i].enabled = (i < currentCharges);
                }
            }
        }

        if (playerMovement != null && dashCooldownOverlay != null)
        {
            dashCooldownOverlay.fillAmount = playerMovement.GetDashCooldownPercentage();
        }
    }

    private void UpdateBossHUD()
    {
        if (bossHealth != null && bossHealthFill != null)
        {
            bossHealthFill.fillAmount = bossHealth.GetHealthPercentage();
        }
    }
}