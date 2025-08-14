using application.Dtos.Series;

namespace application.Dtos.Category;

public class CategorySeriesDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // public SeriesCategory? SeriesCategory { get; set; }
    public List<SeriesDto> SeriesDto { get; set; } = [];
}