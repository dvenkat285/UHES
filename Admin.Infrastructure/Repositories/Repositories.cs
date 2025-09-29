using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Admin.Infrastructure.Shared.Models;
using Dapper;
//using Admin.Application.Features;



namespace Admin.Infrastructure.Repositories
{
    public class Repositories
    {
        private readonly uhesDapperContext _context;

        public Repositories(uhesDapperContext context)
        {
            _context = context;
        }

        public async Task<List<SlabsDetailsModel>> GetAllSubcategoryWiseSlabTariffDetails()
        {
            const string sql = @"
                SELECT 
                    c.CATEGORY_ID AS CategoryId,
                    c.CATEGORY_NAME AS CategoryName,
                    sc.SubCategoryId AS SubCategoryId,
                    sc.SubCategoryName AS SubCategoryName,
                    c.CATEGORY as Category,
                    sc.SubCategory as SubCategory,
                    sc.SubCategory_No AS SubCategoryNumber,
                    st.from_Units AS FromUnits,
                    st.To_Units AS ToUnits,
                    st.lines AS Lines,
                    st.unit_Parameters AS UnitParameter,
                    st.energy_charges AS EnergyCharges,
                    st.Fixed_Charges AS FixedCharges,
                    st.Customer_Charges AS CustomerCharges,
                    st.KV_11 AS Kv11,
                    st.KV_33 AS Kv33,
                    st.KV_132 AS Kv132,
                    st.Kv_220 AS Kv220,
                    st.Electricity_duty_Charges AS ElectricityDutyCharges,
                    st.Minimum_Charges AS MinimumCharges
                FROM Et_slab_tariff_details st
                JOIN ET_CATEGORY_DETAILS c ON st.categoryId = c.CATEGORY_ID
                JOIN ET_SUBCATEGORY_DETAILS sc ON sc.SubCategoryId = st.SubCategoryId
                WHERE sc.CategoryId = c.CATEGORY_ID";

            using var connection = _context.CreateConnection();
            var slabs = (await connection.QueryAsync<SlabsDetailsModel>(sql)).ToList();

            var result = new List<SlabsDetailsModel>();

            if (slabs.Any())
            {
                var subCategories = slabs.Where(t => t.SubCategoryId != null && t.SubCategoryId >= 0)
                                        .Select(t => t.SubCategoryId)
                                        .Distinct();

                var lineTypes = slabs.Where(t => !string.IsNullOrEmpty(t.Lines))
                                    .Select(t => t.Lines)
                                    .Distinct();

                foreach (var subCategoryId in subCategories)
                {
                    foreach (var lineType in lineTypes)
                    {
                        var dataList = slabs.Where(t => t.SubCategoryId == subCategoryId && t.Lines == lineType);

                        if (dataList.Any())
                        {
                            var first = dataList.First();
                            result.Add(new SlabsDetailsModel
                            {
                                SubCategoryId = first.SubCategoryId,
                                CategoryId = first.CategoryId,
                                SubCategoryName = first.SubCategoryName,
                                CategoryName = first.CategoryName,
                                Category = first.Category,
                                SubCategory = first.SubCategory,
                                Lines = first.Lines,
                                SubCategoryNumber = first.SubCategoryNumber,
                                SlabTariffDetails = dataList.OrderBy(t => t.SlabTariffId).ToList()
                            });
                        }
                    }
                }
            }

            return result.OrderByDescending(t => t.SubCategoryId).ToList();
        }

        public async Task<SlabsDetailsModel?> GetSlabByCategorySubcategoryAndLinesAsync(int categoryId, int subCategoryId, string lines, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    d.SlabTariffId,
                    d.CategoryId,
                    d.SubCategoryId,
                    d.From_Units AS FromUnits,
                    d.To_Units AS ToUnits,
                    d.Lines,
                    d.Unit_Parameters AS UnitParameter,
                    d.Energy_Charges AS EnergyCharges,
                    d.Fixed_Charges AS FixedCharges,
                    d.Customer_Charges AS CustomerCharges,
                    d.KV_11 AS Kv11,
                    d.KV_33 AS Kv33,
                    d.KV_132 AS Kv132,
                    d.KV_220 AS Kv220,
                    d.Electricity_Duty_Charges AS ElectricityDutyCharges,
                    d.Minimum_Charges AS MinimumCharges,
                    c.CATEGORY AS Category,
                    c.CATEGORY_NAME AS CategoryName,
                    sc.SubCategory AS SubCategory,
                    sc.SubCategoryName AS SubCategoryName
                FROM slabsNtariffs.dbo.ET_SLAB_TARIFF_DETAILS d
                LEFT JOIN slabsNtariffs.dbo.ET_CATEGORY_DETAILS c ON d.CategoryId = c.CATEGORY_ID
                LEFT JOIN slabsNtariffs.dbo.ET_SUBCATEGORY_DETAILS sc ON d.SubCategoryId = sc.SubCategoryId
                WHERE d.CategoryId = @CategoryId AND d.SubCategoryId = @SubCategoryId AND d.Lines = @Lines";

            using var connection = _context.CreateConnection();
            var slab = await connection.QueryFirstOrDefaultAsync<SlabsDetailsModel>(
                new CommandDefinition(sql, new { CategoryId = categoryId, SubCategoryId = subCategoryId, Lines = lines }, cancellationToken: cancellationToken)
            );

            return slab;
        }

