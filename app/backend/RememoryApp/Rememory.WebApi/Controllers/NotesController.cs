using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rememory.Persistence.Entities;
using Rememory.Persistence.Models;
using Rememory.Persistence.Repositories.JourneyRepository;
using Rememory.Persistence.Repositories.NoteRepository;
using Rememory.Persistence.Repositories.UserRepository;
using Rememory.WebApi.Dtos;
using Rememory.WebApi.Exceptions;

namespace Rememory.WebApi.Controllers;

[Authorize]
public class NotesController : BaseController
{
    private readonly INoteRepository _noteRepository;
    private readonly IJourneyRepository _journeyRepository;

    public NotesController(
        IUserRepository userRepository,
        IJourneyRepository journeyRepository,
        INoteRepository noteRepository) : base(userRepository)
    {
        _journeyRepository = journeyRepository;
        _noteRepository = noteRepository;
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetNote(Guid id)
    {
        var note = await _noteRepository.GetAsync(id);
        if (note is null)
            throw new NotFoundException();

        var user = await GetCurrentUser();
        var journey = await _journeyRepository.GetAsync(note.Id) 
                      ?? throw new NotFoundException();

        if (journey.UserId != user.Id)
            throw new ForbiddenException();

        return Ok(note);
    }

    [HttpPost("/textNote")]
    public async Task<ActionResult> AddNote([FromBody] AddNoteDto addNoteDto)
    {
        return await AddNoteAsync(addNoteDto, NoteContentType.Text);
    }

    private async Task<ActionResult> AddNoteAsync(AddNoteDto addNoteDto, NoteContentType contentType)
    {
        addNoteDto.JourneyId ??= await GetCurrentJourneyIdAsync();
        addNoteDto.DateTime = addNoteDto.DateTime.ToUniversalTime();

        var note = new Note
        {
            Type = contentType,
            Content = addNoteDto.Content,
            DateTime = addNoteDto.DateTime,
            JourneyId = addNoteDto.JourneyId.Value,
        };

        await _noteRepository.CreateAsync(note);

        return Ok(note);
    }

    private async Task<Guid> GetCurrentJourneyIdAsync()
    {
        var user = await GetCurrentUser();
        var now = DateTime.Now;
        var currentJourney = await _journeyRepository.GetByDateAndUserAsync(now, user.Id)
                             ?? throw new NotFoundException();
        return currentJourney.Id;
    }
}