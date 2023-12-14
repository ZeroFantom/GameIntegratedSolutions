using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[PrimaryKey("IdLocation", "SystemObjectId")]
[Table("Location")]
[Index("SystemObjectId", Name = "fk_Location_SystemObject_idx")]
[Index("Longitude", "Latitude", "SystemObjectId", Name = "uq_longitude_latitude", IsUnique = true)]
public class Location
{
    [Key] [Column("idLocation")] public int IdLocation { get; set; }

    /// <summary>
    ///     Идентификатор системного объекта, к которому принадлежат данные кординаты.
    /// </summary>
    [Key]
    [Display(Name = "Идентификатор СО")]
    public int SystemObjectId { get; set; }

    /// <summary>
    ///     Широта.
    /// </summary>
    [Display(Name = "Широта")]
    [Range(-90.0, 90.0, ErrorMessage = "Значение широты должно быть в диапазоне от -90 до 90.")]
    public float Latitude { get; set; }

    /// <summary>
    ///     Долгота.
    /// </summary>
    [Display(Name = "Долгота")]
    [Range(-180.0, 180.0, ErrorMessage = "Значение долготы должно быть в диапазоне от -180 до 180.")]
    public float Longitude { get; set; }

    [JsonIgnore]
    [ForeignKey("SystemObjectId")]
    [InverseProperty("Locations")]
    public virtual SystemObject? SystemObject { get; set; } = null;
}