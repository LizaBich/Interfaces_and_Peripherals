namespace Lab8_GlobalHooks
{
    internal class ConfigFile
    {
        public string Email { get; set; }

        private long _fileSize;

        public long FileSize
        {
            get => _fileSize;
            set => _fileSize = value >= 100 ? value : 100;
        }

        public bool HideAfterRun { get; set; }
    }
}
