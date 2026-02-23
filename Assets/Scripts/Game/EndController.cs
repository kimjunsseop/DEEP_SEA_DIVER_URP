using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndController : MonoBehaviour
{

    public TextMeshProUGUI win;
    public TextMeshProUGUI die;
    void Start()
    {
        if(SceneController.instance.result == true)
        {
            pritWin();
        }
        else
        {
            printDie();
        }
    }
    public bool reward = false;
    public void Lobby()
    {
        if(AdsManager.Instance.IsRewardEarned == false)
        {
            AdsManager.Instance.ShowRewardedAd();
        }
        else
        {
            // 로비로 가기 전 보상 상태 리셋 (필요시)
            AdsManager.Instance.IsRewardEarned = false; 
            SceneManager.LoadScene(0);
        }
    }
    public void RE()
    {
        if(AdsManager.Instance.IsRewardEarned == false)
        {
            AdsManager.Instance.ShowRewardedAd();
        }
        else
        {
            // 로비로 가기 전 보상 상태 리셋 (필요시)
            AdsManager.Instance.IsRewardEarned = false; 
            SceneManager.LoadScene(1);
        }
    }
    public void pritWin()
    {
        win.gameObject.SetActive(true);
        die.gameObject.SetActive(false);
    }
    public void printDie()
    {
        die.gameObject.SetActive(true);
        win.gameObject.SetActive(false);
    }
}
