using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[Keyless]
public class UserinformationView
{
    [Column("idUser")] public int IdUser { get; set; }

    /// <summary>
    ///     Имя пользователя.
    /// </summary>
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    /// <summary>
    ///     Фамилия пользователя.
    /// </summary>
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(152)] public string? FullName { get; set; }

    [StringLength(50)] public string RoleTitle { get; set; } = null!;

    [StringLength(4)]
    [MySqlCharSet("utf8mb4")]
    [MySqlCollation("utf8mb4_0900_ai_ci")]
    public string Permission { get; set; } = null!;
}