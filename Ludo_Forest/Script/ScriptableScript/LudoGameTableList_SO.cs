using System.Collections.Generic;
using System;
using UnityEngine;

namespace LudoMGP
{
    [CreateAssetMenu(fileName = "LudoGameTableList",menuName = "LudoGame/LudoGameTableList_Details")]
    public class LudoGameTableList_SO : ScriptableObject
    {
        public LudoTableListData AllData;
    }
    [Serializable]
    public class LudoTableListData
    {
        public bool status;
        public int sub_code;
        public string message;
        public Data data;
        public List<object> myroom;
        public SocketConfig socket_config;
    }

    [Serializable]
    public class SocketConfig
    {
        public string ip;
        public string port;
        public string server_uses;
        public string server_type;
        public string room_name;
    }
    [Serializable]
    public class Data
    {
        public List<Classic> classic;
        public List<object> challanges;
        public List<Timer> timer;
    }
    [Serializable]
    public class Classic
    {
        public PrizeDistribution prize_distribution;
        public string[] game_varient;
        public string game_type;
        public int game_time;
        public int max_player;
        public int min_player;
        public RakeDetails rake_details;    
        public string entry_fee;
        public string tablename;
        public int network_player_join_timer;
        public string network_player_presit;

        public string max_network_player;
        public int auto_play;
        public int geo_restrication;
        public int allow_network;
        public int is_active;
        public string game_break_countdown;
        public object level;
        public int is_live;
        public List<int> dice_open;
        public int max_winner;
        public string created_by;
        public string modified_by;
        public DateTime dt_creation;
        public DateTime dt_modification;
        public string _id;
        public string user_type;
        public string game_start_countdown;       
    }



    [Serializable]
    public class Timer
    {
        public PrizeDistribution prize_distribution;
        public string[] game_varient;
        public string game_type;
        public int game_time;
        public int max_player;
        public int min_player;
        public RakeDetails rake_details;
        public string entry_fee;
        public string tablename;
        public int network_player_join_timer;
        public string network_player_presit;
        public string max_network_player;
        public int auto_play;
        public int geo_restrication;
        public int allow_network;
        public int is_active;
        public string game_break_countdown;
        //public string level;
        public int is_live;
        public List<int> dice_open;
        public int max_winner;
        public string created_by;
        public string modified_by;
        public DateTime dt_creation;
        public DateTime dt_modification;
        public string _id;

       
    }
    [Serializable]
    public class PrizeDistribution
    {
        public int first_winner;
        public int? second_winner;
    }
    [Serializable]
    public class RakeDetails
    {
        public object rake;
    }
}