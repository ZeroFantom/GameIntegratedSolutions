using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskStatus = IntelliTrackSolutionsWeb.EFModels_Partial.Converter.TaskStatus;

namespace IntelliTrackSolutionsWeb.EFModels;

[PrimaryKey("IdTask", "InformationSystemId")]
[Table("Task")]
[Index("InformationSystemId", Name = "fk_Task_InformationSystem_idx")]
public class Task
{
    [Key] [Column("idTask")] public int IdTask { get; set; }

    /// <summary>
    ///     Идентификатор информационной системы к которой принадлежит задача.
    /// </summary>
    [Key]
    [Display(Name = "Идентификатор игровой платформы")]
    public int InformationSystemId { get; set; }

    /// <summary>
    ///     Название задачи.
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Название")]
    public string Title { get; set; } = null!;

    /// <summary>
    ///     Цель задачи.
    /// </summary>
    [StringLength(400)]
    [Display(Name = "Цель")]
    public string Goal { get; set; } = null!;

    /// <summary>
    ///     Дата регистрации задачи.
    /// </summary>
    [Column(TypeName = "datetime")]
    [Display(Name = "Дата регистрации")]
    public DateTime DataRegistration { get; set; }

    /// <summary>
    ///     Последнее обновление задачи.
    /// </summary>
    [Column(TypeName = "datetime")]
    [Display(Name = "Дата последнего обновления")]
    public DateTime LastUpdate { get; set; }

    /// <summary>
    ///     Статус выполнения задачи.
    /// </summary>
    [Column(TypeName = "enum('Completed','PendingExecution','NotCompleted')")]
    [Display(Name = "Статус  задания")]
    public TaskStatus Status { get; set; } = TaskStatus.PendingExecution;

    /// <summary>
    ///     Срок до которого нужно выполнить задачу.
    /// </summary>
    [Column(TypeName = "datetime")]
    [Display(Name = "Дата крайнего срока")]
    public DateTime Deadline { get; set; }

    [JsonIgnore]
    [ForeignKey("InformationSystemId")]
    [InverseProperty("Tasks")]
    public virtual InformationSystem? InformationSystem { get; set; } = null;
}