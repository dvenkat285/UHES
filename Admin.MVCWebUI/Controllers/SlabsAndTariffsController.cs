using Admin.Infrastructure.Shared.Interfaces.Gateway;
using Admin.Infrastructure.Shared.Models;
using Admin.MVCWebUI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SlabAndTariffInputModel = Admin.Infrastructure.Shared.Models.SlabAndTariffInputModel;

namespace Admin.MVCWebUI.Controllers
{
    public class SlabsAndTariffsController : Controller
    {
        private readonly IHESApiGatewayService hESApiGateway;
        private readonly IMapper mapper;
        private readonly string username = string.Empty;

        public SlabsAndTariffsController(IMapper mapper,
            IHESApiGatewayService hESApiGateway
            )
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.hESApiGateway = hESApiGateway ?? throw new ArgumentException(nameof(hESApiGateway));
        }

        public IActionResult SlabsAndTariffs()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SlabsAndTariffsList()
        {
            return View(); // View is loaded without model, JS will fetch data
        }

        [HttpGet]
        public async Task<IActionResult> GetSlabsList(string listType)
        {
            try
            {
                var slabsList = await this.hESApiGateway.GetSlabsAndTariffsList();

                if (string.IsNullOrWhiteSpace(listType))
                {
                    return Json(new { success = false, message = "listType is required." });
                }

                var dataList = slabsList
                    .Where(t => string.Equals(t.Lines?.Trim(), listType.Trim(), StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(t => t.SubCategoryId)
                    .ToList();

                return Json(new { success = true, data = dataList });
            }
            catch (Exception ex)
            {
                // Ideally log the error
                return Json(new { success = false, message = "Error occurred.", error = ex.Message });
            }
        }


        //get categories for dropdown in insert page

        [Route("GetCategories")]
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await hESApiGateway.GetAllCategoriesAsync();

                if (categories == null || !categories.Any())
                {
                    return Json(new { success = false, message = "No categories found." });
                }

                var mappedResult = categories.Select(category => new Admin.MVCWebUI.Models.CategoryListDto
                {
                    CategoryId = category.CategoryId,
                    Category = category.Category,
                    CategoryName = category.CategoryName,
                    //SubCategoryId = category.SubCategoryId,
                    SubCategory = category.SubCategory,
                    SubCategoryName = category.SubCategoryName,
                    Lines = category.Lines.ToString() // Convert bool to string if needed
                }).ToList();

                return Json(new { success = true, data = mappedResult });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while fetching categories.",
                    error = ex.Message
                });
            }
        }


        [Route("SlabsAndTariffs/CreateSlabs")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSlabs([FromBody] SlabWrapperUnifiedModel slabsModel)
        {
            if (!IsValidSlabsModel(slabsModel, out var validationError))
            {
                return Json(new { success = false, message = validationError, redirectUrl = GetRedirectUrl() });
            }

            try
            {
                var categoryDto = BuildCategoryCreateDto(slabsModel);
                var result = await hESApiGateway.SlabsCreate(categoryDto);

                return Json(new { success = true, message = "Slabs created successfully.", redirectUrl = "/SlabsAndTariffs/SlabsAndTariffsList" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Exception occurred: {ex.Message}", redirectUrl = GetRedirectUrl() });
            }
        }

        // fetch data with ids and redirect for editing in prefilled state
        [HttpGet]
        [Route("SlabsAndTariffs/GetSlab")]
        public async Task<IActionResult> GetSlab(int? categoryId, int? subcategoryId, string lines)
        {
            var model = new SlabsDetailsModel();

            if (categoryId.HasValue && subcategoryId.HasValue && !string.IsNullOrEmpty(lines))
            {
                var slabDto = await hESApiGateway.GetSlabByCategorySubcategoryAndLines(categoryId.Value, subcategoryId.Value, lines);

                if (slabDto != null)
                {
                    model = new SlabsDetailsModel
                    {
                        SlabTariffId = slabDto.SlabTariffId,
                        CategoryId = slabDto.CategoryId,
                        Category = slabDto.Category,
                        CategoryName = slabDto.CategoryName,
                        SubCategoryId = slabDto.SubCategoryId,
                        SubCategory = slabDto.SubCategory,
                        SubCategoryName = slabDto.SubCategoryName,
                        Lines = slabDto.Lines,
                        FromUnits = slabDto.FromUnits ?? 0,
                        ToUnits = slabDto.ToUnits ?? 0,
                        EnergyCharges = slabDto.EnergyCharges ?? 0,
                        UnitParameter = slabDto.UnitParameter,
                        FixedCharges = slabDto.FixedCharges ?? 0,
                        Kv11 = slabDto.Kv11 ?? 0,
                        Kv33 = slabDto.Kv33 ?? 0,
                        Kv132 = slabDto.Kv132 ?? 0,
                        Kv220 = slabDto.Kv220 ?? 0
                    };
                }
            }

            // ❌ PROBLEM: if you're returning default view here, it loads the Create view
            // return View(model);

            // ✅ FIX:
            return View("SlabsEdit", model); // explicitly point to the Edit view
        }

        [Route("SlabsAndTariffs/SlabsUpdate")]
        [HttpPost]
        public async Task<IActionResult> SlabsUpdate([FromBody] SlabWrapperUnifiedUpdateModel updateModel)
        {
            if (updateModel == null)
            {
                return Json(new { success = false, message = "Model binding failed: model is null", redirectUrl = GetRedirectUrl() });
            }

            // ✅ log incoming data
            Log.Information("Received update model: {@updateModel}", updateModel);

            if (!IsValidUpdateModel(updateModel, out var validationError))
            {
                Log.Warning("Validation failed: {ValidationError}", validationError);
                return Json(new { success = false, message = validationError, redirectUrl = GetRedirectUrl() });
            }

            try
            {
                var categoryDto = BuildCategoryUpdateDto(updateModel);
                var result = await hESApiGateway.SlabsUpdate(categoryDto);

                // Optional: check result.IsSuccess
                return Json(new { success = true, message = "Slabs updated successfully.", redirectUrl = GetRedirectUrl() });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception during slab update");
                return Json(new { success = false, message = $"Exception: {ex.Message}", redirectUrl = GetRedirectUrl() });
            }
        }

        private bool IsValidUpdateModel(SlabWrapperUnifiedUpdateModel model, out string errorMessage)
        {
            errorMessage = null;

            if (model == null)
            {
                errorMessage = "Model is null.";
                return false;
            }

            if (model.Subcategories == null || !model.Subcategories.Any())
            {
                errorMessage = "At least one subcategory is required.";
                return false;
            }

            var sub = model.Subcategories.First();

            if (string.IsNullOrWhiteSpace(sub.Lines))
            {
                errorMessage = "Line type is required.";
                return false;
            }

            if (sub.Slabs == null || !sub.Slabs.Any())
            {
                errorMessage = "At least one slab detail is required.";
                return false;
            }

            return true;
        }



        // -------------------------
        // Private Helper Methods
        // -------------------------

        private bool IsValidSlabsModel(SlabWrapperUnifiedModel model, out string errorMessage)
        {
            errorMessage = null;

            if (model == null)
            {
                errorMessage = "Model is null.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Lines))
            {
                errorMessage = "Line type is required.";
                return false;
            }

            if (model.SubCategoriesDetails == null || !model.SubCategoriesDetails.Any())
            {
                errorMessage = "At least one subcategory detail is required.";
                return false;
            }

            return true;
        }

        private string GetRedirectUrl() =>
            Url.Action("SlabsAndTariffsList", "SlabsAndTariffs");

        private CategoryCreateDto BuildCategoryCreateDto(SlabWrapperUnifiedModel slabsModel)
        {
            var lineType = slabsModel.Lines.ToUpper();
            var firstDetail = slabsModel.SubCategoriesDetails.First();

            var subcategory = new SubcategoryCreateDto
            {
                SubCategory = slabsModel.SubCategory,
                SubCategoryName = slabsModel.SubCategoryName ?? slabsModel.SubCategory,
                Lines = lineType,
                BillingUnits = firstDetail.BillingUnits,
                FixedCharges = firstDetail.FixedCharges,
                CustomerCharges = firstDetail.CustomerCharges,
                ElectricityDutyCharges = firstDetail.ElectricityDutyCharges,
                MinimumCharges = firstDetail.MinimumCharges,
                //CrTs = DateTimeOffset.UtcNow,
                Slabs = slabsModel.SubCategoriesDetails.Select(detail => new SlabCreateDto
                {
                    SlabTariffId = 0,
                    //SubcategoryId = 0, // Will be populated in DB
                    From = detail.From,
                    To = detail.To,
                    //Rate = detail.Rate,
                    CrTs = DateTimeOffset.UtcNow,
                }).ToList(),
            };

            if (lineType == "HT")
            {
                subcategory.KV11 = firstDetail._11KV;
                subcategory.KV33 = firstDetail._33KV;
                subcategory.KV132 = firstDetail._132KV;
                subcategory.KV220 = firstDetail._220KV;
            }

            return new CategoryCreateDto
            {
                Category = slabsModel.Category,
                CategoryName = slabsModel.CategoryName ?? $"Category_Descriptn_{DateTime.UtcNow:yyyyMMddHHmmss}",
                //Crts = DateTimeOffset.UtcNow,
                Subcategories = new List<SubcategoryCreateDto> { subcategory },
            };
        }


        private CategoryUpdateDto BuildCategoryUpdateDto(SlabWrapperUnifiedUpdateModel updateModel)
        {
            var sub = updateModel.Subcategories.First();
            var lineType = sub.Lines.ToUpper();

            var subcategory = new SubcategoryUpdateDto
            {
                SubCategoryId = sub.SubCategoryId ?? 0,
                SubCategory = sub.SubCategory,
                SubCategoryName = sub.SubCategoryName,
                Lines = lineType,
                BillingUnits = sub.BillingUnits,
                FixedCharges = sub.FixedCharges ?? 0,
                Slabs = sub.Slabs.Select(slab => new SlabUpdateDto
                {
                    SlabTariffId = slab.SlabTariffId ?? 0,
                    SubcategoryId = sub.SubCategoryId ?? 0,
                    From = slab.From ?? 0,
                    To = slab.To ?? 0,
                    Rate = slab.EnergyCharges ?? 0,
                }).ToList()
            };

            if (lineType == "HT")
            {
                subcategory.KV11 = sub.KV11 ?? 0;
                subcategory.KV33 = sub.KV33 ?? 0;
                subcategory.KV132 = sub.KV132 ?? 0;
                subcategory.KV220 = sub.KV220 ?? 0;
            }

            return new CategoryUpdateDto
            {
                CategoryId = updateModel.CategoryId ?? 0,
                Category = updateModel.Category,
                CategoryName = updateModel.CategoryName,
                Subcategories = new List<SubcategoryUpdateDto> { subcategory }
            };
        }
    }
}