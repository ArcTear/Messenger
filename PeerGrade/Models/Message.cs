using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PeerGrade.Models
{
    /// <summary>
    /// Сообщения.
    /// </summary>
    [DataContract]
    public class Message1
    {
        /// <summary>
        /// тема
        /// </summary>
        [DataMember(Name = "subject")]
        public string Subject { get; set; }
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }
        /// <summary>
        /// Почта отправителя.
        /// </summary>
        [DataMember(Name = "senderId")]
        public string SenderId { get; set; }
        /// <summary>
        /// Почта получателя.
        /// </summary>
        [DataMember(Name = "receiverId")]
        public string ReceiverId { get; set; }
    }
}