        public async Task<List<CategoryListDto>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    c.CATEGORY_ID AS CategoryId,
                    c.CATEGORY AS Category,
                    c.CATEGORY_NAME AS CategoryName,
                    s.SubCategoryId,
                    s.SubCategory,
                    s.SubCategoryName,
                    s.CategoryId,
                    s.Lines
                FROM ET_CATEGORY_DETAILS c
                JOIN ET_SUBCATEGORY_DETAILS s ON c.CATEGORY_ID = s.CategoryId";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<CategoryListDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));

            return result.ToList();
        }



        public async Task<CategoryCreateResultDto> CreateSlabWithCategoryAndSubcategoryAsync(CategoryCreateDto request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateCreateDTO(request);

            if (request.Subcategories == null || !request.Subcategories.Any())
                throw new ArgumentException("At least one subcategory is required");

            //using var connection = uhesDapperContext.CreateConnection();
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                int categoryId;

                if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
                {
                    // Use existing CategoryId
                    categoryId = request.CategoryId.Value;
                }
                else
                {
                    // Insert new category
                    categoryId = await connection.ExecuteScalarAsync<int>(@"
                INSERT INTO ET_CATEGORY_DETAILS (Category, [CATEGORY_NAME], Crts)
                VALUES (@Category, @CategoryName, @Crts);
                SELECT CAST(SCOPE_IDENTITY() as int);",
                        new
                        {
                            request.Category,
                            CategoryName = request.CategoryName,
                            Crts = DateTimeOffset.UtcNow
                        }, transaction);
                }

                int? firstSubcategoryId = null;
                int? firstSlabTariffId = null;

                foreach (var subcategory in request.Subcategories)
                {
                    int subcategoryId;

                    if (subcategory.SubCategoryId.HasValue && subcategory.SubCategoryId.Value > 0)
                    {
                        // Use existing SubCategoryId
                        subcategoryId = subcategory.SubCategoryId.Value;
                    }
                    else
                    {
                        // Insert new subcategory
                        subcategoryId = await connection.ExecuteScalarAsync<int>(@"
                    INSERT INTO ET_SUBCATEGORY_DETAILS 
                    ([SubCategory], [SubCategoryName], [Lines], [CategoryId], [CrTs])
                    VALUES 
                    (@SubCategory, @SubCategoryName, @Lines, @CategoryId, @CrTs);
                    SELECT CAST(SCOPE_IDENTITY() as int);",
                            new
                            {
                                subcategory.SubCategory,
                                subcategory.SubCategoryName,
                                subcategory.Lines,
                                CategoryId = categoryId,
                                CrTs = DateTimeOffset.UtcNow
                            }, transaction);
                    }

                    firstSubcategoryId ??= subcategoryId;

                    if (subcategory.Slabs == null || !subcategory.Slabs.Any())
                        continue;

                    foreach (var slab in subcategory.Slabs)
                    {
                        var slabTariffId = await connection.ExecuteScalarAsync<int>(
                            "SELECT ISNULL(MAX(SlabTariffId), 0) + 1 FROM ET_SLAB_TARIFF_DETAILS",
                            null, transaction);

                        await connection.ExecuteAsync(@"
                    INSERT INTO ET_SLAB_TARIFF_DETAILS(
                        CategoryId,
                        SubCategoryId,
                        From_Units,
                        To_Units,
                        Lines,
                        Unit_Parameters,
                        Energy_Charges,
                        Fixed_Charges,
                        Customer_Charges,
                        KV_11,
                        KV_33,
                        KV_132,
                        KV_220,
                        Electricity_Duty_Charges,
                        Minimum_Charges,
                        CrTs
                    )
                    VALUES(
                        @CategoryId,
                        @SubCategoryId,
                        @FromUnits,
                        @ToUnits,
                        @Lines,
                        @UnitParameters,
                        @EnergyCharges,
                        @FixedCharges,
                        @CustomerCharges,
                        @KV11,
                        @KV33,
                        @KV132,
                        @KV220,
                        @ElectricityDutyCharges,
                        @MinimumCharges,
                        @CrTs
                    );",
                            new
                            {
                                CategoryId = categoryId,
                                SubCategoryId = subcategoryId,
                                FromUnits = slab.From,
                                ToUnits = slab.To,
                                Lines = subcategory.Lines,
                                UnitParameters = subcategory.BillingUnits,
                                //EnergyCharges = slab.Rate ?? 0,
                                EnergyCharges = slab.EnergyCharges ?? 0,

                                FixedCharges = subcategory.FixedCharges ?? 0,
                                CustomerCharges = subcategory.CustomerCharges ?? 0,
                                KV11 = subcategory.KV11 ?? 0,
                                KV33 = subcategory.KV33 ?? 0,
                                KV132 = subcategory.KV132 ?? 0,
                                KV220 = subcategory.KV220 ?? 0,
                                ElectricityDutyCharges = subcategory.ElectricityDutyCharges ?? 0,
                                MinimumCharges = subcategory.MinimumCharges ?? 0,
                                CrTs = slab.CrTs ?? DateTimeOffset.UtcNow
                            }, transaction);

                        firstSlabTariffId ??= slabTariffId;
                    }
                }

                transaction.Commit();

                return new CategoryCreateResultDto
                {
                    CategoryId = categoryId,
                    SubCategoryId = firstSubcategoryId ?? 0,
                    SlabTariffId = firstSlabTariffId ?? 0
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void ValidateCreateDTO(CategoryCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Category?.Length > 15)
                throw new ArgumentException("Category exceeds 15 characters");

            if (dto.CategoryName?.Length > 150)
                throw new ArgumentException("CategoryName exceeds 150 characters");

            var subcategory = dto.Subcategories?.FirstOrDefault();
            if (subcategory == null)
                throw new ArgumentException("At least one Subcategory is required");

            if (subcategory.SubCategory?.Length > 35)
                throw new ArgumentException("SubCategory exceeds 35 characters");

            if (subcategory.SubCategoryName?.Length > 250)
                throw new ArgumentException("SubCategoryName exceeds 250 characters");

            if (subcategory.Lines?.Length > 5)
                throw new ArgumentException("Lines exceeds 5 characters");

            if (subcategory.BillingUnits?.Length > 5)
                throw new ArgumentException("BillingUnits exceeds 5 characters");

            var slab = subcategory.Slabs?.FirstOrDefault();
            if (slab == null)
                throw new ArgumentException("At least one Slab is required");


        }

        public async Task<CategoryUpdateResultDto> UpdateSlabWithCategoryAndSubcategoryAsync(CategoryUpdateDto request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateUpdateDTO(request);

            if (request.Subcategories == null || !request.Subcategories.Any())
                throw new ArgumentException("At least one subcategory is required");

            //using var connection = uhesDapperContext.CreateConnection();
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                if (!request.CategoryId.HasValue || request.CategoryId.Value <= 0)
                    throw new ArgumentException("Valid CategoryId is required for update.");

                int categoryId = request.CategoryId.Value;

                // Update Category
                await connection.ExecuteAsync(@"
                    UPDATE ET_CATEGORY_DETAILS
                    SET Category = @Category,
                        CATEGORY_NAME = @CategoryName
                    WHERE CATEGORY_ID = @CategoryId;",
                    new
                    {
                        request.Category,
                        CategoryName = request.CategoryName,
                        CategoryId = categoryId
                    }, transaction);

                int? firstSubcategoryId = null;
                int? firstSlabTariffId = null;

                foreach (var subcategory in request.Subcategories)
                {
                    if (!subcategory.SubCategoryId.HasValue || subcategory.SubCategoryId.Value <= 0)
                        throw new ArgumentException("Valid SubCategoryId is required for update.");

                    int subcategoryId = subcategory.SubCategoryId.Value;

                    // Update Subcategory
                    await connection.ExecuteAsync(@"
                        UPDATE ET_SUBCATEGORY_DETAILS
                        SET SubCategory = @SubCategory,
                            SubCategoryName = @SubCategoryName,
                            Lines = @Lines,
                            CategoryId = @CategoryId
                        WHERE SubCategoryId = @SubCategoryId;",
                        new
                        {
                            subcategory.SubCategory,
                            subcategory.SubCategoryName,
                            subcategory.Lines,
                            CategoryId = categoryId,
                            SubCategoryId = subcategoryId
                        }, transaction);

                    firstSubcategoryId ??= subcategoryId;

                    if (subcategory.Slabs == null || !subcategory.Slabs.Any())
                        continue;

                    foreach (var slab in subcategory.Slabs)
                    {
                        if (!slab.SlabTariffId.HasValue || slab.SlabTariffId <= 0)
                            throw new ArgumentException("Valid SlabTariffId is required for update.");

                        int slabTariffId = slab.SlabTariffId.Value;

                        // Update Slab
                        await connection.ExecuteAsync(@"
                                UPDATE ET_SLAB_TARIFF_DETAILS
                                SET From_Units = @FromUnits,
                                    To_Units = @ToUnits,
                                    Lines = @Lines,
                                    Unit_Parameters = @UnitParameters,
                                    Energy_Charges = @EnergyCharges,
                                    Fixed_Charges = @FixedCharges,
                                    Customer_Charges = @CustomerCharges,
                                    KV_11 = @KV11,
                                    KV_33 = @KV33,
                                    KV_132 = @KV132,
                                    KV_220 = @KV220,
                                    Electricity_Duty_Charges = @ElectricityDutyCharges,
                                    Minimum_Charges = @MinimumCharges
                                WHERE SlabTariffId = @SlabTariffId AND SubCategoryId = @SubCategoryId AND CategoryId = @CategoryId;",
                            new
                            {
                                SlabTariffId = slabTariffId,
                                CategoryId = categoryId,
                                SubCategoryId = subcategoryId,
                                FromUnits = slab.From,
                                ToUnits = slab.To,
                                Lines = subcategory.Lines,
                                UnitParameters = subcategory.BillingUnits,
                                //EnergyCharges = slab.Rate ?? 0,
                                EnergyCharges = slab.EnergyCharges ?? 0,

                                FixedCharges = subcategory.FixedCharges ?? 0,
                                CustomerCharges = subcategory.CustomerCharges ?? 0,
                                KV11 = subcategory.KV11 ?? 0,
                                KV33 = subcategory.KV33 ?? 0,
                                KV132 = subcategory.KV132 ?? 0,
                                KV220 = subcategory.KV220 ?? 0,
                                ElectricityDutyCharges = subcategory.ElectricityDutyCharges ?? 0,
                                MinimumCharges = subcategory.MinimumCharges ?? 0
                            }, transaction);

                        firstSlabTariffId ??= slabTariffId;
                    }
                }

                transaction.Commit();

                return new CategoryUpdateResultDto
                {
                    CategoryId = request.CategoryId.Value,
                    SubCategoryId = firstSubcategoryId ?? 0,
                    SlabTariffId = firstSlabTariffId ?? 0
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        private void ValidateUpdateDTO(CategoryUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (!dto.CategoryId.HasValue || dto.CategoryId.Value <= 0)
                throw new ArgumentException("Valid CategoryId is required for update");

            if (dto.Category?.Length > 15)
                throw new ArgumentException("Category exceeds 15 characters");

            if (dto.CategoryName?.Length > 150)
                throw new ArgumentException("CategoryName exceeds 150 characters");

            var subcategory = dto.Subcategories?.FirstOrDefault();
            if (subcategory == null)
                throw new ArgumentException("At least one Subcategory is required");

            if (!subcategory.SubCategoryId.HasValue || subcategory.SubCategoryId.Value <= 0)
                throw new ArgumentException("Valid SubCategoryId is required for update");

            if (subcategory.SubCategory?.Length > 25)
                throw new ArgumentException("SubCategory exceeds 25 characters");

            if (subcategory.SubCategoryName?.Length > 250)
                throw new ArgumentException("SubCategoryName exceeds 250 characters");

            if (subcategory.Lines?.Length > 5)
                throw new ArgumentException("Lines exceeds 5 characters");

            if (subcategory.BillingUnits?.Length > 5)
                throw new ArgumentException("BillingUnits exceeds 5 characters");

            var slab = subcategory.Slabs?.FirstOrDefault();
            if (slab == null)
                throw new ArgumentException("At least one Slab is required");

            if (!slab.SlabTariffId.HasValue || slab.SlabTariffId.Value <= 0)
                throw new ArgumentException("Valid SlabTariffId is required for update");
        }



    }
}
