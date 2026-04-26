namespace GoodHamburger.Api.DTOs;

public sealed record OrderRequest(IReadOnlyList<int> ItemIds);
