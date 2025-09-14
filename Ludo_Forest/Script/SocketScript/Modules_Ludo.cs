
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudoMGP
{

    // ------------------------------------- Response Modules--------------------------------------------------------------

    #region RoomCodeInfo Response
    [Serializable]
    public class RoomCodeInfoResponse
    {
        public bool status;
        public string message;
        public string room_code;
        public string room_id;

    }
    #endregion
    //..................................
    //..................................

    #region PlayerInfo Response
    [Serializable]
    public class PlayerInfoResponse
    {

        public string player_id;
        public string player_nickname;
        public string players_avtar_url;
        public double wallet_balance;
        public int seat_number;
        public string room_id;
        public string room_code;
        public string house_color;
        public string player_status;
        public string turn_miss_in_game;
        public bool active;
        
    }
    #endregion
    //..................................

    #region PrepTimer Response
    [Serializable]
    public class PrepTimerResponse
    {
        public int preparation_time;
        public string session_id;
        public string room_code;
    }
    #endregion

    //..................................
    //..................................

    #region GameStarted Response
    [Serializable]
    public class GameStartedResponse
    {
        public string room_id;
        public string room_code;
        public string session_id;
        public string game_type;
        public string game_varient;
        public List<Player> players;
    }

    [Serializable]
    public class Player
    {
        public string player_id;
    }
    #endregion

    //..................................
    #region PlayerTurn Response
    [Serializable]
    public class PlayerTurnResponse
    {
        public bool status;
        public string player_id;
        public string room_code;
        public string session_id;
        public TurnDetails turn_details;
        public int turn_status;
        public List<int> pawn_positions;
        public int game_remaining_time;
        public string message;
        public bool extra_move_status;
    }

    [Serializable]
    public class TurnDetails
    {
        public string start_time;
        public string end_time;
        public int total_turn_time;
        public int remaining_time;
    }
    #endregion


    //..................................
    #region TurnMissed Response
    public class TurnMissedResponse
    {
        public string player_id;
        public int missed_turns;
    }
    #endregion

    //..................................
    #region DiceResult Response
    [Serializable]
    public class DiceResultResponse
    {
        public string player_id;
        public int diceNumber;
        public string color;
        public bool key;
        public List<EligiblePawn> eligible_pawns;
        public bool status;
    }

    [Serializable]
    public class EligiblePawn
    {
        public int pawn_number;
        public int current_position;
        public int new_position;
    }
    #endregion

    //..................................
    #region Pawn Move Response
    [Serializable]
    public class PawnMoveResponse
    {
        public string house_color;
        public int pawn_number;
        public int current_position;
        public int new_position;
        public string player_id;
        public int points;
    }
    #endregion

    //..................................
    #region PlayerList Response
    [Serializable]
    public class PlayerListDetail
    {
        public string player_id;
        public string player_nickname;
        public string players_avtar_url;
        public float wallet;
        public int seat_number;
        public string house_color;
        public string operator_player_id;
    }

    #endregion

    //..................................
    #region PawnKilled Response
    [Serializable]
    public class PawnKilledResponse
    {
        public string player_id;
        public string house_color;
        public int player_pawn_number;
        public int player_points;
        public int player_current_position;
        public KilledByPlayerDetails killed_by_player_details;
    }

    [Serializable]
    public class KilledByPlayerDetails
    {
        public string player_id;
        public int player_points;
        public string house_color;
    }

    #endregion

    //..................................
    #region WinnerDeclared Response
    [Serializable]
    public class WinnerDeclaredResponse
    {
        public string player_id;
        public int rank;
        public int score;
    }

    #endregion

    //..................................
    #region FinalWinner Response
    [Serializable]
    public class FinalWinnerResponse
    {
        public string room_code;
        public List<PlayerFinalData> all_player_data;
    }

    [Serializable]
    public class PlayerFinalData
    {
        public string player_id;
        public string player_nickname;
        public string pawn_color;
        public int rank;
        public double wallet;
        public double win_amount;
        public bool isWinner;
        //public double amount;
        public int points;
    }

    #endregion

    //..................................
    #region leaveRequest Response
    [Serializable]
    public class LeaveRequestResponse
    {
        public bool status;
        public string message;
        public string player_id;
        public int operator_id;
        public string game_code;
        public string session_token;
        public string room_id;
        public string room_code;
    }

    #endregion

    //..................................
    #region RoomInfo Response
    [Serializable]
    public class RoomInfoResponse
    {
        public string game_type;
        public string game_varient;
        public int game_time;
        public int min_player;
        public int max_player;
        public string tableName;
        //public string entry_fee;
        public int max_winner;
        public PrizeDistributionResponse prize_distribution;
        public string game_code;
        public string operator_id;
        public string room_id;
        public string room_code;
        public string game_status;
        public string session_id;
        public List<PlayerDetail> player_details;
        public TurnDetailsResponse turn_details;
    }

    [Serializable]
    public class PrizeDistributionResponse
    {
        public int first_winner;
        public int second_winner;
    }

    [Serializable]
    public class PlayerDetail
    {
        public string player_id;
        public string player_nickname;
        public string players_avtar_url;
        public int wallet_balance;
        public int seat_number;
        public string room_id;
        public string room_code;
        public int turn_miss_in_game;
        public string house_color;
        public string player_status;
    }

    [Serializable]
    public class TurnDetailsResponse
    {
        // Currently empty; define fields as needed when used.
    }

    #endregion

    //..................................
    #region GamePlayTime Response
    [Serializable]
    public class GamePlayTimeRespose
    {
        public string session_id;
        public string room_code;
        public string start_time;
        public string end_time;
        public int remaining_time;
    }

    #endregion

    //...../////////////////////.
    //..................................
    #region LeaveRoom Response
    [Serializable]
    public class LeaveRoomRespose
    {
        public bool status;
        public string message;
        public string player_id;
        public string session_id;
        public string room_id;
        public string room_code;
    }

    #endregion

    //..................................Challenges..............................
    //..................................
    #region NewChallenges Response
    [Serializable]
    public class NewChallengesResponse
    {
        public bool is_active;
        public string room_id;
        public string message;
        public TableData tableData;
    }

    [Serializable]
    public class TableData
    {
        public PrizeDistribution prize_distribution;
        public string[] game_varient;
        public string game_type;
        public int game_time;
        public int max_player;
        public int min_player;
        public RakeDetails rake_details;
        public int entry_fee;
        public string tablename;
        public int network_player_join_timer;
        //public string network_player_presit;
        //public string max_network_player;
        public int auto_play;
        public int geo_restrication;
        public int allow_network;
        public int is_active;
        public string game_break_countdown;
        //public string level;
        public int is_live;
        public int[] dice_open;
        public int max_winner;
        public string created_by;
        public string modified_by;
        public string dt_creation;
        public string dt_modification;
        public string _id;
    }

    #endregion

    //..................................
    #region UserCountChallenge Response
    [Serializable]
    public class UserCountChallengeResponse
    {
        public string room_id;
        public string message;
        public int player_count;
    }

    #endregion

    //..................................
    #region ChallengeClose Response
    [Serializable]
    public class ChallengeCloseResponse
    {
        public bool is_active;
        public string room_id;
        public PlayersList players_list;
        public string message;
    }

    [Serializable]
    public class PlayersList
    {
        // Define fields based on actual "players data" structure
    }

    #endregion




    [Serializable]
    public enum PathType_Ludo
    {
        star, normal, starterGreen, starterYellow, starterBlue, starterRed, pathGreen, pathYellow, pathRed, pathBlue
    }

    [Serializable]
    public enum TransactionType
    {
        DEBIT, CREDIT
    }
    ///



}