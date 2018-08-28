namespace DbGenerator.Args
{
    public class CreateArgs
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ScriptDirectory { get; set; }
        public string ReadonlyPassword { get; set; }
    }
}
