﻿using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VRC.SDKBase;

namespace MarbleRace.Scripts
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BetScreen : UdonSharpBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("Money earned by guessing correctly. Default to -1$ if no value is given.")]
        private int[] payouts = { 3, 2, 1};
            
        [Header("References")]
        [SerializeField] private BetButton[] betButtons;
        [SerializeField] private TextMeshProUGUI textStatus;
        [SerializeField] private TextMeshProUGUI textPayoutInfo;
        [SerializeField] private Animator animator;

        /// <summary>
        /// 0 = Hidden
        /// 1 = Betting open (no timer)
        /// 2 = Betting open (timer running)
        /// 3 = Betting over (timer ran out), show payout if available
        /// 4 = Game is over, show payout if available
        /// </summary>
        [UdonSynced, FieldChangeCallback(nameof(State))]private int state;

        private uint bettingTime;

        private void OnEnable()
        {
            OnStateChanged();
        }

        public int State
        {
            get => state;
            set
            {
                if (State == value) return;
                state = value;
                OnStateChanged();
            }
        }

        private void OnStateChanged()
        {
            animator.SetInteger("State", State);
            var isBettingClosed = State == 0 || State >= 3; 
            LockAllButtons(isBettingClosed);
            switch (State)
            {
                case 0:
                case 1:
                    Reset();
                    break;
                case 2:
                    _StartBettingTimer();
                    break;
            }
            UpdateStatusText();
            if (Networking.IsMaster) RequestSerialization();
        }

        private uint bettingTimer;

        private void _StartBettingTimer()
        {
            if (bettingTimer > 0)
            {
                Debug.LogWarning("Marble Race: Betting timer was started while it was already running!");
                return;
            }

            bettingTimer = bettingTime;
            _UpdateBettingTimer();
        }
        
        public void _UpdateBettingTimer()
        {
            UpdateStatusText();
            if (State != 2) return;
            if (bettingTimer == 0)
            {
                if (Networking.IsMaster) State = 3;
                return;
            }
            
            bettingTimer--;
            
            SendCustomEventDelayedSeconds(nameof(_UpdateBettingTimer), 1f);
        }

        private RaceManager raceManager;
        
        /// <summary>
        /// Index of the marble the local player has bet on.
        /// </summary>
        private sbyte betOnMarbleIndex = -1;

        public void _Setup(RaceManager manager, uint timeToBet)
        {
            raceManager = manager;
            bettingTime = timeToBet;
            SetupPayoutInfo();
        }

        private void SetupPayoutInfo()
        {
            var result = "Payouts";
            for (sbyte placementIndex = 0; placementIndex <= payouts.Length; placementIndex++)
            {
                var payout = GetPayout(placementIndex);
                var placementText = _GetPlacementString(placementIndex);
                result += $"<br>{placementText}\t{payout}$";
            }

            textPayoutInfo.text = result;
        }

        public void _SetupButton(int buttonIndex, string marbleName, Color marbleColor)
        {
            betButtons[buttonIndex]._Setup(
                this,
                (sbyte) buttonIndex,
                marbleName,
                marbleColor);
            UpdateStatusText();
        }

        public void _Press(sbyte index)
        {
            if (State == 0)
            {
                Debug.Log("Marble Race: Bets have not yet started, but player tried to bet.");
                return;
            }
            
            if (State == 3)
            {
                Debug.Log("Marble Race: Betting is locked, but player tried to bet.");
                return;
            }

            if (betOnMarbleIndex != -1) // Undo previous bet, if betting is still unlocked
            {
                betButtons[betOnMarbleIndex].HasPlacedBet = false;
            }
            
            betOnMarbleIndex = index;
            betButtons[index].HasPlacedBet = true;

            raceManager.OnBetPlaced();
        }

        private void LockAllButtons(bool b)
        {
            foreach (var button in betButtons)
            {
                button._SetIsLocked(b);
            }
        }

        public void _SetPlacement(sbyte marbleIndex, sbyte placement)
        {
            betButtons[marbleIndex]._SetPlacement(placement, GetPayout(placement));
        }

        private void UpdateStatusText()
        {
            switch (State)
            {
                case 0:
                    textStatus.text = "Not<br>started";
                    break;
                case 1:
                    textStatus.text = "Click<br>to bet!";
                    break;
                case 2:
                    textStatus.text = $"{bettingTimer - 1}s<br>to bet!";
                    break;
                case 3:
                    textStatus.text = "<i>Bets<br>closed</i>";
                    break;
                case 4:
                    if (betOnMarbleIndex == -1)
                    {
                        textStatus.text = "<i>No<br>bet</i>";
                        return;
                    }

                    var placement = raceManager.RacePlacement[betOnMarbleIndex];
                    var payout = GetPayout(placement);
                    var prefix = "<color=" + (payout > 0 ? "green>+" : payout == 0? "white>" : "red>");
                    textStatus.text = $"{_GetPlacementString(placement)}<br>{prefix}{payout}$</color>";
                    break;
            }
        }

        private void Reset()
        {
            betOnMarbleIndex = (sbyte) -1;
            foreach (var button in betButtons)
            {
                button._ClearPlacement();
                button.HasPlacedBet = false;
            }
            RequestSerialization();
        }

        public void _StartBettingWithoutTimer()
        {
            if (State != 0)
            {
                Debug.LogWarning("Marble Race: Bet screen was not properly reset!");
            }

            State = 1;
        }

        public void _StartBetting()
        {
            if (state >= 2)
            {
                Debug.LogWarning("Marble Race: Bet screen was in an improper state when betting was started.");
            }
            
            State = 2;
        }

        public int _GetPayout(sbyte[] placements)
        {
            if (betOnMarbleIndex == -1) return 0;
            var placement = placements[betOnMarbleIndex];
            return GetPayout(placement);
        }

        private int GetPayout(sbyte placement)
        {
            if (placement < 0 ||placement >= payouts.Length) return -1;
            return payouts[placement];
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!Networking.IsMaster) return;
            if (State == 0) State = 2;
        }

        public void _ShowResultScreen()
        {
            if (State > 0) State = 4;
            else if (State == 4) OnStateChanged(); // Might need to show final placement for late finishes
        }

        public string _GetPlacementString(sbyte n)
        {
            var result = (n + 1).ToString();
            switch (n)
            {
                case -1:
                    return "<color=white>Lost!</color>"; // Happens when marble hasn't finished but race is over
                case 0:
                    return $"<color=yellow>{result}st</color>";
                case 1:
                    return $"<color=white>{result}nd</color>";
                case 2:
                    return $"<color=orange>{result}rd</color>";
                default:
                    return $"<color=red>{result}th</color>";
            }
        }
    }
}
