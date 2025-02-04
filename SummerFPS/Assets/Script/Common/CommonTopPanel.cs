﻿using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Jinyoung.dev.summerfps.common
{
    public class CommonTopPanel : MonoBehaviour
    {
        private readonly string connectionStatusMessage = "Connection Status:";

        [Header("UI References")]
        public Text ConnectionStatusText;

        #region UNITY

        public void Update()
        {
            //Continuously check server status
            ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
        }

        #endregion
    }
}