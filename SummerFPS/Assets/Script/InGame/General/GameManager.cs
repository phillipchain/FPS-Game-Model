

using System.Collections;
using System.IO;
using Com.LGUplus.Homework.Minifps.Utills;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Script.Game;
using UnityEngine;

using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class GameManager : MonoBehaviourPunCallbacks
{
        public static GameManager Instance = null;

        public Text infoText;
        public Text gameStatusText;
        public Text gameTimeText;
        public Text monsterHPText;
        
        private float time_current;
        private bool isEnded;
        
        public float gameTime = 60f;
        public float resutOpenningTime = 3.0f;

        #region UNITY

        public void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (isEnded)
                return;
            CheckTimer();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public void Start()
        {
            Hashtable props = new Hashtable
            {
                {SummerFPSGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            
            ResetTimer();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        #endregion

        #region COROUTINES

    
        private IEnumerator EndOfGame(string winner, int remainMonsterHP)
        {
            while (resutOpenningTime > 0.0f)
            {
                string winnerMessage = " Lose the game"; 
                if (remainMonsterHP > 0)
                {
                    winnerMessage = "Lose the game";
                }
                else
                {
                    winnerMessage = "Win the game";
                }
                
                infoText.text = string.Format("Player {0}  {3} with {1} monster remain HPs.\n\n\nReturning to login screen in {2} seconds.",
                    winner, remainMonsterHP, resutOpenningTime.ToString("n2") , winnerMessage);

                yield return new WaitForEndOfFrame();

                resutOpenningTime -= Time.deltaTime;
            }

            gameStatusText.text = CommonUtils.GetStringMessage("게임 상태 :", SummerFPSGame.FINISH_GAME);

            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            CommonUtils.LoadScene("TitleScene");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                //need to enemy
               // StartCoroutine(SpawnAsteroid());
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckEndOfGame();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(AsteroidsGame.PLAYER_LIVES))
            {
                CheckEndOfGame();
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            
            int startTimestamp;
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(AsteroidsGame.PLAYER_LOADED_LEVEL))
            {
                
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        gameStatusText.text = CommonUtils.GetStringMessage("게임 상태", SummerFPSGame.PREPARE_GAME); 
                        CountdownTimer.SetStartTime();
                    }
                }
                else
                {
                    infoText.text = "Waiting for other players...";
                }
            }
        
        }

        #endregion
        
        private void StartGame()
        {
            
            gameStatusText.text = CommonUtils.GetStringMessage("게임 상태 :", SummerFPSGame.START_GAME);
            MakePlayerManager();
            
        }

        private static void MakePlayerManager()
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    
        private bool CheckAllPlayerLoadedLevel()
        {
            gameStatusText.text = CommonUtils.GetStringMessage("게임 상태 :", SummerFPSGame.CHECK_LOADING);
            
            
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(SummerFPSGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool) playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        private void CheckEndOfGame()
        {
            bool allDestroyed = true;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object lives;
                if (p.CustomProperties.TryGetValue(SummerFPSGame.PLAYER_LIVES, out lives))
                {
                    if ((int) lives > 0)
                    {
                        allDestroyed = false;
                        break;
                    }
                }
            }

            if (allDestroyed)
            {
                finishGame();
            }
        }

        private void finishGame()
        {
            Debug.Log("MonsterHP " +monsterHPText.text);
            
            var finalResult = GetFinalResult();
            
            if (PhotonNetwork.IsMasterClient)
            {
                StopAllCoroutines();
            }

            string winner = "";
            int monsterHP = -1;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.GetScore() > monsterHP)
                {
                    winner = p.NickName;
                    monsterHP = finalResult;
                }
            }

            StartCoroutine(EndOfGame(winner, monsterHP));
        }

        private int GetFinalResult()
        {
            string remainMonsterHP =
                monsterHPText.text.ToString().Substring(monsterHPText.text.ToString().IndexOf(":") + 1);
            int finalResult = int.Parse(remainMonsterHP);
            return finalResult;
        }

        private void OnCountdownTimerIsExpired()
        {
            StartGame();
        }
        
        private void End_Timer()
        {
            time_current = 0;
            gameTimeText.text = CommonUtils.GetStringMessage("게임 시간 :" , $"{time_current:N1}") ;
            isEnded = true;
            finishGame();
        }
        
        private void ResetTimer()
        {
            time_current = gameTime;
            gameTimeText.text = gameTimeText.text = CommonUtils.GetStringMessage("게임 시간 :" , $"{time_current:N1}") ;
            isEnded = false;
            
        }
        
        private void CheckTimer()
        {
            if (0 < time_current)
            {
                time_current -= Time.deltaTime;
                gameTimeText.text = gameTimeText.text = CommonUtils.GetStringMessage("게임 시간 :" , $"{time_current:N1}") ;
            }
            else if (!isEnded)
            {
                End_Timer();
            }
        }
}
