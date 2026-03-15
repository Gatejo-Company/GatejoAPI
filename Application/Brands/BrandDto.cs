using API.Domain.Brands;

namespace API.Application.Brands;

public record BrandDto(int Id, string Name) {
	public static BrandDto From(Brand brand) => new(brand.Id, brand.Name);
}
