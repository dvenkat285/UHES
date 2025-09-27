const model = window.prefillModel;

$(document).ready(function () {

    loadAllDropdownData().done(function (res) {
        if (!res.success) {
            console.error(res.message);
            return;
        }

        const allItems = res.data;

        const $category = $('#categorySelect');
        const $subCategory = $('#subCategorySelect');
        const $supplyType = $('#supplyTypeSelect');

        // Populate Category
        $category.empty().append('<option value="">Select Category</option>');
        const categories = [...new Map(allItems.map(item =>
            [item.categoryId, item]
        )).values()];

        categories.forEach(cat => {
            $category.append(`<option value="${cat.categoryId}">${cat.categoryName}</option>`);
        });

        // Preselect Category
        $category.val(String(model.categoryId)).trigger('change');

        // Populate and Preselect Subcategory
        $subCategory.empty().append('<option value="">Select Sub Category</option>');
        const filteredSubcategories = allItems.filter(item => item.categoryId == model.categoryId);

        filteredSubcategories.forEach(sub => {
            $subCategory.append(`<option value="${sub.subCategoryId}">${sub.subCategoryName}</option>`);
        });

        $subCategory.val(String(model.subCategoryId)).trigger('change');

        // Preselect Supply Type
        $supplyType.val(model.lines).trigger('change');
    });

    // Pre-fill input fields for edit mode
    if (window.isEditMode) {
        $('#fromUnitsInput').val(model.fromUnits);
        $('#toUnitsInput').val(model.toUnits);
        $('#energyChargesInput').val(model.energyCharges);
        $('#unitParameterInput').val(model.unitParameter);
        $('#fixedChargesInput').val(model.fixedCharges);
        $('#kv11Input').val(model.kv11);
        $('#kv33Input').val(model.kv33);
        $('#kv132Input').val(model.kv132);
        $('#kv220Input').val(model.kv220);
        $('#createBtn').hide(); // Optional: hide 'Create' button
    }

    // Save button click handler
    $('#saveBtn').on('click', function () {
        const isHT = $('#supplyTypeSelect').val() === 'HT';
        const slabDetails = [];

        $('#slabTable tbody tr').each(function () {
            const $row = $(this);
            const detail = {
                SlabTariffId: model.slabTariffId || 0, // for new entries it's 0
                From: parseFloat($row.find('.fromInput').val()) || 0,
                To: parseFloat($row.find('.toInput').val()) || 0,
                Rate: parseFloat($row.find('.rateInput').val()) || 0,
                BillingUnits: $row.find('.billingUnitsInput').val(),
                FixedCharges: parseFloat($row.find('.fixedChargesInput').val()) || 0,
                _11KV: isHT ? parseFloat($row.find('.kv11Input').val()) || 0 : 0,
                _33KV: isHT ? parseFloat($row.find('.kv33Input').val()) || 0 : 0,
                _132KV: isHT ? parseFloat($row.find('.kv132Input').val()) || 0 : 0,
                _220KV: isHT ? parseFloat($row.find('.kv220Input').val()) || 0 : 0
            };
            slabDetails.push(detail);
        });

        const slabData = {
            CategoryId: parseInt($('#categorySelect').val()) || 0,
            Category: model.category, // or get from somewhere else
            CategoryName: $('#categorySelect option:selected').text(),
            SubCategoryId: parseInt($('#subCategorySelect').val()) || 0,
            SubCategory: model.subCategory, // or get from somewhere else
            SubCategoryName: $('#subCategorySelect option:selected').text(),
            Lines: $('#supplyTypeSelect').val(),
            SubCategoriesDetails: slabDetails
        };

        $.ajax({
            url: '/SlabsAndTariffs/SlabsUpdate',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(slabData),
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                if (res.success) {
                    window.location.href = res.redirectUrl;
                } else {
                    alert(res.message);
                }
            },
            error: function () {
                alert("An error occurred while saving changes.");
            }
        });
    });
});

function loadAllDropdownData() {
    return $.ajax({
        url: '/SlabsAndTariffs/CategoriesList',
        method: 'GET'
    });
}
if (window.isEditMode) {
    console.log("Edit Mode Model:", model);

    $('#slabTableContainer').removeClass('d-none');

    const isLT = model.lines === 'LT';
    generateTable(isLT);

    // Fill the first row that was generated
    const $row = $('#slabTable tbody tr').first();

    $row.find('.fromInput').val(model.fromUnits);
    $row.find('.toInput').val(model.toUnits);
    $row.find('.rateInput').val(model.energyCharges);
    $row.find('.billingUnitsInput').val(model.unitParameter);
    $row.find('.fixedChargesInput').val(model.fixedCharges);
    $row.find('.kv11Input').val(model.kv11);
    $row.find('.kv33Input').val(model.kv33);
    $row.find('.kv132Input').val(model.kv132);
    $row.find('.kv220Input').val(model.kv220);

    $('#createBtn').hide();
}
function generateTable(isLT) {
    const thead = document.getElementById("tableHead");
    const tbody = document.getElementById("tableBody");

    const columns = isLT
        ? ["Slab From", "Slab To", "Billing Unit", "LT Fixed Charges", "LT Rate", "Add", "Delete"]
        : ["Slab From", "Slab To", "Billing Unit", "HT Fixed Charges", "HT Rate", "HT 11kV", "HT 33kV", "HT 132 kV", "HT 220 kV", "Add", "Delete"];

    thead.innerHTML = "";
    const headRow = document.createElement("tr");
    columns.forEach(col => {
        const th = document.createElement("th");
        th.textContent = col;
        headRow.appendChild(th);
    });
    thead.appendChild(headRow);

    tbody.innerHTML = "";
    addNewRow(isLT);
}

function addNewRow(isLT) {
    const tbody = document.getElementById("tableBody");

    const row = document.createElement("tr");
    row.innerHTML = `
        <td><input type="number" class="form-control fromInput" min="0" /></td>
        <td><input type="number" class="form-control toInput" min="0" /></td>
        <td>
            <select class="form-select billingUnitsInput">
                <option value="kWh">kWh</option>
                <option value="kVAh">kVAh</option>
            </select>
        </td>
    `;

    if (isLT) {
        row.innerHTML += `
            <td><input type="number" class="form-control fixedChargesInput" min="0" step="0.01" /></td>
            <td><input type="number" class="form-control rateInput" min="0" step="0.01" /></td>
        `;
    } else {
        row.innerHTML += `
            <td><input type="number" class="form-control fixedChargesInput" min="0" step="0.01" /></td>
            <td><input type="number" class="form-control rateInput" min="0" step="0.01" /></td>
            <td><input type="number" class="form-control kv11Input" min="0" step="0.01" /></td>
            <td><input type="number" class="form-control kv33Input" min="0" step="0.01" /></td>
            <td><input type="number" class="form-control kv132Input" min="0" step="0.01" /></td>
            <td><input type="number" class="form-control kv220Input" min="0" step="0.01" /></td>
        `;
    }

    row.innerHTML += `
        <td class="text-center"><i class="fas fa-plus text-success addRow" style="cursor:pointer;"></i></td>
        <td class="text-center"><i class="fas fa-trash text-danger deleteRow" style="cursor:pointer;"></i></td>
    `;

    tbody.appendChild(row);
}

