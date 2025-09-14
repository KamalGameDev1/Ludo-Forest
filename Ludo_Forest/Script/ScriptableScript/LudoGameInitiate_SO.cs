using System;
using UnityEngine;

namespace LudoMGP
{
    [CreateAssetMenu(fileName = "LudoGameInitiate_SO", menuName = "LudoGame/LudoGameInitiate_Details")]
    public class LudoGameInitiate_SO : ScriptableObject
    {
        public GameInitData AllData;
    }
    [Serializable]
    public class GameInitData
    {
        public bool status;
        public string message;
        public GSdata data;
    }

    [Serializable]
    public class GSdata
    {
        public userinfo userinfo;
    }
    [Serializable]
    public class userinfo
    {
        // data
        public string currency;
        public string currency_Symbol;
        public string game_code;
        public string operator_name;
        public string player_id;
        public string player_nickname;       
        public double wallet;
        public string profile_pic;
        public string session_token;
        public string auth_token;
    }
}