using BestHTTP.JSON.LitJson;
using BestHTTP.SocketIO3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;


namespace LudoMGP
{
    public class LudoSocketManager : MonoBehaviour
    {
        public static LudoSocketManager instance;


        SocketManager socketManager;
        Socket rootSocket;


        //#eb4034//red
        //#eb9e34//orange
        //#2b8030//green
        //#19218c//blue

        #region ReponseEventsName

        public const string UserValidation_On = "user_validation";
        public const string New_user_join_On = "new_user_join";
        public const string User_join_game_response_On = "user_join_game_response";
        public const string Room_code_info_On = "room_code_info";
        public const string PlayerInfo_On = "player_info";
        public const string PrepTimer_On = "prep_timer";
        public const string GameStarted_On = "game_started";
        public const string PlayerTurn_On = "player_turn";
        public const string Turn_missed_On = "turn_missed";
        public const string Dice_result_On = "dice_result";
        public const string Pawn_move_On = "pawn_move";
        public const string Message_notification_On = "message_notification";
        public const string PlayersList_On = "players_list";
        public const string Pawn_killed_On = "pawn_killed";
        public const string Winner_declared_On = "winner_declared";
        public const string FinalWinnerResponse_On = "final_winner_response";
        public const string LeaveRequest_On = "leave_request_response";
        public const string RoomInfo_On = "room_info";
        public const string GamePlayTime_On = "game_play_time";
        public const string Reconnect_Response_On = "reconnect_response";
        public const string DeviceInfo_On = "device_info";

        public const string Updated_Wallet_On = "updated_wallet";

        public const string GameResult_On = "game_result";

        public const string GameHistoryRequest_On = "game_history";

        public const string ChatMessage_On = "chat_message_receive";
        public const string UserUpdatedWallet_On = "user_updated_wallet";

        public const string GameSummary_On = "game_summary";

        #endregion ReponseEventsName

        // --------------------/////////////////////////////
        // --------------------...................................
        // --------------------/////////////////////////////

        #region RequestEventsName

        public const string UserInfo_Emit = "user_join_game";
        public const string PlayersList_Emit = "players_list";
        public const string DiceRolled_Emit = "dice_rolled";
        public const string LeaveRequest_Emit = "leave_request";
        public const string SelectedPawn_Emit = "selected_pawn";
        public const string ReconnectUser_Emit = "reconnect_user";
        public const string JoinPrivateRoom_Emit = "join_private_room";
        public const string Device_info_Emit = "device_info";

        #endregion RequestEventsName

        public const string New_challenges_Event = "new_challenges";
        public const string User_count_challenge_Event = "user_count_challenge";
        public const string Challenge_close_Event = "challenge_close";
        // ---------------------
        #region Challenges Events

        #endregion
        // ---------------------

        private void Awake()
        {
            instance = this;
            // Connect(IpAddressLocal, portLocal);
        }

        private void Start()
        {
            //Connect(IpAddress, port);
        }

        private void OnEnable()
        {


        }

        private void OnDisable()
        {

        }


        #region Background Process
        void ConnectSocket()
        {
            Debug.Log("Application Connected Again");
            Connect(GameManagerLudo.ip, GameManagerLudo.port);
            //Connect(ApiCallingLudo.SocketIp, ApiCallingLudo.SocketPort);
        }

        public void DisconnectSocket()
        {
            if (IsSocketConnected)
            {
                socketManager.Close();
            }
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Debug.Log("Application in backGround");
                ApiCallingLudo._sessionID = sessionID;
                DisconnectSocket();
                //IsSessionResultComing = false;
                ApiCallingLudo._gameSessionStatus = true;
            }
            else
            {
                // Application is coming back to foreground
                Debug.Log("Application in foreground");
                ConnectSocket();
                StartCoroutine(StartTiming());
            }
        }

        IEnumerator StartTiming()
        {
            yield return new WaitForSeconds(1.5f);
            if (ApiCallingLudo._sessionID != sessionID)
            {
                //GameManagerScript_Poker.instance.ResetAllDetails();
            }
            StopCoroutine(StartTiming());

        }
        #endregion

