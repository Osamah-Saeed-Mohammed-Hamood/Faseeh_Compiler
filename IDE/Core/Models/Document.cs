namespace Faseeh.IDE.Core.Models
{
    public class Document
    {
        public string FilePath { get; set; }
        public string Content { get; set; }
        public bool IsModified { get; set; }
    }
} 