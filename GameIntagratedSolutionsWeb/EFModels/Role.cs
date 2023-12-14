using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[PrimaryKey("IdRole", "AccessLevelId")]
[Table("Role")]
[Index("AccessLevelId", Name = "AccessLevel_idAccessLevel_UNIQUE", IsUnique = true)]
public class Role
{
    [Key] [Column("idRole")] public int IdRole { get; set; }

    /// <summary>
    ///     Привязанный к роли лист доступа.
    /// </summary>
    [Key]
    [Display(Name = "Уровень доступа")]
    public int AccessLevelId { get; set; }

    /// <summary>
    ///     Название роли.
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Название")]
    public string Title { get; set; } = null!;

    /// <summary>
    ///     Описание роли.
    /// </summary>
    [StringLength(400)]
    [Display(Name = "Описание")]
    public string Description { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey("AccessLevelId")]
    [InverseProperty("Role")]
    public virtual AccessLevel? AccessLevel { get; set; } = null;
}