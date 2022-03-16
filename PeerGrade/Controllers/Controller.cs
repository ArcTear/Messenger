using Microsoft.AspNetCore.Mvc;
using PeerGrade.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.Json;

namespace PeerGrade.Controllers
{
    /// <summary>
    /// Контроллер.
    /// </summary>
    [Route("/api/[controller]")]
    public class Controller1 : Controller
    {
        private static Random rnd = new();

        private static List<User> _users = new();
        private static List<Message1> _message1 = new();

        /// <summary>
        /// Random initialization
        /// </summary>
        [HttpPost("Initialization")]
        public IActionResult Post()
        {
            _users.Clear();
            FillUserList();
            FillMessageList();
            _users.Sort();
            SerializationOfUsersAndMessages();
            return Ok(_users);
        }

        [HttpPost("Add new user")]
        public IActionResult Post(string userName, string email)
        {
            if (userName == null || email == null)
            {
                return NoContent();
            }

            _users.Add(new User() { UserName = userName, Email = email });
            _users.Sort();
            SerializationOfUsersAndMessages();
            return Ok(_users);
        }

        /// <summary>
        /// Десериализация данных из файлов Users.json и Messages.json.
        /// </summary>
        /// <returns>Выводит список десериализованных пользователей.</returns>
        [HttpPost("Read data from json")]
        public IActionResult  GetUsersAndMessagesFromJSON()
        {
            using var userStream = new FileStream("Users.json", FileMode.OpenOrCreate, FileAccess.Read);
            {
                var userFormatter = new DataContractJsonSerializer(typeof(List<User>));
                _users = (List<User>)userFormatter.ReadObject(userStream);
            }

            using var messsageStream = new FileStream("Messages.json", FileMode.OpenOrCreate, FileAccess.Read);
            {
                var messsageFormatter = new DataContractJsonSerializer(typeof(List<Message1>));
                _message1 = (List<Message1>)messsageFormatter.ReadObject(messsageStream);
            }

            return Ok(_users);
        }


        /// <summary>
        /// Send custom message
        /// </summary>
        [HttpPost("Send message")]
        public IActionResult Post(string subject, string message, string senderId, string receiverId)
        {
            var senderUser = _users.Find(user => user.UserName.Equals(senderId));
            var receiverUser = _users.Find(user => user.UserName.Equals(receiverId));
            if (senderUser == null || receiverUser == null)
            {
                return NotFound();
            }

            _message1.Add(new Message1 { Message = message, ReceiverId = receiverId, SenderId = senderId, Subject = subject });
            SerializationOfUsersAndMessages();
            return Ok(_message1);
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        [HttpGet("Get users")]
        public IActionResult Get(int Limit, int Offset)
        {
            if (Limit <= 0 || Offset < 0)
                return BadRequest();
            List<User> users = new List<User>();
            for (int i = Offset;  Limit > 0 && i < _users.Count ; i++)
            {
                users.Add(_users[i]);
                --Limit;
            }
            return Ok(users);
        }

        /// <summary>
        /// Gets all messages
        /// </summary>
        [HttpGet("Get messages")]
        public IEnumerable<Message1> GetMessages() => _message1;

        /// <summary>
        /// Получить имя пользователя по его почте.
        /// </summary>
        /// <param name="email">Почта пользователя</param>
        /// <returns>Имя пользователя</returns>
        [HttpGet("Get userName by email")]
        public IActionResult Get(string email)
        {
            var users = _users.SingleOrDefault(user => user.Email.Equals(email));

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        /// <summary>
        /// Gets messages by sender.
        /// </summary>
        /// <param name="senderId">ID of Sender</param>
        [HttpGet("Get messages by sender")]
        public IActionResult GetMessageBySender(string senderId)
        {
            var messages = _message1.FindAll(message => message.SenderId.Equals(senderId));

            if (messages == null)
            {
                return NotFound();
            }
                
            return Ok(messages);
        }

        /// <summary>
        /// Gets messages by receiver.
        /// </summary>
        /// <param name="receiverId">ID of Receiver</param>
        [HttpGet("Get message by receiver")]
        public IActionResult GetMessageByReceiver(string receiverId)
        {
            var messages = _message1.FindAll(message => message.ReceiverId.Equals(receiverId));

            if (messages == null)
            {
                return NotFound();
            }

            return Ok(messages);
        }

        /// <summary>
        /// Gets messages by a specific sender and receiver.
        /// </summary>
        /// <param name="receiverId">ID of Receiver</param>
        /// <param name="senderId">ID of Sender</param>
        /// <returns></returns>
        [HttpGet("Get message by receiver and sender")]
        public IActionResult GetMessageByReceiverAndSender(string receiverId, string senderId)
        {
            var messages = _message1.FindAll(message => message.ReceiverId.Equals(receiverId) && message.SenderId.Equals(senderId));

            if (messages == null)
            {
                return NotFound();
            }

            return Ok(messages);
        }

        /// <summary>
        /// Сериализация Пользователей и Сообщений.
        /// </summary>
        private void SerializationOfUsersAndMessages()
        {
            var jsonStringUsers = JsonSerializer.Serialize(_users);
            System.IO.File.WriteAllText("Users.json", jsonStringUsers);
            var jsonStringMessages = JsonSerializer.Serialize(_message1);
            System.IO.File.WriteAllText("Messages.json", jsonStringMessages);
        }

        /// <summary>
        /// Создает массив, заполненный случайными пользоваетлями.
        /// </summary>
        /// <returns>Созданный массив</returns>
        private void FillUserList()
        {
            int quantity = rnd.Next(3, 11);
            for (int i = 0; i < quantity; i++)
            {
                _users.Add(new User() { UserName = GetRandomText(4, 11), Email = GetRandomText(4, 11) + "@yandex.ru" });
            }
        }

        /// <summary>
        /// Создает массив, заполненный случайными сообщениями.
        /// </summary>
        /// <returns>Созданный массив</returns>
        private void FillMessageList()
        {
            int quantity = rnd.Next(3, 11);
            for (int i = 0; i < quantity; i++)
            {
                _message1.Add(new Message1() { Message = GetRandomText(10, 20), ReceiverId = _users[rnd.Next(0, _users.Count)].Email, SenderId = _users[rnd.Next(0, _users.Count)].Email, Subject = GetRandomText(5, 10) });
            }
        }

        /// <summary>
        /// Генерирует случайный текст из букв и цифр.
        /// </summary>
        /// <param name="startLength">длина, от которой происходит генерация</param>
        /// <param name="endLength">длина, до которой происходит генерация</param>
        /// <returns>Созданный текст</returns>
        private static string GetRandomText(int startLength, int endLength)
        {
            string text = string.Empty;
            int quantity = rnd.Next(startLength, endLength);
            for (int i = 0; i < quantity; i++)
            {
                if (rnd.Next(2) == 0)
                {
                    text += (char)rnd.Next('a', 'z' + 1);
                }
                else
                {
                    text += (char)rnd.Next('0', '9' + 1);
                }
            }
            return text;
        }
    }
}
