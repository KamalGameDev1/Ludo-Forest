using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace LudoMGP
{
    public class MatchMakingScript : MonoBehaviour
    {
        public static MatchMakingScript instance;

        private void Awake()
        {
            instance = this;
        }

        private void OnDisable()
        {
            foreach(var item in matcherTexts)
            {
                item.text="Matching..";
            }
        }

        [Header("House color sprite")]
        public List<Sprite> houseColorMatcherSprite = new List<Sprite>();
        public Color[] playerColors;

        [Header("UI References (size = 4)")]
        public GameObject[] playerMatchers = new GameObject[4];
        public Image[] matcherProfileImages = new Image[4];
        public Image[] matcherFrameImages = new Image[4];
        public Text[] matcherTexts = new Text[4];
        public GameObject[] scrollers = new GameObject[4];

        private int maxPlayerCount;


        public void MatcherCountStarter(int num)
        {
            maxPlayerCount = num;
            if (num == 2)
            {
                playerMatchers[0].SetActive(true);
                playerMatchers[2].SetActive(true);
                scrollers[2].SetActive(true);
                playerMatchers[1].SetActive(false);
                playerMatchers[3].SetActive(false);
            }
            else if (num == 4)
            {
                playerMatchers[0].SetActive(true);

                playerMatchers[1].SetActive(true);
                scrollers[1].SetActive(true);

                playerMatchers[2].SetActive(true);
                scrollers[2].SetActive(true);

                playerMatchers[3].SetActive(true);
                scrollers[3].SetActive(true);
            }
        }

        public void StopScrollMatcher(int seat, string name)
        {
            int index = seat;
            if (index < 0 || index >= 4) return;

            if (index != 0) scrollers[index].SetActive(false); // hide scroller for player 2–4

            
            if (name.Length > 10)
            {
                matcherTexts[index].text = name.Substring(0, 10);
            }
            else
            {
                matcherTexts[index].text = name;
            }

        }

        public void CancelMatchMaking()
        {
            ApiCallingLudo.instance.gamePlaypannel.SetActive(false);
            GameManagerLudo.instance.matchMakingPanel.SetActive(false);
            GameManagerLudo.instance.LudoPanel.transform.localEulerAngles = new Vector3(0, 0, 0);
            GameManagerLudo.instance.ResetAllDetails();
        }

        public void SetMatchMakerProfile()
        {
            int mySeat = GameManagerLudo.instance.playerPosition[0];
            for (int i = 0; i < 4; i++) // 1 to 3 for opponent matchers
            {
                int otherSeat = GameManagerLudo.instance.playerPosition[i];
                matcherFrameImages[i].sprite = houseColorMatcherSprite[otherSeat - 1];
                matcherTexts[i].color = playerColors[otherSeat - 1];
            }
        }

    }
    
}
