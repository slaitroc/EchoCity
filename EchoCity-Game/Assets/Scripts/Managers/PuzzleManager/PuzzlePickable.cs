using UnityEngine;
using EchoCity.Interactables;

namespace EchoCity
{
    /// <summary>
    /// Extended Pickable component that connects to the bunker puzzle controller.
    /// Automatically notifies the puzzle controller when specific items are picked up.
    /// </summary>
    public class PuzzlePickable : Pickable
    {
        #region Constants
        protected override string _TYPE_LOG_TAG => "PUZZLE_PICKABLE";
        #endregion

        #region Serialized Fields

        [Header("Puzzle Integration")]
        [Tooltip("Whether this pickable is part of the bunker puzzle")]
        [SerializeField] private bool isPuzzleItem = true;

        [Tooltip("Item names that should trigger puzzle events")]
        [SerializeField] private string[] puzzleItemNames = { "Loose Cable", "Kael's Research Data", "Walkie-Talkie", "Satellite Phone", "Metal Bar" };

        [Header("References")]
        [SerializeField] private BunkerPuzzleController puzzleController;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (puzzleController == null && isPuzzleItem)
            {
                puzzleController = FindObjectOfType<BunkerPuzzleController>();
            }
        }

        #endregion

        #region Pickable Override

        public override void Interact()
        {
            base.Interact();

            // Check if this is a puzzle item and notify controller
            if (isPuzzleItem && puzzleController != null && pickableData != null)
            {
                NotifyPuzzleController(pickableData.PickableName);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Notifies the puzzle controller when a specific item is picked up
        /// </summary>
        private void NotifyPuzzleController(string itemName)
        {
            if (puzzleController == null || string.IsNullOrEmpty(itemName))
            {
                return;
            }

            // Check item name and call appropriate puzzle controller method
            switch (itemName)
            {
                case "Loose Cable":
                    puzzleController.OnCablePickedUp();
                    break;

                case "Kael's Research Data":
                    puzzleController.OnFloppyPickedUp();
                    break;

                case "Walkie-Talkie":
                case "Satellite Phone":
                    puzzleController.OnCommsDevicePickedUp();
                    break;

                case "Metal Bar":
                    puzzleController.OnMetalToolPickedUp();
                    break;

                default:
                    // Check if item name is in the puzzle item names array
                    foreach (string puzzleItemName in puzzleItemNames)
                    {
                        if (itemName == puzzleItemName)
                        {
                            // Generic puzzle item picked up - you can add specific handling here
                            Log.D($"Puzzle item picked up: {itemName}", _LOG_COLOR, _LOG_TAG_FULL);
                            break;
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}

