using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static DiabatesPredictor.MLModel;
using DiabatesPredictor;

namespace DiabetesPredictor.Web
{
    public class CharacteristicsController : Controller
    {
        private readonly DiabetesPredictorDBContext _context;
        //private readonly UserManager<IdentityUser> _userManager;

        public CharacteristicsController(DiabetesPredictorDBContext context)
        {
            _context = context;
        }

        // GET: Characteristics
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var _characteristics = _context.Characteristics.Where(p => p.UserId == userId);


            if(_characteristics!=null)
            {
                var _characteristicList = await _context.Characteristics.ToListAsync();
                if(_characteristicList.Count>0) 
                    return View(_characteristicList);   
                else return RedirectToAction(nameof(Create));
            }

            return   Problem("Entity set 'DiabetesPredictorDBContext.Characteristics'  is null.");
        }

        // GET: Characteristics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Characteristics == null)
            {
                return NotFound();
            }

            var characteristics = await _context.Characteristics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (characteristics == null)
            {
                return NotFound();
            }

            return View(characteristics);
        }

        // GET: Characteristics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Characteristics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Pregnancies,Glucose,BloodPressure,SkinThickness,Insulin,Bmi,DiabetesPedigreeFunction,Age")] Characteristics characteristics)
        {
          // var userId = _userManager.GetUserId(HttpContext.User);

           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null) 
            characteristics.UserId = userId;


            if (ModelState.IsValid)
            {
                _context.Add(characteristics);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(characteristics);
        }

        // GET: Characteristics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Characteristics == null)
            {
                return NotFound();
            }

            var characteristics = await _context.Characteristics.FindAsync(id);
            if (characteristics == null)
            {
                return NotFound();
            }
            return View(characteristics);
        }

        // POST: Characteristics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Pregnancies,Glucose,BloodPressure,SkinThickness,Insulin,Bmi,DiabetesPedigreeFunction,Age")] Characteristics characteristics)
        {
            if (id != characteristics.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
                characteristics.UserId = userId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(characteristics);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharacteristicsExists(characteristics.Id))
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
            return View(characteristics);
        }

        // GET: Characteristics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Characteristics == null)
            {
                return NotFound();
            }

            var characteristics = await _context.Characteristics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (characteristics == null)
            {
                return NotFound();
            }

            return View(characteristics);
        }

        // POST: Characteristics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Characteristics == null)
            {
                return Problem("Entity set 'DiabetesPredictorDBContext.Characteristics'  is null.");
            }
            var characteristics = await _context.Characteristics.FindAsync(id);
            if (characteristics != null)
            {
                _context.Characteristics.Remove(characteristics);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CharacteristicsExists(int id)
        {
          return (_context.Characteristics?.Any(e => e.Id == id)).GetValueOrDefault();
        }



     

        // POST: Characteristics/Create
      
        public async Task<IActionResult> Predict(ModelInput input)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var _characteristics = _context.Characteristics.Where(p => p.UserId == userId);


            if (_characteristics != null)
            {
                var _characteristicList = await _context.Characteristics.ToListAsync();
                if (_characteristicList.Count > 0)
                {
                    foreach (Characteristics characteristic in _characteristicList)
                    {
                        input.Age = characteristic.Age;
                        input.BloodPressure = characteristic.BloodPressure;
                        input.SkinThickness = characteristic.SkinThickness; 
                        input.Pregnancies = characteristic.Pregnancies;
                        input.BMI = (float)characteristic.Bmi ;
                        input.DiabetesPedigreeFunction = (float)characteristic.DiabetesPedigreeFunction;    
                        input.Glucose = (float)characteristic.Glucose;
                        input.Insulin = characteristic.Insulin;

                    }

                    ModelOutput? careerPredictions = MLModel.Predict(input);
                    var diabetesScore = careerPredictions.Prediction;

                    ViewBag.Result = diabetesScore *100 ;

                    return View();
                }
                   
                else return RedirectToAction(nameof(Create));
            }

            return Problem("Entity set 'DiabetesPredictorDBContext.Characteristics'  is null.");
        }
    }
}
