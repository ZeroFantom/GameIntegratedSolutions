using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[PrimaryKey("IdTwoFactorAuthentification", "UserId")]
[Table("TwoFactorAuthentification")]
[Index("UserId", Name = "fk_TwoFactorAuthentification_User_idx", IsUnique = true)]
public class TwoFactorAuthentification
{
    [Key]
    [Column("idTwoFactorAuthentification")]
    public int IdTwoFactorAuthentification { get; set; }

    /// <summary>
    ///     Идентификатор связанного пользователя.
    /// </summary>
    [Key]
    [Display(Name = "Идентификатор пользователя")]
    public int UserId { get; set; }

    /// <summary>
    ///     Код двухфакторной авторизации.
    /// </summary>
    [StringLength(6)]
    [Display(Name = "Код авторизации")]
    public string Code { get; set; } = null!;

    /// <summary>
    ///     Последнее обновление кода двухфакторной авторизации.
    /// </summary>
    [Column(TypeName = "datetime")]
    [Display(Name = "Дата последнего обновления кода")]
    public DateTime LastUpdateTime { get; set; }

    [JsonIgnore]
    [ForeignKey("UserId")]
    [InverseProperty("TwoFactorAuthentification")]
    public virtual User? User { get; set; } = null;
}