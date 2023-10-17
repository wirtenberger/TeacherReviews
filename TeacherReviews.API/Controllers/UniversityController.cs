using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherReviews.API.Contracts.Requests.University;
using TeacherReviews.API.Mapping;
using TeacherReviews.API.Services;

namespace TeacherReviews.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UniversityController : ControllerBase
{
    private readonly CityService _cityService;
    private readonly UniversityService _universityService;
    private readonly TeacherService _teacherService;

    public UniversityController(UniversityService universityService, TeacherService teacherService, CityService cityService)
    {
        _universityService = universityService;
        _teacherService = teacherService;
        _cityService = cityService;
    }

    [HttpGet("getall")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUniversities()
    {
        var universities = await _universityService.GetAllAsync();

        return Ok(universities.Select(u => u.ToDto()));
    }

    [HttpGet("get")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUniversity([FromQuery] GetUniversityRequest universityRequest)
    {
        return Ok(
            (await _universityService.GetByIdAsync(universityRequest.Id)).ToDto()
        );
    }

    [Authorize]
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUniversity([FromBody] CreateUniversityRequest createUniversityRequest)
    {
        var createdUniversity = await _universityService.CreateAsync(
            createUniversityRequest.ToUniversity()
        );

        return Ok(createdUniversity.ToDto());
    }

    [Authorize]
    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUniversity([FromBody] UpdateUniversityRequest updateUniversityRequest)
    {
        var updatedUniversity = await _universityService.UpdateAsync(
            updateUniversityRequest.ToUniversity()
        );

        return Ok(updatedUniversity.ToDto());
    }

    [Authorize]
    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUniversity([FromQuery] DeleteUniversityRequest deleteUniversityRequest)
    {
        var deletedUniversity = await _universityService.DeleteAsync(
            deleteUniversityRequest.Id
        );

        return Ok(deletedUniversity.ToDto());
    }

    [HttpGet("getcity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCity([FromQuery] GetUniversitysCityRequest universitysCityRequest)
    {
        var university = await _universityService.GetByIdAsync(universitysCityRequest.Id);
        var city = await _cityService.GetByIdAsync(university.CityId);
        return Ok(
            city.ToDto()
        );
    }

    [HttpGet("getteachers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTeachers([FromQuery] GetUniversitysTeachers universitysCityRequest)
    {
        var university = await _universityService.GetByIdAsync(universitysCityRequest.Id);
        var teachers = await _teacherService.GetAllAsync(t => t.UniversityId == university.Id);
        return Ok(
            teachers.Select(t => t.ToDto())
        );
    }
}