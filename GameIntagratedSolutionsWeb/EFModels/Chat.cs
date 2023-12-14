using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[Table("Chat")]
[Index("InformationSystemId", Name = "fk_Chat_InformationSystem_idx")]
public class Chat
{
    [Key] [Column("idChat")] public int IdChat { get; set; }

    /// <summary>
    ///     Название чата.
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Название")]
    public string Title { get; set; } = null!;

    /// <summary>
    ///     Идентификатор информационной системы, к которой принадлежит чат.
    /// </summary>
    [Display(Name = "Идентификатор игровой платформы")]
    public int InformationSystemId { get; set; }

    [JsonIgnore]
    [ForeignKey("InformationSystemId")]
    [InverseProperty("Chats")]
    public virtual InformationSystem? InformationSystem { get; set; } = null;

    [InverseProperty("Chat")] public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}