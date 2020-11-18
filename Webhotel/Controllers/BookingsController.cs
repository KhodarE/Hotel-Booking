using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHotel.Models;
using Webhotel.Data;
using Webhotel.Models.ViewModels;
using Microsoft.Data.Sqlite;
using System.Security.Claims;

namespace Webhotel.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string sortOrder)
        {
            if (String.IsNullOrEmpty(sortOrder))
            {
                // When the Index page is loaded for the first time, the sortOrder is empty.
                // By default, the movies should be displayed in title_asc.
                sortOrder = "in_asc";
            }

            string _email = User.FindFirst(ClaimTypes.Name).Value;
            var bookings = (IQueryable<Booking>)_context.Booking.Include(b => b.TheCustomer).Include(b => b.TheRoom).Where(b => b.CustomerEmail == _email);

            switch (sortOrder)
            {
                case "in_asc":
                    bookings = bookings.OrderBy(m => m.CheckIn);
                    break;
                case "in_desc":
                    bookings = bookings.OrderByDescending(m => m.CheckIn);
                    break;
                case "cost_asc":
                    bookings = bookings.OrderBy(m => m.Cost);
                    break;
                case "cost_desc":
                    bookings = bookings.OrderByDescending(m => m.Cost);
                    break;
            }
            ViewData["NextCheckIn"] = sortOrder != "in_asc" ? "in_asc" : "in_desc";
            ViewData["NextCost"] = sortOrder != "cost_asc" ? "cost_asc" : "cost_desc";
            return View(await bookings.AsNoTracking().ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email");
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID");
            return View();
        }
        

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MakeBooking basicBooking)
        {
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            if (ModelState.IsValid)
            {
                var roomID = new SqliteParameter("roomID", basicBooking.RoomID);
                var checkIn = new SqliteParameter("in", basicBooking.CheckIn);
                var checkOut = new SqliteParameter("out", basicBooking.CheckOut);

                var freeBookings = _context.Booking.FromSql("select * from [Booking] inner join [Room] on [Booking].RoomID = [Room].ID "
                                           + "where [Booking].RoomID = @roomID and @in >= [Booking].checkIn" +
                                           " and @out <= [Booking].checkOut", roomID, checkIn, checkOut)
                                   .Select(mo => new Booking
                                   {
                                       ID = mo.ID,
                                       RoomID = mo.RoomID,
                                       CustomerEmail = mo.CustomerEmail,
                                       CheckIn = mo.CheckIn,
                                       CheckOut = mo.CheckOut,
                                       Cost = mo.Cost
                                   });

                ViewBag.FreeBookings = await freeBookings.ToListAsync();

                if (ViewBag.FreeBookings.Count == 0)
                {
                    var booking = new Booking
                    {
                        RoomID = basicBooking.RoomID,
                        CheckIn = basicBooking.CheckIn,
                        CheckOut = basicBooking.CheckOut,
                        CustomerEmail = _email
                    };

                    var theRoom = await _context.Room.FindAsync(basicBooking.RoomID);
                    var theCustomer = await _context.Customer.FindAsync(_email);
                    theCustomer.Email = _email;
                    TimeSpan diff = basicBooking.CheckOut - basicBooking.CheckIn;
                    double noOfDays = diff.TotalDays;

                    booking.Cost = theRoom.Price * noOfDays;

                    _context.Add(booking);
                    await _context.SaveChangesAsync();

                    ViewBag.roomLevel = theRoom.Level;
                    ViewBag.totalCost = booking.Cost;
                }
            }
            return View(basicBooking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", booking.RoomID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (id != booking.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "ID", "ID", booking.RoomID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.ID == id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> CalcHotelStats()
        {
           
            // divide the movies into groups by genre
            var customerGroups = _context.Customer.GroupBy(m => m.Postcode);
            var bookingGroups = _context.Booking.GroupBy(m => m.RoomID);

            // for each group, get its genre value and the number of movies in this group
            var customerStats = customerGroups.Select(g => new HotelStatistics { Postcode = g.Key, CustomerCount = g.Count() });
            var bookingStats = bookingGroups.Select(g => new HotelStatistics { RoomID = g.Key, BookingCount = g.Count() });
            ViewBag.CustomerStats = await customerStats.ToListAsync();
            ViewBag.BookingStats = await bookingStats.ToListAsync();
            // pass the list of GenreStatistic objects to view
            return View();
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.ID == id);
        }
    }
}
