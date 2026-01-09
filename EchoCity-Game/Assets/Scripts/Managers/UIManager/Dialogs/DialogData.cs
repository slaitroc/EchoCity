namespace EchoCity
{
    public struct DialogData
    {
        private readonly DialogLine[] dialogLines;
        public readonly DialogLine[] DialogLines => dialogLines;

        public DialogData(SODialogContainer dialogContainer)
        {
            this.dialogLines = dialogContainer.DialogLines;
        }
    }
}
