using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[Table("User")]
[Index("AccessLevelId", Name = "fk_User_AccessLevel_idx")]
[Index("Password", "Login", Name = "uq_login_password", IsUnique = true)]
public class User
{
    [Key] [Column("idUser")] public int IdUser { get; set; }

    /// <summary>
    ///     Логин пользователя.
    /// </summary>
    [StringLength(100)]
    [Display(Name = "Логин")]
    public string Login { get; set; } = null!;

    /// <summary>
    ///     Пароль пользователя.
    /// </summary>
    [StringLength(100)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;

    /// <summary>
    ///     Идентификатор листа доступа пользователя, необходим для получения соответствующей уровню доступа информации.
    /// </summary>
    [Display(Name = "Уровень доступа")]
    public int? AccessLevelId { get; set; }

    [ForeignKey("AccessLevelId")]
    [InverseProperty("Users")]
    public virtual AccessLevel? AccessLevel { get; set; }

    [InverseProperty("User")] public virtual InformationUser? InformationUser { get; set; }

    [InverseProperty("Author")] public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [InverseProperty("User")] public virtual TwoFactorAuthentification? TwoFactorAuthentification { get; set; }
}