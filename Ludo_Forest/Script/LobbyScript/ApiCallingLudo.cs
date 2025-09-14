using BestHTTP.JSON.LitJson;
using DG.Tweening;
using OMS;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LudoMGP
{
    public class ApiCallingLudo : MonoBehaviour
    {
        public static ApiCallingLudo instance;
        public static string _sessionID;
        public static bool _gameSessionStatus;

        public LudoGameInitiate_SO gameInit_SO;

        public LudoGameTableList_SO ludotableList_SO;
        GameInitiateParam_SO gameInitiateParam_SO;

        public GameObject gamePlaypannel;

        public Sprite myProfileSprite, defaultDP;
        public Image myProfileImage;
        public LudoModesTableList ludoModesTable;

        public static string MainGameUrl;
  
        public static string MainUrlGameSetting;


        public static string Endstring_GameInitApi = "/get-Initiate-Game";

        public static string Endstring_TableList = "/ludo/table-list";


        public static bool loadFromLocal = false;
        public static string APKLEVEL = "staging";
        //string APKLEVEL = "dev";
        private void Awake()
        {
            gameInitiateParam_SO = Resources.Load<GameInitiateParam_SO>("GameInitiateParams");           
        }
        private void OnEnable()
        {
           
        }

        private void Start()
        {
            instance = this;
            Application.runInBackground = true;

            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToLandscapeLeft = false;            

            if (gameInitiateParam_SO.myProfile.ContainsKey("defaultdp"))
            {
                defaultDP = gameInitiateParam_SO.myProfile["defaultdp"];
            }

            if (gameInitiateParam_SO.myProfile.ContainsKey("mydp"))
            {
                myProfileSprite = gameInitiateParam_SO.myProfile["mydp"];
                myProfileImage.sprite = myProfileSprite;
            }

            //.............dev
            //MainGameUrl = "http://18.143.78.245:2200";
            //MainUrlGameSetting = "http://192.168.0.101:4102";//http://192.168.0.101:4102
            //...................staging
            MainGameUrl = "http://18.143.78.245:2200";
            MainUrlGameSetting = "http://54.179.180.24:4102";//
            StartCoroutine(GameInititate());
        }

        private void OnDisable()
        {
            //DynamicURLsManager_Poker.StartTheGame -= StartApiCalling;
           
           

        }

        public string localFilePath;
        private void Start1()
        {
            localFilePath = Application.persistentDataPath + "/localTableListData_" + gameInitiateParam_SO.game_code + ".txt";

            if (File.Exists(localFilePath))
            {
                //ReadFromFileAsync(localFilePath);
                //delay = 0f;
                loadFromLocal = true;
            }
            else
            {

            }
        }
        public void ReadFromFileAsync(string filePath)
        {
            string s = System.IO.File.ReadAllText(filePath);
            Debug.Log(s);

            //TableListAllInData tableList = JsonMapper.ToObject<TableListAllInData>(s);

            //StartCoroutine(GenerateTableList(tableList));
        }

        public async void WriteToFileAsync(string content, string filePath)
        {
            try
            {
                await Task.Run(() =>
                {
                    // Write text to the file asynchronously
                    System.IO.File.WriteAllText(filePath, content);
                });

                Debug.Log("File write operation completed successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("File write operation failed: " + e.Message);
            }
        }

        #region GameInitiate
        IEnumerator GameInititate()
        {
            yield return null;

            PlayerPrefs.SetString("LastAuthToken", gameInitiateParam_SO.token);
            WWWForm form = new WWWForm();

            form.AddField("operator_code", gameInitiateParam_SO.operator_name);
            form.AddField("game_code", gameInitiateParam_SO.game_code);
            form.AddField("integration_env", gameInitiateParam_SO.integration_env);

            using UnityWebRequest www = UnityWebRequest.Post(MainGameUrl + Endstring_GameInitApi, form);
            www.SetRequestHeader("Authorization", "Bearer " + gameInitiateParam_SO.token);
            Debug.Log("Request --/get-Initiate-Game--// -- " + MainGameUrl + Endstring_GameInitApi + "operator_code :- " + gameInitiateParam_SO.operator_name + " " + "game_code :- " + gameInitiateParam_SO.game_code + " " + "integration_env :- " + gameInitiateParam_SO.integration_env
                + "  TOKEN :-  " + gameInitiateParam_SO.token);
            yield return www.SendWebRequest();
            gameInitiateParam_SO.token = "";

            string s = www.downloadHandler.text;

            print(" time taken by Game Initiate API : "
                + Time.time);
            Debug.Log(www.result);
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

                    ManipulateJSON(s);

                    break;
            }
        }

        void ManipulateJSON(string str)
        {
            Debug.Log("RESPONSE--get-Initiate-Game--" + str);
            JsonData jd = JsonMapper.ToObject(str);

            jd["data"]["userinfo"]["wallet"] = double.Parse(jd["data"]["userinfo"]["wallet"].ToString());
            string s = JsonMapper.ToJson(jd);


            GameInitData giAPIdet = JsonMapper.ToObject<GameInitData>(s);
            if (giAPIdet.status)
            {
                gameInit_SO.AllData = giAPIdet;

                StartCoroutine(GetLudoTableList());

            }
        }
        #endregion

        #region LudoTableList

        
        IEnumerator GetLudoTableList()
        {
            WWWForm form = new WWWForm();
            form.AddField("operator_id", gameInit_SO.AllData.data.userinfo.operator_name);
            form.AddField("game_code", gameInitiateParam_SO.game_code);
            //form.AddField("max_player", "4");
            form.AddField("player_id", gameInit_SO.AllData.data.userinfo.player_id);


            Debug.Log("REQUEST  /poker/table-list :-" + "operator_id :-" + MainUrlGameSetting + Endstring_TableList + gameInit_SO.AllData.data.userinfo.operator_name + " " + "game_code ;-"
                + "MGP010067" + "  " + "player_id ;-" + gameInit_SO.AllData.data.userinfo.player_id + "\n token - \n" + gameInit_SO.AllData.data.userinfo.session_token);

            using UnityWebRequest www = UnityWebRequest.Post(MainUrlGameSetting + Endstring_TableList, form);
            www.SetRequestHeader("Authorization", "Bearer " + gameInit_SO.AllData.data.userinfo.session_token);

            Debug.Log("   url :-  " + www.url);
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
                    TableListManipulateJSON(s);
                    break;
            }
            print(" time taken by TableList  API : "
               + Time.time);
        }

        private int SafeParseInt(JsonData data, string key)
        {
            return data.Keys.Contains(key) && data[key] != null && int.TryParse(data[key].ToString(), out int result)
                ? result
                : 0;
        }


        void TableListManipulateJSON(string str)
        {
            JsonData jd = JsonMapper.ToObject(str);


            foreach (JsonData item in jd["data"]["classic"])
            {
               
                //item["game_varient"] = item["game_varient"].ToString();
                item["entry_fee"] = item["entry_fee"].ToString();
                item["network_player_presit"] = item["network_player_presit"].ToString();
                item["max_network_player"] = item["max_network_player"].ToString();
            }
            foreach (JsonData item in jd["data"]["timer"])
            {
                          
                item["entry_fee"] = item["entry_fee"].ToString();
                item["network_player_presit"] = item["network_player_presit"].ToString();
                item["max_network_player"] = item["max_network_player"].ToString();
            }
            string s = JsonMapper.ToJson(jd);
            LudoTableListData tableList = JsonMapper.ToObject<LudoTableListData>(s);

            if (tableList.status)
            {
                ludotableList_SO.AllData = tableList;

                if (!string.IsNullOrEmpty(gameInitiateParam_SO.friend_player_id))
                {
                    // StartCoroutine(GetFriendStatus());
                }
                else if (!string.IsNullOrEmpty(gameInitiateParam_SO.room_code))
                {
                    // StartCoroutine(GetDirectRoomDetails(gameInitiateParam_SO.room_code));
                }
                ludoModesTable.CalledTableList();
            }
            else
            {
                Debug.Log($"Error Message : {tableList.message} ");
            }
        }

        #endregion

        public void ExitToMainLobby()
        {
            //if (SceneChangerScript.instance.servergames.Contains("MZ020116"))
            //{
            //    //OMS.LoadAddressables.instance.UnloadGame();
            //}
            //else
            //{
            //    StartCoroutine(LoadSceneAsyncCoroutine(SceneManager.GetActiveScene().name, 0));
            //}
            LudoSocketManager.instance.DisconnectSocket();

           
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
      
        public GameObject CurrentShowPopup;
        IEnumerator Alert(string str)
        {
            CurrentShowPopup.SetActive(true);
            CurrentShowPopup.GetComponentInChildren<Text>().text = "Please wait for next session";
            CurrentShowPopup.transform.DOScale(1, 1f);
            yield return new WaitForSeconds(2);
            CurrentShowPopup.transform.DOScale(0, 1f);
            yield return new WaitForSeconds(1);
            CurrentShowPopup.SetActive(false);
        }

        IEnumerator LoadSceneAsyncCoroutine(string oldScene, int newScene)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newScene);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                // You can update a loading bar or display loading progress here
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                Debug.Log("Loading progress: " + (progress * 100) + "%");
                yield return null;
            }
            SceneManager.UnloadSceneAsync(oldScene);
        }



        public static string StringBetAmount(double amount)
        {
            string value = amount.ToString();

            if (amount >= 10000000) value = string.Format("{0:0.##}", (amount / 10000000)) + "Cr";
            else if (amount >= 100000) value = string.Format("{0:0.##}", (amount / 100000)) + "L";
            else if (amount >= 1000) value = string.Format("{0:0.##}", (amount / 1000)) + "K";
            else value = string.Format("{0:0.##}", amount);
            return value;
        }
        #region Friend and Direct room details

        //IEnumerator GetFriendStatus()
        //{
        //    WWWForm form = new WWWForm();
        //    form.AddField("friend_player_id", gameInitiateParam_SO.friend_player_id);
        //    form.AddField("player_id", gameInit_SO.AllData.data.userinfo.player_id);
        //    form.AddField("session_token", gameInit_SO.AllData.data.userinfo.session_token);

        //    using UnityWebRequest www = UnityWebRequest.Post(MainGameUrl + "/game/find-userRoom", form);
        //    print(www.url + " player_id : " + gameInit_SO.AllData.data.userinfo.player_id + " friend_player_id : " + gameInitiateParam_SO.friend_player_id + " \nsession_token : " + gameInit_SO.AllData.data.userinfo.session_token);

        //    yield return www.SendWebRequest();

        //    string s = www.downloadHandler.text;
        //    JsonData jd = JsonMapper.ToObject(s);
        //    print(s);
        //    if (!((bool)jd["status"]))
        //    {
        //        Debug.Log(jd["message"]);

        //        yield return null;
        //    }
        //    else
        //    {
        //        switch (www.result)
        //        {
        //            case UnityWebRequest.Result.ConnectionError:
        //                Debug.LogError("Can't connect to api, connection error !!!");
        //                break;
        //            case UnityWebRequest.Result.ProtocolError:
        //                Debug.LogError("Can't connect to api, protocol erros !!!");
        //                break;
        //            case UnityWebRequest.Result.DataProcessingError:
        //                Debug.LogError("Can't connect to api, data processing error !!!");
        //                break;

        //            case UnityWebRequest.Result.Success:

        //                if (jd["data"].ContainsKey("game_code"))
        //                {
        //                    if (jd["data"]["game_code"].ToString() == gameInitiateParam_SO.game_code)
        //                    {
        //                        string room = jd["data"]["room_name"].ToString();
        //                        //PublicTable currRoom = _allIntableList_SO.AllData.data.public_tables.ToList().Find(x => x._id == room);

        //                        if (currRoom != null)
        //                        {


        //                        }
        //                        else
        //                        {


        //                            gamePlaypannel.SetActive(true);


        //                        }
        //                    }
        //                    else
        //                    {
        //                        Debug.Log("other game detailes -------------- " + jd["data"]["game_code"].ToString());
        //                    }
        //                }


        //                //GenerateTableList();

        //                break;
        //        }
        //    }
        //}
        //IEnumerator GetDirectRoomDetails(string room_code)
        //{
        //    WWWForm form = new WWWForm();
        //    form.AddField("player_id", gameInit_SO.AllData.data.userinfo.player_id);  // PlayerPrefs.GetString(PlayerPrefsKeys.PLAYERID)
        //    form.AddField("room_code", room_code);
        //    form.AddField("auth_token", gameInit_SO.AllData.data.userinfo.auth_token);

        //    using UnityWebRequest www = UnityWebRequest.Post(MainGameUrl + "/game/fetch-roomCodeDetails", form);
        //    Debug.Log("player_id " + gameInit_SO.AllData.data.userinfo.player_id + "\n room_code " + room_code + "\n auth_token " + gameInit_SO.AllData.data.userinfo.auth_token);
        //    yield return www.SendWebRequest();

        //    string s = www.downloadHandler.text;
        //    print(s);
        //    JsonData jd = JsonMapper.ToObject(s);

        //    if (!((bool)jd["status"]))
        //    {
        //        Debug.Log(jd["message"]);

        //        yield return null;
        //    }
        //    else
        //    {
        //        switch (www.result)
        //        {
        //            case UnityWebRequest.Result.ConnectionError:
        //                Debug.LogError("Can't connect to api, connection error !!!");
        //                break;
        //            case UnityWebRequest.Result.ProtocolError:
        //                Debug.LogError("Can't connect to api, protocol erros !!!");
        //                break;
        //            case UnityWebRequest.Result.DataProcessingError:
        //                Debug.LogError("Can't connect to api, data processing error !!!");
        //                break;

        //            case UnityWebRequest.Result.Success:

        //                if (jd["data"].ContainsKey("game_code"))
        //                {
        //                    if (jd["data"]["game_code"].ToString() == gameInitiateParam_SO.game_code)
        //                    {
        //                        string room = jd["data"]["room_name"].ToString();
        //                        PublicTable currRoom = _allIntableList_SO.AllData.data.public_tables.ToList().Find(x => x._id == room);

        //                        if (currRoom != null)
        //                        {

        //                        }
        //                        else
        //                        {
        //                            gamePlaypannel.SetActive(true);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Debug.Log("other game detailes -------------- " + jd["data"]["game_code"].ToString());
        //                    }
        //                }
        //                //GenerateTableList();
        //                break;
        //        }
        //    }
        //}
        #endregion
    }

}