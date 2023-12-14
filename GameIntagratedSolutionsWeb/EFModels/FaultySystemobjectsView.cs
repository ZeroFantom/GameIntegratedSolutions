using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[Keyless]
public class FaultySystemobjectsView
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
    ///     Широта.
    /// </summary>
    public float Latitude { get; set; }

    /// <summary>
    ///     Долгота.
    /// </summary>
    public float Longitude { get; set; }
}