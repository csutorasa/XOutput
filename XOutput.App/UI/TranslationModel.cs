namespace XOutput.App.UI
{
    public class TranslationModel : ModelBase
    {
        public static TranslationModel Instance => instance;
        private static TranslationModel instance = new TranslationModel();

        private string language;
        public string Language
        {
            get => language;
            set
            {
                if (language != value)
                {
                    language = value;
                    OnPropertyChanged(nameof(Language));
                }
            }
        }

    }
}
