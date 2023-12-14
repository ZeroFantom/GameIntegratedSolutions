using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[Table("AccessLevel")]
[Index("InformationSystemId", Name = "fk_AccessLevel_InformationSystem_idx")]
[Index("InformationSystemId", "Permission", Name = "uq_System_Permission", IsUnique = true)]
public class AccessLevel
{
    [Key] [Column("idAccessLevel")] public int IdAccessLevel { get; set; }

    /// <summary>
    ///     Уровень доступа в информационной системе.
    /// </summary>
    [Display(Name = "Показатель доступа")]
    public sbyte Permission { get; set; }

    /// <summary>
    ///     Идентификатор информационной системы к которой принадлежит лист доступа.
    /// </summary>
    [Display(Name = "Идентификатор игровой платформы")]
    public int InformationSystemId { get; set; }

    [ForeignKey("InformationSystemId")]
    [InverseProperty("AccessLevels")]
    public virtual InformationSystem? InformationSystem { get; set; } = null;

    [InverseProperty("AccessLevel")] public virtual Role? Role { get; set; }

    [JsonIgnore]
    [InverseProperty("AccessLevel")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}