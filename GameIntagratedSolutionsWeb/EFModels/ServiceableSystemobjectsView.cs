using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[Keyless]
public class ServiceableSystemobjectsView
{
    [Column("idSystemObject")] public int IdSystemObject { get; set; }

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
    ///     Название информационной системы.
    /// </summary>
    [StringLength(50)]
    public string InformationSystemTitle { get; set; } = null!;
}