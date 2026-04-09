namespace UI.Popup
{
    public interface IPopupConfig
    {
        string Title { get; }
        string Body { get; }
        PopupButtonConfig[] Buttons { get; } // 1–5 entries
    }
}
