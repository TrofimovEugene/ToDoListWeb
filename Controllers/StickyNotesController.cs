using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListWeb.Data;
using ToDoListWeb.Models;

namespace ToDoListWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StickyNotesController : ControllerBase
    {
        private readonly ToDoListWebContext _context;

        public StickyNotesController(ToDoListWebContext context)
        {
            _context = context;
        }

        // GET: api/StickyNotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StickyNote>>> GetStickyNote()
        {
            return await _context.StickyNote.ToListAsync();
        }

        // GET: api/StickyNotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StickyNote>> GetStickyNote(int id)
        {
            var stickyNote = await _context.StickyNote.FindAsync(id);

            if (stickyNote == null)
            {
                return NotFound();
            }

            return stickyNote;
        }

        // PUT: api/StickyNotes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStickyNote(int id, StickyNote stickyNote)
        {
            if (id != stickyNote.Id)
            {
                return BadRequest();
            }

            _context.Entry(stickyNote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StickyNoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/StickyNotes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<StickyNote>> PostStickyNote(StickyNote stickyNote)
        {
            _context.StickyNote.Add(stickyNote);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStickyNote", new { id = stickyNote.Id }, stickyNote);
        }

        // DELETE: api/StickyNotes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StickyNote>> DeleteStickyNote(int id)
        {
            var stickyNote = await _context.StickyNote.FindAsync(id);
            if (stickyNote == null)
            {
                return NotFound();
            }

            _context.StickyNote.Remove(stickyNote);
            await _context.SaveChangesAsync();

            return stickyNote;
        }

        private bool StickyNoteExists(int id)
        {
            return _context.StickyNote.Any(e => e.Id == id);
        }
    }
}
