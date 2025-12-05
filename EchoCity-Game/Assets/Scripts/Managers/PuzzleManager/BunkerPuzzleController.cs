using UnityEngine;

namespace EchoCity
{
    /// <summary>
    /// Central puzzle controller for the bunker puzzle in Noah's Lab level.
    /// Tracks puzzle state and coordinates interactions between puzzle elements.
    /// </summary>
    public class BunkerPuzzleController : MonoBehaviour
    {
        #region Constants
        private const string LOG_TAG = "BUNKER PUZZLE CONTROLLER";
        private const string LOG_COLOR = "#9b59b6ff";
        #endregion

        #region Serialized Fields

        [Header("Puzzle State")]
        [SerializeField] private bool hasCable = false;
        [SerializeField] private bool cablePlugged = false;
        [SerializeField] private bool floppyEjected = false;
        [SerializeField] private bool hasFloppy = false;
        [SerializeField] private bool hasCommsDevice = false;
        [SerializeField] private bool hasMetalTool = false;

        [Header("Powered Devices")]
        [Tooltip("Devices that should activate when power is restored")]
        [SerializeField] private PoweredDeviceInteractable[] poweredDevices;

        [Header("Invoking Events")]
        [SerializeField] private SOEventVoid puzzleCompletedEvent;

        #endregion

        #region Properties

        /// <summary>
        /// Whether the player has picked up the cable
        /// </summary>
        public bool HasCable => hasCable;

        /// <summary>
        /// Whether the cable has been plugged into the correct socket
        /// </summary>
        public bool CablePlugged => cablePlugged;

        /// <summary>
        /// Whether the floppy disk has been ejected from the reader
        /// </summary>
        public bool FloppyEjected => floppyEjected;

        /// <summary>
        /// Whether the player has picked up the floppy disk
        /// </summary>
        public bool HasFloppy => hasFloppy;

        /// <summary>
        /// Whether the player has picked up a communication device (walkie-talkie or satellite phone)
        /// </summary>
        public bool HasCommsDevice => hasCommsDevice;

        /// <summary>
        /// Whether the player has picked up the metal bar tool
        /// </summary>
        public bool HasMetalTool => hasMetalTool;

        #endregion

        #region Puzzle Event Methods

        /// <summary>
        /// Called when the player picks up the cable from the medic room
        /// </summary>
        public void OnCablePickedUp()
        {
            if (hasCable)
            {
                Log.W("Cable already picked up", LOG_COLOR, LOG_TAG);
                return;
            }

            hasCable = true;
            Log.D("Cable picked up", LOG_COLOR, LOG_TAG);
        }

        /// <summary>
        /// Called when the player plugs the cable into the correct socket
        /// </summary>
        public void OnCablePluggedIn()
        {
            if (cablePlugged)
            {
                Log.W("Cable already plugged in", LOG_COLOR, LOG_TAG);
                return;
            }

            if (!hasCable)
            {
                Log.W("Cannot plug in cable - player doesn't have cable", LOG_COLOR, LOG_TAG);
                return;
            }

            cablePlugged = true;
            Log.D("Cable plugged in - power restored", LOG_COLOR, LOG_TAG);

            // Activate all powered devices
            OnPowerRestored();
        }

        /// <summary>
        /// Called when power is restored. Enables all powered devices.
        /// </summary>
        private void OnPowerRestored()
        {
            if (poweredDevices != null)
            {
                foreach (var device in poweredDevices)
                {
                    if (device != null)
                    {
                        device.OnPowerRestored();
                    }
                }
            }
        }

        /// <summary>
        /// Called when the floppy disk is ejected from the correct reader
        /// </summary>
        public void OnFloppyEjected()
        {
            if (floppyEjected)
            {
                Log.W("Floppy disk already ejected", LOG_COLOR, LOG_TAG);
                return;
            }

            if (!cablePlugged)
            {
                Log.W("Cannot eject floppy disk - no power", LOG_COLOR, LOG_TAG);
                return;
            }

            floppyEjected = true;
            Log.D("Floppy disk ejected", LOG_COLOR, LOG_TAG);
        }

        /// <summary>
        /// Called when the player picks up the floppy disk
        /// </summary>
        public void OnFloppyPickedUp()
        {
            if (hasFloppy)
            {
                Log.W("Floppy disk already picked up", LOG_COLOR, LOG_TAG);
                return;
            }

            if (!floppyEjected)
            {
                Log.W("Cannot pick up floppy disk - not ejected yet", LOG_COLOR, LOG_TAG);
                return;
            }

            hasFloppy = true;
            Log.D("Floppy disk picked up", LOG_COLOR, LOG_TAG);
            CheckPuzzleCompletion();
        }

        /// <summary>
        /// Called when the player picks up a communication device (walkie-talkie or satellite phone)
        /// </summary>
        public void OnCommsDevicePickedUp()
        {
            if (hasCommsDevice)
            {
                Log.W("Communication device already picked up", LOG_COLOR, LOG_TAG);
                return;
            }

            hasCommsDevice = true;
            Log.D("Communication device picked up", LOG_COLOR, LOG_TAG);
            CheckPuzzleCompletion();
        }

        /// <summary>
        /// Called when the player picks up the metal bar tool
        /// </summary>
        public void OnMetalToolPickedUp()
        {
            if (hasMetalTool)
            {
                Log.W("Metal tool already picked up", LOG_COLOR, LOG_TAG);
                return;
            }

            hasMetalTool = true;
            Log.D("Metal tool picked up", LOG_COLOR, LOG_TAG);
            CheckPuzzleCompletion();
        }

        /// <summary>
        /// Checks if the player can force open the exit door.
        /// Requires: floppy disk, communication device, and metal tool.
        /// </summary>
        public bool CanForceExitDoor()
        {
            bool canExit = hasFloppy && hasCommsDevice && hasMetalTool;
            
            if (canExit)
            {
                Log.D("Puzzle completed - player can exit", LOG_COLOR, LOG_TAG);
            }
            
            return canExit;
        }

        /// <summary>
        /// Checks if the puzzle is completed and raises completion event if so
        /// </summary>
        private void CheckPuzzleCompletion()
        {
            if (CanForceExitDoor())
            {
                puzzleCompletedEvent?.RaiseEvent();
                Log.D("Puzzle completed!", LOG_COLOR, LOG_TAG);
            }
        }

        #endregion
    }
}

