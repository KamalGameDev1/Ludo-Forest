using LudoMGP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPanelScript : MonoBehaviour
{
    public static WinnerPanelScript instance;
    private void Awake()
    {
        instance = this;
    }
    private void OnDisable()
    {
        WinnerpanelObj.SetActive(false);
    }

    [Header("Winner panel")]
    public GameObject WinnerpanelObj;
    public Text playerNameranktext1;
    public Text playerwonText;
    public Text playerrankText;
    public Transform playerwonTransform;

    public GameObject winnerListPrefab;

    public void ClearWinnerListData()
    {
        for (int i = playerwonTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(playerwonTransform.GetChild(i).gameObject);
        }
    }
    //..............................Final winner response..........................///
    public void FinalWinnerResponse(FinalWinnerResponse finalWinner)
    {
        ClearWinnerListData();
        WinnerpanelObj.SetActive(true);
        foreach (var item in finalWinner.all_player_data)
        {
            GameObject winner = Instantiate(winnerListPrefab, playerwonTransform, false);
            winner.SetActive(true);
            winner.GetComponent<WInLoseITem>().playerNameText.text = item.player_nickname;
            winner.GetComponent<WInLoseITem>().playerScoreText.text = item.points.ToString();
            winner.GetComponent<WInLoseITem>().playerPrizeText.text = item.win_amount.ToString();

            GameObject playerObj = GameManagerLudo.instance.playerScriptList.Find(x => x.playerID == item.player_id)?.gameObject;

            if (playerObj != null)
            {
                LudoPlayerScript playerScript = playerObj.GetComponent<LudoPlayerScript>();

                if (item.isWinner && playerScript.playerID == item.player_id)
                {
                    playerNameranktext1.text = item.player_nickname;
                    playerwonText.text = "Player Won - " + item.win_amount; // formatted amount
                       
                }
                if(GameManagerLudo.playerID == item.player_id)
                {
                    playerrankText.text = item.rank.ToString();
                }
            }

        }
    }
}