        public void Connect(string IpAddress, string Port)
        {
            Debug.Log(IpAddress);
            //Port = "4041";
            Uri uri = new Uri(string.Format($"http://{IpAddress}:{Port}"));
            Debug.Log("Url : " + uri);
            SocketOptions options = new SocketOptions();
            options.AutoConnect = false;

            socketManager = new SocketManager(uri, options);

            rootSocket = socketManager.Socket;
            socketManager.Open();
            rootSocket.On(SocketIOEventTypes.Connect, OnConnected);
            rootSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected);
            rootSocket.On<object>(SocketIOEventTypes.Error, OnErrorOccur);
            rootSocket.On("reconnect", OnReconnect);
            rootSocket.On("reconnecting", OnReconnecting);
            rootSocket.On("reconnect_attempt", OnReconnectAttempt);
            rootSocket.On("reconnect_failed", OnReconnectFailed);


            rootSocket.On<object>(UserValidation_On, UserJoinRoomReponse);
            rootSocket.On<object>(New_user_join_On, NewUserJoinReponse);
            rootSocket.On<object>(User_join_game_response_On, UserJoinGameReponse);
            rootSocket.On<object>(Room_code_info_On, RoomCodeInfoReponse);

            rootSocket.On<object>(PlayerInfo_On, PlayerInfoReponse);
            rootSocket.On<object>(PrepTimer_On, PrepTimerResponse);
            rootSocket.On<object>(GameStarted_On, GameStartedResponse);
            rootSocket.On<object>(PlayerTurn_On, PlayerTurnResponse);
            rootSocket.On<object>(Turn_missed_On, TurnMissedResponse);
            rootSocket.On<object>(Dice_result_On, DiceResultResponse);
            rootSocket.On<object>(Pawn_move_On, PawnMoveResultResponse);
            rootSocket.On<object>(Message_notification_On, MessageNotificationResponse);
            rootSocket.On<object>(PlayersList_On, PlayerListResponse);
            rootSocket.On<object>(Pawn_killed_On, PawnkilledResponse);
            rootSocket.On<object>(Winner_declared_On, WinnerDeclaredResponse);
            rootSocket.On<object>(FinalWinnerResponse_On, FinalWinnerResponse);
            rootSocket.On<object>(LeaveRequest_On, _LeaveRequestResponse);
            rootSocket.On<object>(RoomInfo_On, RoomInfoResponse);
            rootSocket.On<object>(GamePlayTime_On, GamePlayTimeReponse);
            rootSocket.On<object>(Reconnect_Response_On, ReconnectReponse);
            rootSocket.On<object>(DeviceInfo_On, DeviceInfoReponse);

            rootSocket.On<object>(Updated_Wallet_On, UpdatedWallet_Response);
            rootSocket.On<object>(UserUpdatedWallet_On, USERUpdatedWallet_Response);
            rootSocket.On<object>("renew_token", RenewToken_Response);
         


