namespace CRIAudio.Container.UTF
{
    public struct UTFInfo
    {
        public uint FileSize { get; set; }
        public uint Version { get; set; }
        public uint TableOffset { get; set; }
        public uint StringOffset { get; set; }
        public uint BinaryOffset { get; set; }
        public uint TableNameOffset { get; set; }
        public uint ColumnCount { get; set; }
        public uint RowWidth { get; set; }
        public uint RowCount { get; set; }

        public string TableName { get; set; }
    }
}
