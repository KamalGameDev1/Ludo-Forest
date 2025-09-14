using BestHTTP.JSON.LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LudoMGP
{
    public class LudoModesTableList : MonoBehaviour
    {
        public static LudoModesTableList instance;

        [Header("ScriptableObject")]
        public LudoGameInitiate_SO ludoGameInitiate;
        public LudoGameTableList_SO ludoGameTableList_SO;


        [Header("LobbyItem")]
        [Space]
        public Sprite H1, H4;//1House//4House.
        public Sprite playerCount2, playerCount3, playerCount4;

        public GameObject bottomGameobj;
        public double PlayerBalance
        {
            get
            {
                return ludoGameInitiate.AllData.data.userinfo.wallet;
            }
            set
            {
                ludoGameInitiate.AllData.data.userinfo.wallet = value;
                playerBalanceText.text=GameManagerLudo.BalanceStringAmount(value);

                PlayerPrefs.SetString(PlayerPrefsKeys.WALLETBALANCE, PlayerBalance.ToString());
                Debug.Log("Player Balance is updated");
            }
        }

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {

        }
        public void CalledTableList()
        {
            SetPlayerDetails();
            GameManagerLudo.playerID = ludoGameInitiate.AllData.data.userinfo.player_id;
            LudoSocketManager.instance.Connect(ludoGameTableList_SO.AllData.socket_config.ip, ludoGameTableList_SO.AllData.socket_config.port);
            //LudoSocketManager.instance.Connect("192.168.0.101", "4103");
            bottomGameobj.SetActive(true);
            ClassicTables();
            TimerTables();

        }
        #region Challanges
        [Header("Challanges")]
        public Transform contentPosChallanges;
        public GameObject offlineChallange;
        public Text challengeElementCountText;


        public void ClosedChallenge(string roomID)
        {
            if (challengeGameTypeList.Count > 0)
            {
                for (int i = contentPosChallanges.childCount - 1; i >= 0; i--)
                {
                    string gameType = challengeGameTypeList[i];

                    Transform child = contentPosChallanges.GetChild(i);

                    if (gameType == "timer")
                    {
                        var item = child.GetComponent<TimerItem>();
                        if (item != null && item.TableId == roomID)
                        {
                            Destroy(child.gameObject);
                            challengeGameTypeList.RemoveAt(i);

                        }
                    }
                    else if (gameType == "classic")
                    {
                        var item = child.GetComponent<ClassicItem>();
                        if (item != null && item.TableId == roomID)
                        {
                            Destroy(child.gameObject);
                            challengeGameTypeList.RemoveAt(i);
                        }
                    }
                }

                challengeElementCountText.text = challengeGameTypeList.Count.ToString();
                //Debug.Log("Challenge count1-" + challengeCount);
                if (challengeGameTypeList.Count == 0)
                {
                    offlineChallange.SetActive(true);
                }
            }
        }

        List<string> challengeGameTypeList = new List<string>();
        public void ChallengeTables(NewChallengesResponse newChallengesResponse)
        {           
            if (newChallengesResponse.is_active)
            {
                offlineChallange.SetActive(false);
                var item = newChallengesResponse.tableData;

                if (newChallengesResponse.tableData.game_type == "timer")
                {
                    challengeGameTypeList.Add("timer");
                    GameObject element = Instantiate(contentElementTimer, contentPosChallanges, false);
                    element.SetActive(true);
                    element.GetComponent<TimerItem>().TableName = item.tablename.ToString();
                    element.GetComponent<TimerItem>().gameVariant = item.game_varient.ToString();
                    element.GetComponent<TimerItem>().gametype = item.game_type.ToString();
                    element.GetComponent<TimerItem>().TableId = item._id.ToString();
                    element.GetComponent<TimerItem>().maxplayerCount = item.max_player.ToString();

                    element.GetComponent<TimerItem>().entryValueText.text = item.entry_fee.ToString();

                    element.GetComponent<TimerItem>().TimerText.text = item.game_time.ToString();



                    if (item.max_player == 4)
                    {
                        element.GetComponent<TimerItem>().maxplayerCountImage.sprite = playerCount4;
                    }
                    else if (item.max_player == 3)
                    {
                        element.GetComponent<TimerItem>().maxplayerCountImage.sprite = playerCount3;
                    }
                    else if (item.max_player == 2)
                    {
                        element.GetComponent<TimerItem>().maxplayerCountImage.sprite = playerCount2;
                    }
                    if (item.prize_distribution.first_winner != 0)
                    {
                        element.GetComponent<TimerItem>().prizePool1.transform.GetChild(1).GetComponent<Text>().text =
                            item.prize_distribution.first_winner.ToString();
                        element.GetComponent<TimerItem>().prizePool2.SetActive(false);
                    }

                    
                }
                else if (newChallengesResponse.tableData.game_type == "classic")
                {
                    challengeGameTypeList.Add("classic");
                    GameObject element = Instantiate(contentElementClassic, contentPosChallanges, false);
                    element.SetActive(true);
                    element.GetComponent<ClassicItem>().TableName = item.tablename.ToString();
                    element.GetComponent<ClassicItem>().gameVariant = item.game_varient.ToString();
                    element.GetComponent<ClassicItem>().gametype = item.game_type.ToString();
                    element.GetComponent<ClassicItem>().TableId = item._id.ToString();
                    element.GetComponent<ClassicItem>().maxplayerCount = item.max_player.ToString();

                    element.GetComponent<ClassicItem>().entryValueText.text = item.entry_fee.ToString();
                    if (item.game_varient[0] == "4H")
                    {
                        element.GetComponent<ClassicItem>().gameVariantImage.sprite = H4;
                    }
                    else
                    {
                        element.GetComponent<ClassicItem>().gameVariantImage.sprite = H1;
                    }

                    if (item.max_player == 4)
                    {
                        element.GetComponent<ClassicItem>().maxplayerCountImage.sprite = playerCount4;
                    }
                    else if (item.max_player == 3)
                    {
                        element.GetComponent<ClassicItem>().maxplayerCountImage.sprite = playerCount3;
                    }
                    else if (item.max_player == 2)
                    {
                        element.GetComponent<ClassicItem>().maxplayerCountImage.sprite = playerCount2;
                    }
                    if (item.prize_distribution.first_winner != 0)
                    {
                        element.GetComponent<ClassicItem>().prizePool1.transform.GetChild(1).GetComponent<Text>().text =
                            item.prize_distribution.first_winner.ToString();
                        element.GetComponent<ClassicItem>().prizePool2.SetActive(false);
                    }
                    
                }

            }
            else
            {
                offlineChallange.SetActive(true);
            }

            int challengeCount = contentPosChallanges.childCount;

            
            challengeElementCountText.text = challengeCount.ToString();


        }
        #endregion

        #region Classic Modes
        [Header("Classic")]
        public Transform contentPosClassic;
        public GameObject contentElementClassic;
        public GameObject offlineClassic;
        public Text classicelementCountText;

        private void ClassicTables()
        {
            var classicData = ludoGameTableList_SO.AllData.data.classic;
            classicelementCountText.text = classicData.Count.ToString();

            if (ludoGameTableList_SO.AllData.data.classic.Count > 0)
            {

                offlineClassic.SetActive(false);

                foreach (var item in classicData)
                {
                    GameObject element = Instantiate(contentElementClassic, contentPosClassic, false);
                    element.SetActive(true);
                    element.GetComponent<ClassicItem>().TableName = item.tablename.ToString();
                    element.GetComponent<ClassicItem>().gameVariant = item.game_varient.ToString();
                    element.GetComponent<ClassicItem>().gametype = item.game_type.ToString();
                    element.GetComponent<ClassicItem>().TableId = item._id.ToString();
                    element.GetComponent<ClassicItem>().maxplayerCount = item.max_player.ToString();

                    element.GetComponent<ClassicItem>().entryValueText.text = item.entry_fee.ToString();
                    if (item.game_varient[0] == "4H")
                    {
                        element.GetComponent<ClassicItem>().gameVariantImage.sprite = H4;
                    }
                    else
                    {
                        element.GetComponent<ClassicItem>().gameVariantImage.sprite = H1;
                    }

                    if (item.max_player == 4)
                    {
                        element.GetComponent<ClassicItem>().maxplayerCountImage.sprite = playerCount4;
                    }
                    else if (item.max_player == 3)
                    {
                        element.GetComponent<ClassicItem>().maxplayerCountImage.sprite = playerCount3;
                    }
                    else if (item.max_player == 2)
                    {
                        element.GetComponent<ClassicItem>().maxplayerCountImage.sprite = playerCount2;
                    }
                    if (item.prize_distribution.first_winner != 0)
                    {
                        element.GetComponent<ClassicItem>().prizePool1.transform.GetChild(1).GetComponent<Text>().text =
                            item.prize_distribution.first_winner.ToString();
                        element.GetComponent<ClassicItem>().prizePool2.SetActive(false);
                    }

                }
            }
            else
            {
                offlineClassic.SetActive(true);
            }


        }
        #endregion

        #region Timer Modes
        [Space]
        [Header("Timer")]
        public Transform contentPosTimer;
        public GameObject contentElementTimer;
        public GameObject offlineTimer;
        public Text timerelementCountText;
        private void TimerTables()
        {
            timerelementCountText.text = ludoGameTableList_SO.AllData.data.timer.Count.ToString();

            if (ludoGameTableList_SO.AllData.data.timer.Count > 0)
            {
                offlineTimer.SetActive(false);

                foreach (var item in ludoGameTableList_SO.AllData.data.timer)
                {
                    GameObject element = Instantiate(contentElementTimer, contentPosTimer, false);
                    element.SetActive(true);
                    element.GetComponent<TimerItem>().TableName = item.tablename.ToString();
                    element.GetComponent<TimerItem>().gameVariant = item.game_varient.ToString();
                    element.GetComponent<TimerItem>().gametype = item.game_type.ToString();
                    element.GetComponent<TimerItem>().TableId = item._id.ToString();
                    element.GetComponent<TimerItem>().maxplayerCount = item.max_player.ToString();

                    element.GetComponent<TimerItem>().entryValueText.text = item.entry_fee.ToString();

                    element.GetComponent<TimerItem>().TimerText.text = item.game_time.ToString();



                    if (item.max_player == 4)
                    {
                        element.GetComponent<TimerItem>().maxplayerCountImage.sprite = playerCount4;
                    }
                    else if (item.max_player == 3)
                    {
                        element.GetComponent<TimerItem>().maxplayerCountImage.sprite = playerCount3;
                    }
                    else if (item.max_player == 2)
                    {
                        element.GetComponent<TimerItem>().maxplayerCountImage.sprite = playerCount2;
                    }
                    if (item.prize_distribution.first_winner != 0)
                    {
                        element.GetComponent<TimerItem>().prizePool1.transform.GetChild(1).GetComponent<Text>().text =
                            item.prize_distribution.first_winner.ToString();
                        element.GetComponent<TimerItem>().prizePool2.SetActive(false);
                    }
                }
            }
            else
            {
                offlineTimer.SetActive(true);
            }


        }
        #endregion

        #region Join-TableAPI


        public void CallTableApiData(string table_Id)
        {
            StartCoroutine(GetTableApiData(table_Id));
            Debug.Log("Item");
        }

        public static string Endstring_TableAPI = "/ludo/join-table";
        IEnumerator GetTableApiData(string table_Id)
        {
            GameManagerLudo.instance.winnerPanel.SetActive(false);
            GameManagerLudo.instance.matchMakingPanel.SetActive(true);
            WWWForm form = new WWWForm();

            form.AddField("operator_id", ApiCallingLudo.instance.gameInit_SO.AllData.data.userinfo.operator_name);
            form.AddField("game_code", ApiCallingLudo.instance.gameInit_SO.AllData.data.userinfo.game_code);
            form.AddField("player_id", ApiCallingLudo.instance.gameInit_SO.AllData.data.userinfo.player_id);
            form.AddField("table_id", table_Id);


            using UnityWebRequest www = UnityWebRequest.Post(ApiCallingLudo.MainUrlGameSetting + Endstring_TableAPI, form);
            www.SetRequestHeader("Authorization", "Bearer " + ApiCallingLudo.instance.gameInit_SO.AllData.data.userinfo.session_token);

            Debug.Log("   url :-  " + www.url);
            Debug.Log("\n operator_id :" + ApiCallingLudo.instance.gameInit_SO.AllData.data.userinfo.operator_name +
                "\n game_code:" + ApiCallingLudo.instance.gameInit_SO.AllData.data.userinfo.game_code +
                "\n player_id:" + ApiCallingLudo.instance.gameInit_SO.AllData.data.userinfo.player_id

                + "\n table_id:"+ table_Id);
                
            yield return www.SendWebRequest();

            string s = www.downloadHandler.text;
            print(s);
            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Can't connect to api, connection error !!!");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Can't connect to api, protocol erros !!!");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Can't connect to api, data processing error !!!");
                    break;

                case UnityWebRequest.Result.Success:
                    StartCoroutine(SetJoinDataForTable(s));
                    break;
            }
            print(" time taken by TableList  API : "
               + Time.time);
        }

        public static string room_id;
        public static string room_name;
        public static string ip;
        public static string port;

        public GameObject classicTable;
        public GameObject timerTable;

        IEnumerator SetJoinDataForTable(string s)
        {
            JsonData jd = JsonMapper.ToObject(s);

            if ((bool)jd["status"])
            {
                room_id = jd["room_id"].ToString();
                room_name = jd["room_code"].ToString();
                ip = jd["ip"].ToString();
                port = jd["port"].ToString();
            }
            ApiCallingLudo.instance.gamePlaypannel.SetActive(true);
            yield return new WaitForSeconds(1);
            GameManagerLudo.instance.SetUserDetails();
        }
        #endregion

        [Space]
        [Header("Player details")]
        public Image PlayerProfileImage;
        public Text playerBalanceText;
        public Text playerNameText;
        void SetPlayerDetails()
        {
            PlayerBalance = ludoGameInitiate.AllData.data.userinfo.wallet;
           

            playerNameText.text = ludoGameInitiate.AllData.data.userinfo.player_nickname;
            Debug.Log("Player name--" + ludoGameInitiate.AllData.data.userinfo.player_nickname);
            if (ludoGameInitiate.AllData.data.userinfo.profile_pic != null)
            {
                PlayerProfileImage.sprite = ApiCallingLudo.instance.myProfileSprite;
            }
            else
            {
                PlayerProfileImage.sprite = ApiCallingLudo.instance.defaultDP;
            }

        }
    }
}