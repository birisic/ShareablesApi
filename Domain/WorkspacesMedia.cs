namespace Domain
{
    public class WorkspacesMedia
    {
        public int WorkspaceId { get; set; }
        public int MediaId { get; set; }

        public Media Media { get; set; }
        public Workspace Workspace { get; set; }
    }
}
