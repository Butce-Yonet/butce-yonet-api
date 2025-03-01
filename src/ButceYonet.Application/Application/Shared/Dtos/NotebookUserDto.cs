namespace ButceYonet.Application.Application.Shared.Dtos;

public class NotebookUserDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int NotebookId { get; set; }
    public bool IsDefault { get; set; }
    public UserDto User { get; set; }
}