            rootSocket.On<object>(New_challenges_Event, NewChallenges);
            rootSocket.On<object>(User_count_challenge_Event, UserCountChallenges);
            rootSocket.On<object>(Challenge_close_Event, ChallangesClose);


        }

        #region Responses
        public static bool IsSocketConnected;
        void OnConnected()
        {
            Debug.Log($"<color=green>Socket Id :  {rootSocket.Id}</color>");
            //SendUserDetails();
            SenDeviceInfo();
            IsSocketConnected = true;
        }

        void OnDisconnected()
        {
            IsSocketConnected = false;
            Debug.Log($"<color=red>Disconnected Socket Id :  {rootSocket.Id}</color>");

            OMS.NetworkConnectivity.SocketConnected?.Invoke(false);

        }

        void OnErrorOccur(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=red>Error Occured. {s}</color>");

            IsSocketConnected = false;
        }

        void OnReconnect()
        {
            Debug.Log("OnReconnect");
            IsSocketConnected = false;
            ReconnectUser();
        }

        void OnReconnecting()
        {
            IsSocketConnected = false;
            Debug.Log("OnReconnecting");
            OMS.NetworkConnectivity.SocketConnected?.Invoke(false);
        }

        void OnReconnectAttempt()
        {
            Debug.Log("OnReconnectAttempt");
            OMS.NetworkConnectivity.SocketConnected?.Invoke(false);
        }

        void OnReconnectFailed()
        {
            Debug.Log("OnReconnectFailed");
        }


        ////////////////////////////////
    
        ////////////////////////////////
        void UserJoinRoomReponse(object room_Join_Request)//Show only
        {
            string s = JsonMapper.ToJson(room_Join_Request);
            Debug.Log($"<color=yellow> Reponse: {UserValidation_On} , \n Data : {s}  </color>");
        }
        ////////////////////////////////
        void NewUserJoinReponse(object room_Join_Request)//Show only
        {
            string s = JsonMapper.ToJson(room_Join_Request);
            Debug.Log($"<color=yellow> Reponse: {New_user_join_On} , \n Data : {s}  </color>");
        }
        ////////////////////////////////
        void UserJoinGameReponse(object room_Join_Request)//Show only
        {
            string s = JsonMapper.ToJson(room_Join_Request);
            Debug.Log($"<color=yellow> Reponse: {User_join_game_response_On} , \n Data : {s}  </color>");
        }

        ////////////////////////////////
        void RoomCodeInfoReponse(object room_Join_Request)//show only
        {
            string s = JsonMapper.ToJson(room_Join_Request);
            Debug.Log($"<color=yellow> Reponse: {Room_code_info_On} , \n Data : {s}  </color>");
        }

        bool IsplayerLeave;
        ////............................................................................
        void _LeaveRequestResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb3461> Reponse: {LeaveRequest_On} , \n Data : {s}  </color>");

            JsonData jsonData = JsonMapper.ToObject(s);
            if (jsonData != null)
            {
                LeaveRoomRespose leaveRoom = JsonMapper.ToObject<LeaveRoomRespose>(s);
               
                if (leaveRoom.player_id == GameManagerLudo.playerID)
                {
                    PawnManager.instance.ResetAll();
                    WinnerPanelScript.instance.WinnerpanelObj.SetActive(false);
                    MatchMakingScript.instance.CancelMatchMaking();
                    ApiCallingLudo.instance.gamePlaypannel.SetActive(false);
                    IsplayerLeave = true;
                }
                else
                {
                    foreach (var player in GameManagerLudo.instance.playerScriptList)
                    {
                        if (player.playerID == leaveRoom.player_id)
                        {
                            player.playerLeaveMessagetext.text = "User Leave !";
                        }
                        else
                        {
                            player.playerLeaveMessagetext.text = "";
                        }
                    }
                }
            }
        }
        ////............................................................................
        public string sessionID = "";
        ////............................................................................




        void RoomInfoResponse(object obj)
        {
            try
            {
                string jsonStr = JsonMapper.ToJson(obj);
                Debug.Log($"<color=#eb4034>Response: {RoomInfo_On},\nData: {jsonStr}</color>");

                JsonData roomInfo = JsonMapper.ToObject(jsonStr);
                if (roomInfo == null)
                {
                    Debug.LogError("Error: Room info is null or invalid.");
                    return;
                }
                else
                {
                    RoomInfoResponse roomInfoResponse = JsonMapper.ToObject<RoomInfoResponse>(jsonStr);

                    GameManagerLudo.instance.RoomInfoResponse(roomInfoResponse);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception in RoomInfoReponse: {e.Message}\n{e.StackTrace}\nData: {JsonMapper.ToJson(obj)}");
            }
        }

        // Helper Methods
        private long SafeParseLong(JsonData data, string key)
        {
            return data.Keys.Contains(key) && data[key] != null && long.TryParse(data[key].ToString(), out long result)
                ? result
                : 0;
        }

        private int SafeParseInt(JsonData data, string key)
        {
            return data.Keys.Contains(key) && data[key] != null && int.TryParse(data[key].ToString(), out int result)
                ? result
                : 0;
        }


        void PlayerInfoReponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#348ceb> Reponse: {PlayerInfo_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {
                PlayerInfoResponse playerInfoResponse = JsonMapper.ToObject<PlayerInfoResponse>(s);

                GameManagerLudo.instance.PlayerInfoView(playerInfoResponse);
            }
        }




        void PlayerListResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#c6eb34> Reponse: {PlayersList_On} , \n Data : {s}  </color>");

            JsonData playerListdata = JsonMapper.ToObject(s);
            if (playerListdata != null)
            {
                List<PlayerListDetail> playerListResponse = JsonMapper.ToObject<List<PlayerListDetail>>(s);


                GameManagerLudo.instance.PlayerDetailsList(playerListResponse);

            }
        }


        void PrepTimerResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb3461> Reponse: {PrepTimer_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {
                PrepTimerResponse prepTimer = JsonMapper.ToObject<PrepTimerResponse>(s);
                sessionID = prepTimer.session_id;

                if (GameManagerLudo.updated_balance != 0)
                {
                    LudoModesTableList.instance.PlayerBalance = GameManagerLudo.updated_balance;
                }
                StartCoroutine(GameManagerLudo.instance.PrepTimer(prepTimer));

            }
        }

        void GameStartedResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);

            Debug.Log($"<color=#eb3461> Reponse: {GameStarted_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {
                GameStartedResponse gameStarted = JsonMapper.ToObject<GameStartedResponse>(s);

                GameManagerLudo.instance.GameStarted(gameStarted);
            }
        }

        void PlayerTurnResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=green> Reponse: {PlayerTurn_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);

            if (jd != null)
            {
                PlayerTurnResponse playerturn = JsonMapper.ToObject<PlayerTurnResponse>(s);
                StartCoroutine(DelayTimer(1f));
                if (playerturn.extra_move_status)
                {
                    StartCoroutine(GameManagerLudo.instance.ExtraMove());
                }
                GameManagerLudo.instance.PlayerTurn(playerturn);
            }
        }
        void TurnMissedResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Turn_missed_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);

            if (jd != null)
            {
                TurnMissedResponse playerturnmissed = JsonMapper.ToObject<TurnMissedResponse>(s);

                GameManagerLudo.instance.PlayerTurnMissed(playerturnmissed);
            }
        }
        void DiceResultResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Dice_result_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);

            if (jd != null)
            {
                DiceResultResponse playerturn = JsonMapper.ToObject<DiceResultResponse>(s);

                GameManagerLudo.instance.DiceResultTurn(playerturn);
            }
        }

        void PawnMoveResultResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Pawn_move_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {
                PawnMoveResponse pawn = JsonMapper.ToObject<PawnMoveResponse>(s);

                
                PawnManager.instance.MovePawn(pawn.house_color, pawn.pawn_number, pawn.current_position, pawn.new_position);
              
                PawnManager.instance.SetPlayerScore(pawn.house_color, pawn.points.ToString());
                foreach (var player in GameManagerLudo.instance.playerScriptList)
                {
                    if (player.playerID == pawn.player_id)
                    {
                        player.playerScore = pawn.points.ToString();
                    }
                }
            }
        }
        void MessageNotificationResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Message_notification_On} , \n Data : {s}  </color>");
        }
        void PawnkilledResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Pawn_killed_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {
                PawnKilledResponse pawnKilled = JsonMapper.ToObject<PawnKilledResponse>(s);
                StartCoroutine(DelayTimer(1f));
                PawnManager.instance.MovePawn(pawnKilled.house_color, pawnKilled.player_pawn_number, pawnKilled.player_current_position, 0);
            }
        }

        IEnumerator DelayTimer(float t)
        {
            yield return new WaitForSeconds (t);
            
        }
        void WinnerDeclaredResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Winner_declared_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {
                WinnerDeclaredResponse winnerResponse = JsonMapper.ToObject<WinnerDeclaredResponse>(s);
                string color = GameManagerLudo.instance.playerScriptList.Find(x => x.playerID == winnerResponse.player_id).HouseColor;
                PawnManager.instance.SetWinAnimation(color);
            }
        }
        void FinalWinnerResponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {FinalWinnerResponse_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {
                FinalWinnerResponse winnerResponse = JsonMapper.ToObject<FinalWinnerResponse>(s);

                WinnerPanelScript.instance.FinalWinnerResponse(winnerResponse);
            }
        }
        void GamePlayTimeReponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {GamePlayTime_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {
                GamePlayTimeRespose timer = JsonMapper.ToObject<GamePlayTimeRespose>(s);
                StartCoroutine(GameManagerLudo.instance.GamePlayTimerResponse(timer));
            }
        }
        void ReconnectReponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Reconnect_Response_On} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);
            if (jd != null)
            {

            }
        }
        void DeviceInfoReponse(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Reconnect_Response_On} , \n Data : {s}  </color>");
        }

        void UpdatedWallet_Response(object obj)
        {
            string str = JsonMapper.ToJson(obj);
            Debug.Log($"<color=green> Reponse: {Updated_Wallet_On} , \n Data : {str}  </color>");
            JsonData update_wallet = JsonMapper.ToObject(str);
            if ((bool)update_wallet["status"])
            {
                if (update_wallet["type"].ToString() == TransactionType.DEBIT.ToString())
                {
                    LudoModesTableList.instance.PlayerBalance = double.Parse(update_wallet["wallet"].ToString());
                }
                else
                {
                    GameManagerLudo.updated_balance = double.Parse(update_wallet["wallet"].ToString());
                    if (GameManagerLudo.updated_balance != 0)
                    {
                        LudoModesTableList.instance.PlayerBalance = GameManagerLudo.updated_balance;
                        string color = GameManagerLudo.instance.playerScriptList.Find(x =>x.playerID == GameManagerLudo.playerID).HouseColor;                       
                    }
                }
            }
            else
            {
                double amt = double.Parse(update_wallet["amount"].ToString());
                //StartCoroutine(GameManager_Cards32.instance.ErrorPopup($" Your last transaction failed of amount { (amt >= 1e4 ? GameManager_Cards32.StringBetAmount(amt) : amt.ToString())}", 3f));

                //BetManager_Cards32.PlayerBalance = double.Parse(update_wallet["wallet"].ToString());

                //GameManager_Cards32.instance.DestroyCoins(GameManager_Cards32.playerId);
            }
        }


        void RenewToken_Response(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#ffb399> Reponse: renew_token, \n Data : {s}  </color>");

            JsonData jd = JsonMapper.ToObject<JsonData>(s);
        }

        void USERUpdatedWallet_Response(object obj)
        {
            string str = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#00cc00> Reponse: {UserUpdatedWallet_On} , \n Data : {str}  </color>");
            JsonData update_wallet = JsonMapper.ToObject(str);
            if ((bool)update_wallet["status"])
            {

                Debug.Log("wallet updated -----------  " + update_wallet["wallet"].ToString());

                //BuyInPopUpScript.BalanceUpdated?.Invoke(TableManager.instance.MyTotalBalance);

                Debug.Log("Tip submitted");
            }

        }

        #endregion


        #region Requests

        public void SendUserDetails()
        {
            RoomJoinDetailsEmit userDetails = new RoomJoinDetailsEmit();

            userDetails.game_code = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.game_code;
            userDetails.operator_id = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.operator_name;
            userDetails.player_id = GameManagerLudo.playerID;
            userDetails.room_id = GameManagerLudo.room_id;
            userDetails.room_code = GameManagerLudo.room_code;
            userDetails.session_token = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.session_token;

            Debug.Log($"<color=#ffb399> Send: {UserInfo_Emit} , \n Data : {JsonMapper.ToJson(userDetails)}  </color>");

            rootSocket.Emit(UserInfo_Emit, userDetails);

        }

        public void SenDeviceInfo()
        {
            DeviceInfo deviceInfo = new DeviceInfo();
            deviceInfo.player_id = GameManagerLudo.playerID;
            deviceInfo.operator_id = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.operator_name;
            Debug.Log($"<color=#ffb399> Send: {Device_info_Emit} , \n Data : {JsonMapper.ToJson(deviceInfo)}  </color>");

            rootSocket.Emit(Device_info_Emit, deviceInfo);
        }

       
        public void PlayerListRequest()
        {
            Debug.Log("Calling Player list");
            PlayerListEmit playerListEmit = new PlayerListEmit();
            playerListEmit.player_id = GameManagerLudo.playerID;
            playerListEmit.session_id = sessionID;
            playerListEmit.room_id = GameManagerLudo.room_id;
            playerListEmit.room_code = GameManagerLudo.room_code;
            Debug.Log($"<color=#ffb399> Send: {PlayersList_Emit} , \n Data : {JsonMapper.ToJson(playerListEmit)}  </color>");

            rootSocket.Emit(PlayersList_Emit, playerListEmit);
        }

        public void DiceRolled()
        {
            DiceRolledEmit diceRolled = new DiceRolledEmit();

            diceRolled.player_id = GameManagerLudo.playerID;
            diceRolled.session_id = sessionID;
            diceRolled.room_code = GameManagerLudo.room_code;

            Debug.Log($"<color=#ffb399> Send: {DiceRolled_Emit} , \n Data : {JsonMapper.ToJson(diceRolled)}  </color>");

            rootSocket.Emit(DiceRolled_Emit, diceRolled);
        }


        public void LeaveRequest()
        {
            LeaveRequestEmit leaveRequest = new LeaveRequestEmit();

            leaveRequest.game_code = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.game_code;
            leaveRequest.operator_id = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.operator_name;
            leaveRequest.player_id = GameManagerLudo.playerID;
            leaveRequest.room_id = GameManagerLudo.room_id;
            leaveRequest.room_code = GameManagerLudo.room_code;
            leaveRequest.session_id = sessionID;

            Debug.Log($"<color=#ffb399> Send: {LeaveRequest_Emit} , \n Data : {JsonMapper.ToJson(leaveRequest)}  </color>");

            rootSocket.Emit(LeaveRequest_Emit, leaveRequest);
            WinnerPanelScript.instance.WinnerpanelObj.SetActive(false);
        }

        public void SelectedPawn(int pawn_no, string pawn_color, int dice_no, string key)
        {
            SelectedPawnEmit selectedPawn = new SelectedPawnEmit();

            selectedPawn.player_id = GameManagerLudo.playerID;
            selectedPawn.session_id = sessionID;
            selectedPawn.room_code = GameManagerLudo.room_code;

            selectedPawn.pawn_number = pawn_no;
            selectedPawn.house_color = pawn_color;
            selectedPawn.diceNumber = dice_no;
            selectedPawn.key = key;

            Debug.Log($"<color=#ffb399> Send: {SelectedPawn_Emit} , \n Data : {JsonMapper.ToJson(selectedPawn)}  </color>");

            rootSocket.Emit(SelectedPawn_Emit, selectedPawn);
        }


        public void ReconnectUser()
        {
            ReconnectUserEmit reconnectUser = new ReconnectUserEmit();
            reconnectUser.player_id = GameManagerLudo.playerID;
            reconnectUser.operator_id = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.operator_name;
            reconnectUser.game_code = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.game_code;
            reconnectUser.session_token = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.session_token;
            reconnectUser.room_id = GameManagerLudo.room_id;
            reconnectUser.session_id = sessionID;
            reconnectUser.room_code = GameManagerLudo.room_code;

            Debug.Log($"<color=#ffb399> Send: {ReconnectUser_Emit} , \n Data : {JsonMapper.ToJson(reconnectUser)}  </color>");

            rootSocket.Emit(ReconnectUser_Emit, reconnectUser);
        }

        public void JoinPrivateTable(string joinCode)
        {

            JoinPrivateTable privateTable = new JoinPrivateTable();
            privateTable.player_id = GameManagerLudo.playerID;
            privateTable.operator_id = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.operator_name;
            privateTable.game_code = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.game_code;
            privateTable.session_token = GameManagerLudo.instance.ludoGameInitiate.AllData.data.userinfo.session_token;
            privateTable.room_id = GameManagerLudo.room_id;
            privateTable.room_code = "";
            privateTable.join_code = joinCode;

            Debug.Log($"<color=#ffb399> Send: {JoinPrivateRoom_Emit} , \n Data : {JsonMapper.ToJson(privateTable)}  </color>");

            rootSocket.Emit(JoinPrivateRoom_Emit, privateTable);
        }

        #endregion


        #region Challenges

        void NewChallenges(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {New_challenges_Event} , \n Data : {s}  </color>");
            JsonData jd = JsonMapper.ToObject(s);

            if ((bool)jd["is_active"])
            {
                NewChallengesResponse newChallenges = JsonMapper.ToObject<NewChallengesResponse>(s);
                Debug.Log("Challange1:" + newChallenges.message);
                LudoModesTableList.instance.ChallengeTables(newChallenges);
            }
        }
        void UserCountChallenges(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {User_count_challenge_Event} , \n Data : {s}  </color>");
        }
        void ChallangesClose(object obj)
        {
            string s = JsonMapper.ToJson(obj);
            Debug.Log($"<color=#eb6b34> Reponse: {Challenge_close_Event} , \n Data : {s}  </color>");

            JsonData jd = JsonMapper.ToObject(s);
            if (!(bool)jd["is_active"])
            {
                LudoModesTableList.instance.ClosedChallenge(jd["room_id"].ToString());
            }
        }
        #endregion

    }




    ///////////////////////////Emit Module Class////////////////////////////////

    #region RoomJoin Emit
    [Serializable]
    public class RoomJoinDetailsEmit
    {
        public string player_id;
        public string operator_id;
        public string game_code;
        public string session_token;
        public string room_id;
        public string room_code;
    }
    #endregion
    //..................................
    #region PlayerList Emit
    [Serializable]
    public class PlayerListEmit
    {
        public string player_id;
        public string session_id;
        public string room_id;
        public string room_code;
    }
    #endregion
    ////...............................................

    #region DiceRolled Emit
    [Serializable]
    public class DiceRolledEmit
    {
        public string player_id;
        public string session_id;
        public string room_code;
    }
    #endregion
    ////...............................................

    #region LeaveRequest Emit
    [Serializable]
    public class LeaveRequestEmit
    {
        public string game_code;
        public string operator_id;
        public string player_id;
        public string room_code;
        public string room_id;
        public string session_id;
    }
    #endregion
    ////...............................................

    #region Selected Pawn Emit
    [Serializable]
    public class SelectedPawnEmit
    {
        public string session_id;
        public string player_id;
        public string room_code;
        public int pawn_number;
        public string house_color;
        public int diceNumber;
        public string key;
    }
    #endregion
    ////...............................................
    ///
    #region Reconnect User Emit
    [Serializable]
    public class ReconnectUserEmit
    {
        public string player_id;
        public string operator_id;
        public string game_code;
        public string session_token;
        public string room_id;
        public string session_id;
        public string room_code;
    }
    #endregion
    ////...............................................
    ///
     #region Join Private Table Emit
    [Serializable]
    public class JoinPrivateTable
    {
        public string player_id;
        public string operator_id;
        public string game_code;
        public string session_token;
        public string room_id;
        public string room_code;
        public string join_code;
    }
    #endregion
    ////...............................................
    public class DeviceInfo
    {
        public string player_id;
        public string operator_id;
    }

    
}
