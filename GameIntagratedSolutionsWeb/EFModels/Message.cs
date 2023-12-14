using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace IntelliTrackSolutionsWeb.EFModels;

[PrimaryKey("IdMessage", "ChatId")]
[Table("Message")]
[Index("ChatId", Name = "fk_Message_Chat_idx")]
[Index("AuthorId", Name = "fk_Message_User_idx")]
public class Message
{
    [Key] [Column("idMessage")] public int IdMessage { get; set; }

    /// <summary>
    ///     Идентификатор чата, в котором находится данное сообщение.
    /// </summary>
    [Key]
    public int ChatId { get; set; }

    /// <summary>
    ///     Текст сообщения.
    /// </summary>
    [Column(TypeName = "text")]
    [Display(Name = "Текст сообщения")]
    public string TextMessage { get; set; } = null!;

    /// <summary>
    ///     Идентификатор автора сообщения.
    /// </summary>
    [Display(Name = "Идентификатор автора")]
    public int AuthorId { get; set; }

    [JsonIgnore]
    [ForeignKey("AuthorId")]
    [InverseProperty("Messages")]
    public virtual User? Author { get; set; } = null;

    [JsonIgnore]
    [ForeignKey("ChatId")]
    [InverseProperty("Messages")]
    public virtual Chat? Chat { get; set; } = null;
}