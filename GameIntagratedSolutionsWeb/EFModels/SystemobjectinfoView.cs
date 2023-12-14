using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[Keyless]
public class SystemobjectinfoView
{
    [Column("idSystemObject")] public int IdSystemObject { get; set; }

    /// <summary>
    ///     Название информационной системы.
    /// </summary>
    [StringLength(50)]
    public string InformationSystemTitle { get; set; } = null!;

    /// <summary>
    ///     Наименование системного объекта.
    /// </summary>
    [StringLength(50)]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Описание системного объекта.
    /// </summary>
    [StringLength(400)]
    public string Description { get; set; } = null!;

    /// <summary>
    ///     Состояние системного объекта.
    /// </summary>
    [Column(TypeName = "enum('Faulty','Serviceable','Disabled')")]
    public string Condition { get; set; } = null!;

    /// <summary>
    ///     Дата регистрации системного объекта.
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime DataRegistration { get; set; }
}