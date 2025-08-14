using application.Dtos.Category;
using application.Dtos.Series;
using domain.Entities;

namespace application.Mappers;

public static class CategoryMapper
{
    public static CategorySeriesDto ToCategorySeriesDto(
        this Category category, 
        List<SeriesDto> seriesDto)
    {
        return new CategorySeriesDto
        {
            Id = category.Id,
            Name = category.Name,
            SeriesDto = seriesDto
        };
    }
}