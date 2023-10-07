using Microsoft.AspNetCore.Mvc;
using TeacherReviews.API.Contracts.Requests.City;
using TeacherReviews.API.Mapping;
using TeacherReviews.API.Services;

namespace TeacherReviews.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly CityService _cityService;

    public CityController(CityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAllCities()
    {
        return Ok(
            (await _cityService.GetAllAsync())
            .Select(c => c.ToDto())
        );
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetCity([FromQuery] GetCityRequest cityRequest)
    {
        return Ok(
            (await _cityService.GetByIdAsync(cityRequest.Id)).ToDto()
        );
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityRequest createCityRequest)
    {
        var createdCity = await _cityService.CreateAsync(
            createCityRequest.ToCity()
        );

        return Ok(createdCity.ToDto());
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCity([FromBody] UpdateCityRequest updateCityRequest)
    {
        var updatedCity = await _cityService.UpdateAsync(
            updateCityRequest.ToCity()
        );

        return Ok(updatedCity.ToDto());
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteCity([FromQuery] DeleteCityRequest deleteCityRequest)
    {
        var deletedCity = await _cityService.DeleteAsync(
            deleteCityRequest.Id
        );

        return Ok(deletedCity.ToDto());
    }

    [HttpGet("universities")]
    public async Task<IActionResult> GetUniversities([FromQuery] GetCityUniversitiesRequest cityUniversitiesRequest)
    {
        var universities = await _cityService.GetCitysUniversitiesAsync(cityUniversitiesRequest.Id);
        return Ok(universities.Select(u => u.ToDto()));
    }
}