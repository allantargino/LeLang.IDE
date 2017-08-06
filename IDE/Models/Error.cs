namespace IDE
{
    public class Error
    {
        public ErrorCategory Category { get; set; }
        public int Code { get; set; }
        public int Line { get; set; }
        public string Message { get; set; }
    }
}
