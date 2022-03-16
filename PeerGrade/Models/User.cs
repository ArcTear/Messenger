using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PeerGrade.Models
{
    /// <summary>
    /// Пользователь.
    /// </summary>
    [DataContract]
    public class User : IComparable<User>
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>

        [DataMember(Name = "userName")]
        public string UserName { get; set; }
        /// <summary>
        /// Email пользователя.
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Метод для сравнения по электронной почте.
        /// </summary>
        public int CompareTo(User other)
        {
            return this.Email.CompareTo(other.Email);
        }
    }
}
