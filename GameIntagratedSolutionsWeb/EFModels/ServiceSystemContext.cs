using System.ComponentModel.DataAnnotations;
using IntelliTrackSolutionsWeb.EFModels_Partial;
using IntelliTrackSolutionsWeb.EFModels_Partial.Converter;
using Microsoft.EntityFrameworkCore;
using TaskStatus = IntelliTrackSolutionsWeb.EFModels_Partial.Converter.TaskStatus;

namespace IntelliTrackSolutionsWeb.EFModels;

public partial class ServiceSystemContext : DbContext
{
    public ServiceSystemContext()
    {
    }

    public ServiceSystemContext(DbContextOptions<ServiceSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessLevel> AccessLevels { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<FaultySystemobjectsView> FaultySystemobjectsViews { get; set; }

    public virtual DbSet<InformationSystem> InformationSystems { get; set; }

    public virtual DbSet<InformationUser> InformationUsers { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServiceableSystemobjectsView> ServiceableSystemobjectsViews { get; set; }

    public virtual DbSet<SystemObject> SystemObjects { get; set; }

    public virtual DbSet<SystemobjectinfoView> SystemobjectinfoViews { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TwoFactorAuthentification> TwoFactorAuthentifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserinformationView> UserinformationViews { get; set; }

    public IQueryable<string> SelectExpression(string nameFunction, List<string> parametrList)
    {
        return Database.SqlQueryRaw<string>(
            $"SELECT `{nameFunction}`({parametrList.Aggregate((x, y) => $"\"{x}\",\"{y}\"")});");
    }

    public IQueryable<string> SelectExpression(string nameFunction, string parametr)
    {
        return Database.SqlQueryRaw<string>($"SELECT `{nameFunction}`(\"{parametr}\");");
    }

    ///
    public string DecryptPassword([StringLength(255)] string encryptSoli)
    {
        return SelectExpression(nameof(DecryptPassword), encryptSoli).ToList()[0];
    }

    public string EncryptPassword([StringLength(255)] string soli)
    {
        return SelectExpression(nameof(EncryptPassword), soli).ToList()[0];
    }

    ///
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigurationMySql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<AccessLevel>(entity =>
        {
            entity.HasKey(e => e.IdAccessLevel).HasName("PRIMARY");

            entity.Property(e => e.InformationSystemId)
                .HasComment("Идентификатор информационной системы к которой принадлежит лист доступа.");
            entity.Property(e => e.Permission).HasComment("Уровень доступа в информационной системе.");

            entity.HasOne(d => d.InformationSystem).WithMany(p => p.AccessLevels)
                .HasConstraintName("fk_AccessList_InformationSystem");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.IdChat).HasName("PRIMARY");

            entity.Property(e => e.InformationSystemId)
                .HasComment("Идентификатор информационной системы, к которой принадлежит чат.");
            entity.Property(e => e.Title)
                .HasDefaultValueSql("'Название чата...'")
                .HasComment("Название чата.");

            entity.HasOne(d => d.InformationSystem).WithMany(p => p.Chats)
                .HasConstraintName("fk_Chat_InformationSystem");
        });

        modelBuilder.Entity<FaultySystemobjectsView>(entity =>
        {
            entity.ToView("faulty_systemobjects_view");

            entity.Property(e => e.Description).HasComment("Описание системного объекта.");
            entity.Property(e => e.Latitude).HasComment("Широта.");
            entity.Property(e => e.Longitude).HasComment("Долгота.");
            entity.Property(e => e.Name).HasComment("Наименование системного объекта.");
        });

        modelBuilder.Entity<InformationSystem>(entity =>
        {
            entity.HasKey(e => e.IdInformationSystem).HasName("PRIMARY");

            entity.Property(e => e.ApiKey).HasComment("Ключ апи для работы с данной информационной системой.");
            entity.Property(e => e.Description).HasComment("Описание информационной системы.");
            entity.Property(e => e.Title).HasComment("Название информационной системы.");
        });

        modelBuilder.Entity<InformationUser>(entity =>
        {
            entity.HasKey(e => new { e.IdInformationUser, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.IdInformationUser).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasComment("Идентификатор связанного пользователя.");
            entity.Property(e => e.FirstName).HasComment("Имя пользователя.");
            entity.Property(e => e.LastName).HasComment("Фамилия пользователя.");
            entity.Property(e => e.MiddleName).HasComment("Отчество пользователя.");

            entity.HasOne(d => d.User).WithOne(p => p.InformationUser).HasConstraintName("fk_InformationUser_User1");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => new { e.IdLocation, e.SystemObjectId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.IdLocation).ValueGeneratedOnAdd();
            entity.Property(e => e.SystemObjectId)
                .HasComment("Идентификатор системного объекта, к которому принадлежат данные кординаты.");
            entity.Property(e => e.Latitude)
                .HasComment("Широта.");
            entity.Property(e => e.Longitude).HasComment("Долгота.");

            entity.HasOne(d => d.SystemObject).WithMany(p => p.Locations).HasConstraintName("fk_Location_SystemObject");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => new { e.IdMessage, e.ChatId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.IdMessage).ValueGeneratedOnAdd();
            entity.Property(e => e.ChatId).HasComment("Идентификатор чата, в котором находится данное сообщение.");
            entity.Property(e => e.AuthorId).HasComment("Идентификатор автора сообщения.");
            entity.Property(e => e.TextMessage).HasComment("Текст сообщения.");

            entity.HasOne(d => d.Author).WithMany(p => p.Messages).HasConstraintName("fk_Message_User");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages).HasConstraintName("fk_Message_Chat");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => new { e.IdRole, e.AccessLevelId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.IdRole).ValueGeneratedOnAdd();
            entity.Property(e => e.AccessLevelId).HasComment("Привязанный к роли лист доступа.");
            entity.Property(e => e.Description).HasComment("Описание роли.");
            entity.Property(e => e.Title).HasComment("Название роли.");

            entity.HasOne(d => d.AccessLevel).WithOne(p => p.Role).HasConstraintName("fk_Role_AccessList");
        });

        modelBuilder.Entity<ServiceableSystemobjectsView>(entity =>
        {
            entity.ToView("serviceable_systemobjects_view");

            entity.Property(e => e.Description).HasComment("Описание системного объекта.");
            entity.Property(e => e.InformationSystemTitle).HasComment("Название информационной системы.");
            entity.Property(e => e.Name).HasComment("Наименование системного объекта.");
        });

        modelBuilder.Entity<SystemObject>(entity =>
        {
            entity.HasKey(e => e.IdSystemObject).HasName("PRIMARY");

            entity.Property(e => e.Condition)
                .HasDefaultValueSql("'Serviceable'")
                .HasComment("Состояние системного объекта.")
                .HasConversion(new TaskStatusConverter<ObjectCondition>());
            entity.Property(e => e.DataRegistration)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Дата регистрации системного объекта.");
            entity.Property(e => e.Description).HasComment("Описание системного объекта.");
            entity.Property(e => e.InformationSystemId)
                .HasComment("Идентификатор информационной системы к которой принадлежит системный объект.");
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Срок последнего обновления информации о системном объекте.");
            entity.Property(e => e.Name).HasComment("Наименование системного объекта.");

            entity.HasOne(d => d.InformationSystem).WithMany(p => p.SystemObjects)
                .HasConstraintName("fk_SystemObject_InformationSystem");
        });

        modelBuilder.Entity<SystemobjectinfoView>(entity =>
        {
            entity.ToView("systemobjectinfo_view");

            entity.Property(e => e.Condition)
                .HasDefaultValueSql("'Serviceable'")
                .HasComment("Состояние системного объекта.");
            entity.Property(e => e.DataRegistration)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Дата регистрации системного объекта.");
            entity.Property(e => e.Description).HasComment("Описание системного объекта.");
            entity.Property(e => e.InformationSystemTitle).HasComment("Название информационной системы.");
            entity.Property(e => e.Name).HasComment("Наименование системного объекта.");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => new { e.IdTask, e.InformationSystemId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.IdTask).ValueGeneratedOnAdd();
            entity.Property(e => e.InformationSystemId)
                .HasComment("Идентификатор информационной системы к которой принадлежит задача.");
            entity.Property(e => e.DataRegistration)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Дата регистрации задачи.");
            entity.Property(e => e.Deadline).HasComment("Срок до которого нужно выполнить задачу.");
            entity.Property(e => e.Goal).HasComment("Цель задачи.");
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Последнее обновление задачи.");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'PendingExecution'")
                .HasComment("Статус выполнения задачи.")
                .HasConversion(new TaskStatusConverter<TaskStatus>());
            entity.Property(e => e.Title).HasComment("Название задачи.");

            entity.HasOne(d => d.InformationSystem).WithMany(p => p.Tasks)
                .HasConstraintName("fk_Task_InformationSystem");
        });

