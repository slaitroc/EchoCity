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

        #region Enums

        /// <summary>
        /// Type of puzzle item this pickable represents
        /// </summary>
        public enum PuzzleItemType
        {
            None,
            Cable,
            FloppyDisk,
            CommunicationDevice,
            MetalTool
        }

        #endregion

        #region Serialized Fields

        [Header("Puzzle Integration")]
        [Tooltip("Type of puzzle item this pickable represents")]
        [SerializeField] private PuzzleItemType puzzleItemType = PuzzleItemType.None;

        [Header("References")]
        [SerializeField] private BunkerPuzzleController puzzleController;

        #endregion

        #region Private Fields

        private bool _hasNotifiedPuzzle = false;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (puzzleController == null && puzzleItemType != PuzzleItemType.None)
            {
                puzzleController = FindFirstObjectByType<BunkerPuzzleController>();
            }
        }

        #endregion

        #region Pickable Override

        public override void Interact()
        {
            base.Interact();

            // Notify puzzle controller when player interacts with the item
            // This happens when the player attempts to pick up the item
            if (!_hasNotifiedPuzzle && puzzleItemType != PuzzleItemType.None && puzzleController != null)
            {
                _hasNotifiedPuzzle = true;
                NotifyPuzzleController(puzzleItemType);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Notifies the puzzle controller when a specific item is picked up
        /// </summary>
        private void NotifyPuzzleController(PuzzleItemType itemType)
        {
            if (puzzleController == null)
            {
                return;
            }

            // Call appropriate puzzle controller method based on item type
            switch (itemType)
            {
                case PuzzleItemType.Cable:
                    puzzleController.OnCablePickedUp();
                    break;

                case PuzzleItemType.FloppyDisk:
                    puzzleController.OnFloppyPickedUp();
                    break;

                case PuzzleItemType.CommunicationDevice:
                    puzzleController.OnCommsDevicePickedUp();
                    break;

                case PuzzleItemType.MetalTool:
                    puzzleController.OnMetalToolPickedUp();
                    break;

                case PuzzleItemType.None:
                default:
                    break;
            }
        }

        #endregion
    }
}

