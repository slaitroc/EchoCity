namespace EchoCity
{
    public struct DialogData
    {
        private readonly DialogLines[] dialogLines;
        public readonly DialogLines[] DialogLines => dialogLines;

        public DialogData(SODialogContainer dialogContainer)
        {
            this.dialogLines = dialogContainer.DialogLines;
        }
    }
}
