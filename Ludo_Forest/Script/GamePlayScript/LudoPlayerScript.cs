using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LudoMGP
{
    public class LudoPlayerScript : MonoBehaviour
    {
        public string playerID;
        public string playerColor;
        public int playerSeatNumber;
        public string HouseColor;
        public string room_id;
        public string player_Status;
        public string operator_player_id;
        public string playerScore;

        public Text playerNametext;
        public Text playerLeaveMessagetext;

        public Image playerProfileFrameImage;
        public Image playerDiceFrameImage;
        public Image playerProfileImage;

        public Image profileTimerImage;

        public GameObject dotframeObj;
        public GameObject diceSlider;

        #region GetterSetter
        string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set
            {
                playerName = value;
                if (value.Length > 10)
                {
                    playerNametext.text = value.Substring(0, 10);
                }
                else
                {
                    playerNametext.text = value;
                }
            }
        }

        double playingbalance;
        public double PlayingBalance
        {
            get { return playingbalance; }
            set
            {
                playingbalance = value;
                //walletBalance.text = value >= 1000 ? GameManagerScript_Poker.BalanceStringAmount(value) : value.ToString();
            }
        }

        string imgUrl;
        public string ImageUrl
        {
            get { return imgUrl; }
            set
            {
                imgUrl = value;
                //Debug.Log(imgUrl);
                if (imgUrl != null)
                {
                    StartCoroutine(DownloadProfilePic(imgUrl));
                }
                else
                {
                    playerProfileImage.sprite= ApiCallingLudo.instance.defaultDP;
                }
            }
        }

        Sprite realSprite;
        IEnumerator DownloadProfilePic(string url)
        {
            UnityWebRequest imgload = UnityWebRequestTexture.GetTexture(url);
            yield return imgload.SendWebRequest();
            if (imgload.isNetworkError)
            {
                Debug.Log("Error in Download Image.");
                playerProfileImage.sprite = ApiCallingLudo.instance.defaultDP;

            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(imgload);
                realSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                playerProfileImage.sprite = realSprite;
            }

        }
        #endregion

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        ////.....................///////////////Dice Result //////........................../////
        public void RotateDice()
        {
            Debug.Log("dice click");
            if (playerID == GameManagerLudo.playerID && _playerTurn)
            {
                LudoSocketManager.instance.DiceRolled();
                _playerTurn = false;
            }
        }

        int dicenumber;
        bool key;

        public void DiceResult(DiceResultResponse diceResult)
        {
            if (diceResult == null) return;

            diceSlider.GetComponent<LudoDiceController>().DiceRolling(diceResult, HouseColor);

            // Highlight current player's turn


            // Get the house and eligible pawns
            var houseData = PawnManager.instance.GetHouseData(diceResult.color);
            if (houseData == null) return;

            var eligible = diceResult.eligible_pawns;
            dicenumber = diceResult.diceNumber;
            key = diceResult.key;
            if (diceResult.diceNumber == 6)
            {
                if (eligible.Count == 1)
                {
                    // Only one pawn can be moved -> Auto move
                    MovePawnAuto(diceResult.color, eligible);
                }
                else
                {
                    // Multiple eligible pawns -> Let player choose
                    EnablePawnSelection(diceResult.color, eligible);
                }
            }
            else
            {
                // If not 6, and pawn is already out, handle accordingly
                if (eligible.Count == 1)
                {
                    MovePawnAuto(diceResult.color, eligible);
                }
                else if (eligible.Count > 1)
                {
                    EnablePawnSelection(diceResult.color, eligible);
                }
                else
                {
                    Debug.Log("No valid pawn to move");
                }
            }
        }


        private void MovePawnAuto(string color, List<EligiblePawn> eligible_pawns)
        {
            if (eligible_pawns == null || eligible_pawns.Count == 0)
            {
                Debug.LogWarning("MovePawnAuto failed: No eligible pawns.");
                return;
            }
        }


        private void EnablePawnSelection(string color, List<EligiblePawn> eligible)
        {
            if (eligible == null || eligible.Count == 0)
            {
                Debug.LogWarning("EnablePawnSelection failed: No eligible pawns.");
                return;
            }

            int diceNumber = dicenumber;
            bool isSix = diceNumber == 6;

            // Optional: Sort pawns or validate logic here
            PawnManager.instance.HandleEligiblePawns(eligible, color, diceNumber, isSix);
        }


        ////...................../////////////////////........................../////
        ////...................../////////////////////........................../////
        public bool IsPlayerDataSet;
        public void SetPlayerInfoResponse(PlayerInfoResponse playerInfo)
        {
            betTimeCountertext.gameObject.SetActive(false);
            playerID = playerInfo.player_id;
            PlayerName = playerInfo.player_nickname;

            if (playerInfo.players_avtar_url != "")
            {
                ImageUrl = playerInfo.players_avtar_url;
            }
            else
            {
                playerProfileImage.sprite = ApiCallingLudo.instance.defaultDP;
            }
            PlayingBalance = playerInfo.wallet_balance;
            playerSeatNumber = playerInfo.seat_number;
            room_id = playerInfo.room_id;
            HouseColor = playerInfo.house_color;
            player_Status = playerInfo.player_status;

            //if (playerID == GameManagerLudo.playerID)
            //{
            //    this.gameObject.SetActive(true);
            //}
            //PawnManager.instance.SetLayout(HouseColor);
            if (playerInfo.active)
            {
                IsPlayerDataSet = true;
            }
            else
            {
               
            }

        }

        ////...................../////////////////////........................../////

        ////...................../////////////////////........................../////

        #region TimerCounting

        public void PlayerTurn(PlayerTurnResponse playerTurn)
        {
            PawnManager.instance.ChanceHighLighter(HouseColor);           
            Debug.Log("player Turn:" + HouseColor);
            StartCoroutine(BetTimer(playerTurn.turn_details.total_turn_time, playerTurn.turn_details.remaining_time));
            StartCoroutine(BetTimeCounter(playerTurn.turn_details.remaining_time));
            diceSlider.SetActive(true);
          
        }


        public void SetPlayerScore()
        {
            PawnManager.instance.ChanceHighLighter(HouseColor);
        }

     
        public bool _playerdiceRolled = false;
        public bool _playerTurn = false;
        public IEnumerator BetTimer(float totalTime, float remainingTime)
        {
            _playerTurn = true;

            while (0 <= remainingTime-1 && remainingTime-1 <= totalTime)
            {
                if (_playerdiceRolled)
                {
                    //diceSlider.SetActive(false);
                    break;
                }
                if (profileTimerImage.fillAmount == 0.0)
                {
                    //SoundManager.VibrateDevice?.Invoke();
                }

                remainingTime -= Time.deltaTime;
                //			UIManager.shownTimer += Time.deltaTime;
                float lerpValue = (totalTime - remainingTime) / (totalTime);

                profileTimerImage.fillAmount = Mathf.Lerp(0, 1, lerpValue);


                if (profileTimerImage.fillAmount >= 0.6f)
                {
                    if (lerpValue == 0.6)
                    {
                        //SoundManager.VibrateDevice?.Invoke();

                    }


                }
                if (profileTimerImage.fillAmount == 1)
                {
                    SoundManager.VibrateDevice?.Invoke();
                }

                yield return null;
            }
            _playerTurn = false;
            diceSlider.SetActive(false);
            profileTimerImage.fillAmount = 0;
            _playerdiceRolled = false;
        }

        public Text betTimeCountertext;
        public IEnumerator BetTimeCounter(float remainingTime)
        {
            betTimeCountertext.gameObject.SetActive(true);
            while (remainingTime-1 >= 0)
            {
                if (_playerdiceRolled)
                {
                    break;
                }
                betTimeCountertext.text = (remainingTime-1).ToString();
                yield return new WaitForSeconds(1f);
                remainingTime--;
            }
            betTimeCountertext.text = "";
            betTimeCountertext.gameObject.SetActive(false);
        }
        #endregion

        ////................................../// SetPlayerAnimation..................................//////////////////.................
        public GameObject playerAnimObj;

        public IEnumerator SetAnimation()
        {
            GameObject player = Instantiate(this.gameObject, Vector2.zero, Quaternion.identity);
            player.SetActive(true);
            player.transform.SetParent(transform.parent, false); // Use parent of original, not the object itself
            player.transform.localPosition = GameManagerLudo.instance.playertransformForAnimation.position;

            this.gameObject.SetActive(false);

            yield return player.transform.DOLocalMove(transform.localPosition, 0.5f).WaitForCompletion();

            Destroy(player);
            this.gameObject.SetActive(true);

            SpawnPawns();
        }
        ////................................../// Creating pawn..................................//////////////////.................
        public void SpawnPawns()
        {
            StartCoroutine(PawnManager.instance.SpawnPawn(HouseColor, playerID));
        }

        public GameObject missedChanceObj;
        public void PlayerTurnMissed(TurnMissedResponse playerTurnMissed)
        {
            for (int i = 0; i < playerTurnMissed.missed_turns; i++)
            {
                dotframeObj.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }

       


        public void PlayerTurnReset()
        {
            StopAllCoroutines();
            betTimeCountertext.text = "";
            profileTimerImage.fillAmount = 0;
            diceSlider.SetActive(false);
            _playerTurn = false;
            playerLeaveMessagetext.text = "";
        }

        public void AllDataReset()
        {
            this.gameObject.SetActive(false);
            StopAllCoroutines();
            betTimeCountertext.text = "";
            profileTimerImage.fillAmount = 0;
            diceSlider.SetActive(false);
            _playerTurn = false;
            playerLeaveMessagetext.text = "";
            dotframeObj.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            dotframeObj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            dotframeObj.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
            HouseColor = "";
            playerColor = "";

        }

    }
}