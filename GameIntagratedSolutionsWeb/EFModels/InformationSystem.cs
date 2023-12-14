using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IntelliTrackSolutionsWeb.EFModels;

[Table("InformationSystem")]
public class InformationSystem
{
    [Key] [Column("idInformationSystem")] public int IdInformationSystem { get; set; }

    /// <summary>
    ///     Название информационной системы.
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Название")]
    public string Title { get; set; } = null!;

    /// <summary>
    ///     Описание информационной системы.
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Описание")]
    public string Description { get; set; } = null!;

    /// <summary>
    ///     Ключ апи для работы с данной информационной системой.
    /// </summary>
    [Column(TypeName = "text")]
    [Display(Name = "Ключ апи для работы ИИ с данными")]
    public string ApiKey { get; set; } = null!;

    [JsonIgnore]
    [InverseProperty("InformationSystem")]
    public virtual ICollection<AccessLevel> AccessLevels { get; set; } = new List<AccessLevel>();

    [InverseProperty("InformationSystem")] public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    [InverseProperty("InformationSystem")]
    public virtual ICollection<SystemObject> SystemObjects { get; set; } = new List<SystemObject>();

    [InverseProperty("InformationSystem")] public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}