using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.JourneyRepository;
using Rememory.Persistence.Repositories.UserRepository;
using Rememory.WebApi.Dtos;
using Rememory.WebApi.Exceptions;

namespace Rememory.WebApi.Controllers;

[Authorize]
public class JourneysController : BaseController
{
    private readonly IJourneyRepository _journeyRepository;

    public JourneysController(
        IUserRepository userRepository,
        IJourneyRepository journeyRepository) : base(userRepository)
    {
        _journeyRepository = journeyRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetJourney(Guid id)
    {
        var journey = await GetJourneyAndCheckAccessAsync(id);
        return Ok(journey);
    }
    
    [HttpGet("/current")]
    public async Task<ActionResult> GetCurrentJourney()
    {
        var user = await GetCurrentUser();
        var now = DateTime.Now;

        var journey = await _journeyRepository.GetByDateAndUserAsync(now, user.Id);

        if (journey is null)
            throw new BadRequestException();

        return Ok(journey);
    }

    [HttpGet("")]
    public async Task<ActionResult> GetJourneys()
    {
        var user = await GetCurrentUser();
        var journeys = await _journeyRepository.GetByUserAsync(user.Id);
        
        return Ok(journeys);
    }

    [HttpPost("")]
    public async Task<ActionResult> AddJourney([FromBody] AddJourneyDto addJourneyDto)
    {
        if (addJourneyDto.Start > addJourneyDto.End)
            throw new BadRequestException();
        
        var user = await GetCurrentUser();

        var journey = new Journey
        {
            Title = addJourneyDto.Title,
            Start = addJourneyDto.Start.ToUniversalTime(),
            End = addJourneyDto.End.ToUniversalTime(),
            UserId = user.Id
        };

        await _journeyRepository.CreateAsync(journey);

        return Ok(journey);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateJourney(
        [FromQuery] Guid id,
        [FromBody] UpdateJourneyDto updateJourneyDto)
    {
        var journey = await GetJourneyAndCheckAccessAsync(id);

        journey.Start = (updateJourneyDto.Start ?? journey.Start).ToUniversalTime();
        journey.End = (updateJourneyDto.End ?? journey.End).ToUniversalTime();
        journey.Title = updateJourneyDto.Title ?? journey.Title;

        if (journey.Start > journey.End)
            throw new BadRequestException();

        await _journeyRepository.UpdateAsync(journey.Id, journey);

        return Ok(journey);
    } 

    private async Task<Journey> GetJourneyAndCheckAccessAsync(Guid id)
    {
        var user = await GetCurrentUser();
        var journey = await _journeyRepository.GetAsync(id);
        
        if (journey is null)
            throw new NotFoundException();

        if (journey.UserId != user.Id)
            throw new ForbiddenException();

        return journey;
    }
}