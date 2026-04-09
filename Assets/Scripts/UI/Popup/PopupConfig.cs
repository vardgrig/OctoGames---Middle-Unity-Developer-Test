namespace UI.Popup
{
    public class PopupConfig : IPopupConfig
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public PopupButtonConfig[] Buttons { get; set; }
    }
}
