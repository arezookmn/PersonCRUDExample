using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace PersonCRUDExample.Controllers
{
    [Route("[controller]")]
    public class PersonController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        public PersonController(IPersonsService personsService , ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("[action]")]
        [Route("/")]

        public async Task<IActionResult> Index(string searchBy, string? searchString , string sortBy = "PersonName",
            SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>() 
            {
                {nameof(PersonResponse.PersonName), "Person Name"},
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryId), "Country" },
                { nameof(PersonResponse.Address), "Address" }
            };
            List<PersonResponse> persons = await
                _personsService.GetFilteredPerson(searchBy, searchString);
            List<PersonResponse> sortedPersons =await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.SortFields = sortedPersons;
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(persons);
        }


        //Executes when the user clicks on "Create Person" hyperlink (while opening the create view)
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem() 
            { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async  Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)//reload same view and show error messages
            {
                List<CountryResponse> countries =await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp => new SelectListItem()
                { Text = temp.CountryName, Value = temp.CountryId.ToString() });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).SelectMany(e => e.ErrorMessage).ToList();

                return View("create");
            }

            _personsService.AddPerson(personAddRequest); 
            return RedirectToAction( "Index","Person");//it makes another get request
        }

        [Route("[action]/{personId}")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? person = await _personsService.GetPersonByPersonID(personId);
            if(person == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdate = person.ToPersonUpdateRequest();
            List<CountryResponse> countries =await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            { Text = temp.CountryName, Value = temp.CountryId.ToString() });
            return View(personUpdate);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            if(ModelState.IsValid)
            {
                PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries =await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp => new SelectListItem()
                { Text = temp.CountryName, Value = temp.CountryId.ToString() });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).SelectMany(e => e.ErrorMessage).ToList();

                return View(personResponse.ToPersonUpdateRequest());

            }
        }

        [Route("[action]/{personId}")]
        [HttpGet] 
        public async Task<IActionResult> Delete(Guid personId)
        {
            PersonResponse? personResponse =await _personsService.GetPersonByPersonID(personId);
            if(personResponse == null)
            { 
                return RedirectToAction("Index");
            }
            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse =await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
           if(personResponse == null)
            {
                return RedirectToAction("Index");
            }
           await _personsService.DeletePerson(personResponse.PersonId);
            return RedirectToAction("Index");
        }

    } 
}
