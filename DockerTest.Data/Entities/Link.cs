using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerTest.Data.Entities
{
    [Table("Links")]
    public class Link
    {
        public int Id { get; set; }

        /// <summary>
        /// Href ссылки
        /// </summary>
        [MaxLength(50)]
        public string Href { get; set; }

        public LinkStatus LinkStatus { get; set; }

        /// <summary>
        /// Возвращенный статус ссылка
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Текущий шаг выполнения
        /// </summary>
        public int CurrentStep { get; set; }

        /// <summary>
        /// Общее количество шагов
        /// </summary>
        public int CountStep { get; set; }

        /// <summary>
        /// Время одного шага (в секундах)
        /// </summary>
        public double Tact { get; set; }
    }

    public enum LinkStatus
    {
        Waiting,
        Queue,
        Processing,
        Done
    }
}