namespace ChatServer
{
    public class Message
    {
        public int Id { get; set; }
        public int Id_chat {  get; set; }
        public string Text { get; set; } = string.Empty;
        public int Id_sender { get; set; }
    }
}
