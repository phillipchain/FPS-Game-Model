using UnityEngine;

namespace Script.Game
{
    public class SummerFPSGame
    {
        public const string CHECK_LOADING = "Check loading";
        public const string PREPARE_GAME = "Prepare Game";
        public const string START_GAME = "Game Start";
        public const string FINISH_GAME = "Game Finish";
        public const string PLAYER_LIVES = "PlayerLives";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
        public const string NOT_PLAYING_GAME = "notPlaying";
        public const string PLAYING_GAME = "playing";
        public const int PLAYER_MAX_LIVES = 3;
        
        public static Color GetColor(int colorChoice)
        {
            switch (colorChoice)
            {
                case 0: return Color.red;
                case 1: return Color.green;
                case 2: return Color.blue;
                case 3: return Color.yellow;
                case 4: return Color.cyan;
                case 5: return Color.grey;
                case 6: return Color.magenta;
                case 7: return Color.white;
            }

            return Color.black;
        }
    }
}