using System.Security.Claims;

namespace WebStore.Domain.DTO.Identity;

public class ClaimDTO : UserDTO
{
    public IEnumerable<ClaimDTO> Claims { get; init; } = null!;
}

public class ReplaceClaimDTO : UserDTO
{
    // исходный
    public Claim Claim { get; init; } = null!;
    // на который производится замена
    public Claim NewClaim { get; init; } = null!;
}
