using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LudoMGP
{
    public class GameManagerLudo : MonoBehaviour
    {
        public static GameManagerLudo instance;

        private void Awake()
        {
            instance = this;
        }
        public static double updated_balance;



        [Header("ScriptableObject")]
        public LudoGameInitiate_SO ludoGameInitiate;
        public LudoGameTableList_SO ludoGameTableList_SO;

        [Header("All : List")]
        public List<LudoPlayerScript> ludoPlayerList = new List<LudoPlayerScript>();


        [Header("Dice Sprite")]
        public Sprite[] diceAnimatingSprite;//Ludo Dice Controller
        public Sprite[] diceMovingSprite;//Ludo Dice Controller

        [Header("Script reference")]
        public LudoSocketManager socketManager;

        public List<LudoPlayerScript> playerScriptList;

        [Header("Panel reference")]
        public GameObject matchMakingPanel;
        public GameObject winnerPanel;

        [Header("House color sprite")]
        public List<Sprite> houseColorPlayerFrameSprite = new List<Sprite>();
        public List<Sprite> DiceFrameSprite = new List<Sprite>();

        

        private void Start()
        {

        }


        #region Static Variable

        public static string room_id;
        public static string room_code;
        public static string ip;
        public static string port;

        public static string session_id;

        public static string game_type;

        public static string game_varient;

        public static string playerID;



        //......................///////////////////////
        public static string BalanceStringAmount(double amount)
        {
            string value = amount.ToString();

            if (amount >= 10000000) value = string.Format("{0:0.00}", (amount / 10000000)) + " Cr";
            else if (amount >= 100000) value = string.Format("{0:0.00}", (amount / 100000)) + " L";
            else value = amount.ToString("F2"); 
            return value;
        }

        #endregion
        public void SetUserDetails()
        {
            room_id = LudoModesTableList.room_id;
            room_code = LudoModesTableList.room_name;
            ip = LudoModesTableList.ip;
            port = LudoModesTableList.port;

            playerID = ludoGameInitiate.AllData.data.userinfo.player_id;
            socketManager.SendUserDetails();
            PawnManager.instance.ResetAll();

        }

        public Text totalWinningAmtText;
        public void RoomInfoResponse(RoomInfoResponse response)
        {
            if (response == null)
            {
                return;
            }
            else
            {
                LudoSocketManager.instance.sessionID = response.session_id;
                totalWinningAmtText.text = response.prize_distribution.first_winner.ToString();
                matchMakingPanel.SetActive(true);

                MatchMakingScript.instance.MatcherCountStarter(response.max_player);
                game_type = response.game_type;

            }
        }

        //////.........................................................................................................///////////////////////
        public void PlayerInfoView(PlayerInfoResponse playerInfo)
        {
            if (playerInfo.player_id == playerID)
            {
                RotateLudoPanel(playerInfo.house_color, playerInfo.player_nickname);
            }

            for (int i = 0; i < playerScriptList.Count; i++)
            {
                playerScriptList[i].PlayerTurnReset();
                for (int j = 0; j < playerHouse.Length; j++)
                {
                    if (playerHouse[j] == playerInfo.house_color)
                    {
                        MatchMakingScript.instance.StopScrollMatcher(j, playerInfo.player_nickname);
                    }
                }
                if (playerScriptList[i].playerColor == playerInfo.house_color)
                {
                    playerScriptList[i].SetPlayerInfoResponse(playerInfo);
                }
            }
        }
        public void PlayerDetailsList(List<PlayerListDetail> playerInfo)
        {
            foreach (var item in playerInfo)
            {
                for (int i = 0; i < playerScriptList.Count; i++)
                {
                    if (playerScriptList[i].playerColor == item.house_color)
                    {
                        playerScriptList[i].playerID = item.player_id;
                        playerScriptList[i].playerNametext.text = item.player_nickname;
                        playerScriptList[i].HouseColor = item.house_color;
                        if (item.players_avtar_url != "")
                        {
                            playerScriptList[i].ImageUrl = item.players_avtar_url;
                        }
                        else
                        {
                            playerScriptList[i].playerProfileImage.sprite = ApiCallingLudo.instance.defaultDP;
                        }
                        playerScriptList[i].PlayingBalance = item.wallet;
                        playerScriptList[i].playerSeatNumber = item.seat_number;
                        playerScriptList[i].operator_player_id = item.operator_player_id;
                        playerScriptList[i].IsPlayerDataSet = true;
                        playerScriptList[i].gameObject.SetActive(true);
                       

                    }
                    else
                    {
                        playerScriptList[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        #region SetLudoBoard and player
        public GameObject LudoPanel;


        int colorIndex;
        public int[] playerPosition;
        public string[] playerHouse;//match making
        public float rotationZ;

        void RotateLudoPanel(string houseColor, string name)
        {
            playerPosition = new int[4];
            playerHouse = new string[4];

            Debug.Log("Rotating Ludo panel");

            string[] allColors = { "red", "green", "yellow", "blue" };
            int colorIndex = Array.IndexOf(allColors, houseColor.ToLower());

            if (colorIndex == -1)
            {
                Debug.LogWarning("Unknown house color: " + houseColor);
                return;
            }

            this.colorIndex = colorIndex;

            rotationZ = colorIndex * 90;

            // Assign player position and house based on rotation
            for (int i = 0; i < 4; i++)
            {
                int index = (colorIndex + i) % 4;

                playerPosition[i] = index + 1;
                playerHouse[i] = allColors[index];

                playerScriptList[i].playerColor = allColors[index];
                playerScriptList[i].playerProfileFrameImage.sprite = houseColorPlayerFrameSprite[index];
                playerScriptList[i].playerDiceFrameImage.sprite = DiceFrameSprite[index];
            }

            //LudoPanel.transform.DORotate(new Vector3(0, 0, rotationZ), 0.1f);
            LudoPanel.transform.eulerAngles = new Vector3(0, 0, rotationZ);

            // Set current player's UI
            var mm = MatchMakingScript.instance;
            mm.matcherFrameImages[0].sprite = mm.houseColorMatcherSprite[colorIndex];
            mm.matcherTexts[0].text = name;
            mm.matcherTexts[0].color = mm.playerColors[colorIndex];

            mm.SetMatchMakerProfile();
        }

        public Transform playertransformForAnimation;

        public void SetPlayerMovingAnimation()
        {
            foreach (var player in playerScriptList)
            {
                if (player.IsPlayerDataSet)
                {
                    StartCoroutine(player.SetAnimation());
                }
            }

        }
        #endregion


        //////...................................................................................

        //////.........................................................................................................///////////////////////

        [Space]
        [Header("objects")]
        public GameObject extraMoveobj;
        public GameObject[] countdownObjs;
        //timerSprites60

        public IEnumerator PrepTimer(PrepTimerResponse prepTimer)
        {
            ResetAllDetails();


            yield return new WaitForSeconds(1f);
            LudoSocketManager.instance.PlayerListRequest();
            matchMakingPanel.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            SetPlayerMovingAnimation();

            int t = prepTimer.preparation_time;


            for (int i = t - 1; i >= 0; i--)
            {
                countdownObjs[i].SetActive(true);
                countdownObjs[i].transform.DOScale(1.1f, 0.5f);
                yield return new WaitForSeconds(0.5f);
                countdownObjs[i].transform.DOScale(0.5f, 0.5f);
                yield return new WaitForSeconds(0.5f);
                countdownObjs[i].SetActive(false);

            }

        }
        //////.........................................................................................................///////////////////////
        public void GameStarted(GameStartedResponse gameStarted)
        {
            if (matchMakingPanel.gameObject.activeSelf)
            {
                matchMakingPanel.SetActive(false);
            }
            session_id = gameStarted.session_id;
            game_type = gameStarted.game_type;

            game_varient = gameStarted.game_varient;
            Debug.Log("GAmeTupe:" + game_type);
        }

        //..............................Player turn..........................///
        public void PlayerTurn(PlayerTurnResponse turnResponse)
        {
            foreach (var player in ludoPlayerList)
            {
                player.PlayerTurnReset();
                if (turnResponse.player_id == player.playerID)
                {
                    player.PlayerTurn(turnResponse);
                }
            }
        }
        //..............................Player turn Missed..........................///
        public void PlayerTurnMissed(TurnMissedResponse turnResponse)
        {
            foreach (var player in ludoPlayerList)
            {
                if (turnResponse.player_id == player.playerID)
                {
                    player.PlayerTurnMissed(turnResponse);
                }
            }
        }
        //..............................DiceTruned turn..........................///
        public void DiceResultTurn(DiceResultResponse diceResponse)
        {
            foreach (var player in ludoPlayerList)
            {
                if (diceResponse.player_id == player.playerID)
                {
                    player.DiceResult(diceResponse);
                }
            }
        }

       
        //..............................Game play Timer response..........................///
        public IEnumerator ExtraMove()
        {
            extraMoveobj.transform.DOScale(0, 0.1f);
            extraMoveobj.SetActive(true);
            extraMoveobj.transform.DOScale(1.5f, 0.5f);
            yield return new WaitForSeconds(1f);
            extraMoveobj.transform.DOScale(0, 0.5f);
            yield return new WaitForSeconds(.5f);
            extraMoveobj.SetActive(false);

        }
        //..............................Game play Timer response..........................///
        public GameObject TimerObject;

        public IEnumerator GamePlayTimerResponse(GamePlayTimeRespose timer)
        {
            TimerObject.SetActive(true);

            Text timerText = TimerObject.GetComponentInChildren<Text>();
            int totalTime = timer.remaining_time;
            int timeLeft = totalTime;

            while (timeLeft >= 0)
            {
                int minutes = timeLeft / 60;
                int seconds = timeLeft % 60;
                timerText.text = $"{minutes:D2}:{seconds:D2}";

                yield return new WaitForSeconds(1f);
                timeLeft--;
            }

            TimerObject.SetActive(false);
        }

        [Header("Exit panel")]
        public GameObject Exitpanel;
        public Text exitTitleText;
        public Text MessageText;

        public void OpenExitPanel()
        {
            Exitpanel.SetActive(true);
        }
        private void OnDisable()
        {
            ResetAllDetails();
        }
        public void ResetAllDetails()
        {
            StopAllCoroutines();
            TimerObject.SetActive(false);
            WinnerPanelScript.instance.WinnerpanelObj?.SetActive(false);
            foreach (var item in playerScriptList)
            {
                item.AllDataReset();
            }
            foreach (var item in countdownObjs)
            {
                item.SetActive(false);
            }
        }

    }
}