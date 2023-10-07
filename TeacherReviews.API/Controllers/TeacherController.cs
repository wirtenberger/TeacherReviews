using Microsoft.AspNetCore.Mvc;
using TeacherReviews.API.Contracts.Requests.Teacher;
using TeacherReviews.API.Mapping;
using TeacherReviews.API.Services;

namespace TeacherReviews.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeacherController : ControllerBase
{
    private readonly TeacherService _teacherService;

    public TeacherController(TeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet("getall")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTeachers()
    {
        var teachers = await _teacherService.GetAllAsync();

        return Ok(teachers.Select(t => t.ToDto()));
    }

    [HttpGet("get")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTeacher([FromQuery] GetTeacherRequest getTeacherRequest)
    {
        return Ok(
            (await _teacherService.GetByIdAsync(getTeacherRequest.Id)).ToDto()
        );
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherRequest createTeacherRequest)
    {
        var createdTeacher = await _teacherService.Create(
            createTeacherRequest.ToTeacher()
        );

        return Ok(createdTeacher.ToDto());
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTeacher([FromBody] UpdateTeacherRequest updateTeacherRequest)
    {
        var updatedTeacher = await _teacherService.UpdateAsync(
            updateTeacherRequest.ToTeacher()
        );
        return Ok(updatedTeacher.ToDto());
    }

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTeacher([FromQuery] DeleteTeacherRequest deleteTeacherRequest)
    {
        var deletedTeacher = await _teacherService.DeleteAsync(
            deleteTeacherRequest.Id
        );
        return Ok(deletedTeacher.ToDto());
    }

    [HttpGet("getreviews")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReviews([FromQuery] GetTeachersReviewsRequest getTeachersReviewsRequest)
    {
        var teacher = await _teacherService.GetByIdAsync(
            getTeachersReviewsRequest.Id
        );

        return Ok(
            teacher.Reviews.Select(r => r.ToDto())
        );
    }
}