        modelBuilder.Entity<TwoFactorAuthentification>(entity =>
        {
            entity.HasKey(e => new { e.IdTwoFactorAuthentification, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.IdTwoFactorAuthentification).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasComment("Идентификатор связанного пользователя.");
            entity.Property(e => e.Code).HasComment("Код двухфакторной авторизации.");
            entity.Property(e => e.LastUpdateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Последнее обновление кода двухфакторной авторизации.");

            entity.HasOne(d => d.User).WithOne(p => p.TwoFactorAuthentification)
                .HasConstraintName("fk_TwoFactorAuthentification_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PRIMARY");

            entity.Property(e => e.AccessLevelId)
                .HasComment(
                    "Идентификатор листа доступа пользователя, необходим для получения соответствующей уровню доступа информации.");
            entity.Property(e => e.Login).HasComment("Логин пользователя.");
            entity.Property(e => e.Password).HasComment("Пароль пользователя.");

            entity.HasOne(d => d.AccessLevel).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_User_AccessList");
        });

        modelBuilder.Entity<UserinformationView>(entity =>
        {
            entity.ToView("userinformation_view");

            entity.Property(e => e.FirstName).HasComment("Имя пользователя.");
            entity.Property(e => e.LastName).HasComment("Фамилия пользователя.");
            entity.Property(e => e.Permission).HasDefaultValueSql("''");
            entity.Property(e => e.RoleTitle).HasDefaultValueSql("''");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}