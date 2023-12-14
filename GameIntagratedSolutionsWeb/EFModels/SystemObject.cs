using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using IntelliTrackSolutionsWeb.EFModels_Partial.Converter;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[Table("SystemObject")]
[Index("InformationSystemId", Name = "fk_SystemObject_InformationSystem_idx")]
[Index("Name", "IdSystemObject", Name = "uq_Name_idSystemObject", IsUnique = true)]
public class SystemObject
{
    [Key] [Column("idSystemObject")] public int IdSystemObject { get; set; }

    /// <summary>
    ///     Наименование системного объекта.
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Название")]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Описание системного объекта.
    /// </summary>
    [StringLength(400)]
    [Display(Name = "Описание")]
    public string Description { get; set; } = null!;

    /// <summary>
    ///     Состояние системного объекта.
    /// </summary>
    [Column(TypeName = "enum('Faulty','Serviceable','Disabled')")]
    [Display(Name = "Состояние")]
    public ObjectCondition Condition { get; set; } = ObjectCondition.Disabled;

    /// <summary>
    ///     Дата регистрации системного объекта.
    /// </summary>
    [Column(TypeName = "datetime")]
    [Display(Name = "Дата регистрации")]
    public DateTime DataRegistration { get; set; }

    /// <summary>
    ///     Срок последнего обновления информации о системном объекте.
    /// </summary>
    [Column(TypeName = "datetime")]
    [Display(Name = "Дата последнего обновления")]
    public DateTime LastUpdate { get; set; }

    /// <summary>
    ///     Идентификатор информационной системы к которой принадлежит системный объект.
    /// </summary>
    [Display(Name = "Идентификатор игровой платформы")]
    public int InformationSystemId { get; set; }

    [JsonIgnore]
    [ForeignKey("InformationSystemId")]
    [InverseProperty("SystemObjects")]
    public virtual InformationSystem? InformationSystem { get; set; } = null;

    [InverseProperty("SystemObject")]
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}