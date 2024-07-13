using static System.Net.Mime.MediaTypeNames;

namespace IdentityService.WebAPI.ResponseObject.User
{
    public record UserPersonalInformation(
        string? Ip,
        long UserId,
        string UserName,
        string AvatarImageURL
        );
}
