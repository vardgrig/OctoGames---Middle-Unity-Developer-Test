namespace UI.Popup
{
    public interface IPopupManager
    {
        void Show(IPopupConfig config);
        void Hide();
    }
}
