using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[PrimaryKey("IdInformationUser", "UserId")]
[Table("InformationUser")]
[Index("UserId", Name = "fk_InformationUser_User_idx", IsUnique = true)]
public class InformationUser
{
    [Key] [Column("idInformationUser")] public int IdInformationUser { get; set; }

    /// <summary>
    ///     Идентификатор связанного пользователя.
    /// </summary>
    [Key]
    public int UserId { get; set; }

    /// <summary>
    ///     Имя пользователя.
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Имя")]
    public string FirstName { get; set; } = null!;

    /// <summary>
    ///     Фамилия пользователя.
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; } = null!;

    /// <summary>
    ///     Отчество пользователя.
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Отчество")]
    public string? MiddleName { get; set; }

    [Column(TypeName = "text")]
    public string Avatar { get; set; } =
        "https://w7.pngwing.com/pngs/13/639/png-transparent-computer-icons-book-reading-symbol-book-angle-share-icon-ooh.png";

    [JsonIgnore]
    [ForeignKey("UserId")]
    [InverseProperty("InformationUser")]
    public virtual User? User { get; set; } = null;
}