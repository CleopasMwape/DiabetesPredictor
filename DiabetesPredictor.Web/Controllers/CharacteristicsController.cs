
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static DiabatesPredictor.MLModel;
using DiabatesPredictor;
using Microsoft.AspNetCore.Authorization;

namespace DiabetesPredictor.Web
{
    [Authorize]
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
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

         

            var _characteristics = await _context.Characteristics
                .Where(m => m.UserId == _userId).ToListAsync();


            if (_characteristics != null && _characteristics.Count>=5)
            {


                return View(_characteristics);
            }

                           
            

            return RedirectToAction(nameof(Create));
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
      
        public async Task<IActionResult> Predict()
        {
            ModelInput input = new ModelInput();

            List<int> ageList = new List<int>();
            List<int> bloodPressureList = new List<int>();
            List<int> skinThicknessList = new List<int>();
            List<int> pregnancyList = new List<int>();
            List<double> bmiList = new List<double>();
            List<double> diabetesPedigreeFunctionList = new List<double>();
            List<int> glucoseList = new List<int>();
            List<int> insulinList = new List<int>();



            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var _characteristicsList = await _context.Characteristics
                        .Where(m => m.UserId == _userId).ToListAsync();

            if (_characteristicsList != null )
            {
                if (_characteristicsList.Count >= 5)
                {
                    foreach (Characteristics characteristic in _characteristicsList)
                    {
                        ageList.Add(characteristic.Age);
                        bloodPressureList.Add(characteristic.BloodPressure);    
                        skinThicknessList.Add(characteristic.SkinThickness);
                        pregnancyList.Add(characteristic.Pregnancies);
                        diabetesPedigreeFunctionList.Add(characteristic.DiabetesPedigreeFunction);
                        insulinList.Add(characteristic.Insulin);
                        glucoseList.Add(characteristic.Glucose);
                        bmiList.Add(characteristic.Bmi);

                    }

                    //extract trending feature values for each feature
                    //

                    int pregnancies = MostCommon(pregnancyList);
                    int age = MostCommon(ageList);
                    int insulin = MostCommon(insulinList);
                    int glucose = MostCommon(glucoseList);
                    double bmi = MostCommon(bmiList);
                    double diabetesPedigreeFunction = MostCommon(diabetesPedigreeFunctionList);
                    int bloodPressure = MostCommon(bloodPressureList);
                    double skinThickness = MostCommon(skinThicknessList);

                    //feed feature values to model input
                    //

                    

                    input.Age = age;
                    input.BloodPressure = bloodPressure;
                    input.SkinThickness = (float)skinThickness;
                    input.Pregnancies = pregnancies;
                    input.BMI = (float)bmi;
                    input.DiabetesPedigreeFunction = (float)diabetesPedigreeFunction;
                    input.Glucose = (float)glucose;
                    input.Insulin = insulin;

                    //feed the model input into predict method to get prediction
                    //
                    ModelOutput? diabetesPredictions = MLModel.Predict(input);
                    var diabetesScore = diabetesPredictions.Prediction;

                    ViewBag.Prediction = diabetesScore*100 ;

                    return View();
                }
                   
                else return RedirectToAction(nameof(Create));
            }

            return Problem("Entity set 'DiabetesPredictorDBContext.Characteristics'  is null.");
        }

        public  int MostCommon(List<int> list)
        {
            return list.GroupBy(i => i).OrderByDescending(grp => grp.Count())
                                               .Select(grp => grp.Key).First();
        }

        public double MostCommon(List<double> list)
        {
            return list.GroupBy(i => i).OrderByDescending(grp => grp.Count())
                                               .Select(grp => grp.Key).First();
        }
    }
}
