


//document.addEventListener("DOMContentLoaded", function () {
//    if (!window.isEditMode) return;

//    const rawModel = window.prefillModel || {};
//    const model = {
//        categoryId: rawModel.categoryId || rawModel.CategoryId || "",
//        subCategoryId: rawModel.subCategoryId || rawModel.SubCategoryId || "",
//        lines: rawModel.lines || rawModel.Lines || "",
//        fromUnits: rawModel.fromUnits || rawModel.FromUnits || "",
//        toUnits: rawModel.toUnits || rawModel.ToUnits || "",
//        energyCharges: rawModel.energyCharges || rawModel.EnergyCharges || "",
//        unitParameter: rawModel.unitParameter || rawModel.UnitParameter || "",
//        fixedCharges: rawModel.fixedCharges || rawModel.FixedCharges || "",
//        kv11: rawModel.kv11 || rawModel.Kv11 || "",
//        kv33: rawModel.kv33 || rawModel.Kv33 || "",
//        kv132: rawModel.kv132 || rawModel.Kv132 || "",
//        kv220: rawModel.kv220 || rawModel.Kv220 || "",
//        slabTariffId: rawModel.slabTariffId || rawModel.SlabTariffId || 0,
//    };

//    let allCategories = [];
//    let allSubcategories = [];
//    let resData = [];

//    // ✅ Load categories/subcategories, populate dropdowns
//    function loadDropdownsAndPrefill() {
//        return new Promise((resolve, reject) => {
//            $.get('/GetCategories', function (res) {
//                if (!res.success) {
//                    alert('Failed to load categories.');
//                    reject();
//                    return;
//                }

//                resData = res.data;

//                resData.forEach(item => {
//                    if (!allCategories.some(cat => cat.categoryId === item.categoryId)) {
//                        allCategories.push({
//                            categoryId: item.categoryId,
//                            categoryName: item.category,
//                            lines: item.lines
//                        });
//                    }

//                    allSubcategories.push({
//                        categoryId: item.categoryId,
//                        subCategoryId: item.subCategoryId,
//                        subCategoryName: item.subCategoryName,
//                        lines: item.lines
//                    });
//                });

//                populateDropdowns();
//                resolve();
//            }).fail(function () {
//                alert("Could not load category data.");
//                reject();
//            });
//        });
//    }

//    // ✅ Populate dropdowns based on current model
//    function populateDropdowns() {
//        const $supplyTypeSelect = $('#supplyTypeSelect');
//        const $categorySelect = $('#categorySelect');
//        const $subCategorySelect = $('#subCategorySelect');

//        const supplyType = model.lines;

//        // Populate supply type dropdown if not already populated
//        if ($supplyTypeSelect.find('option').length === 0) {
//            $supplyTypeSelect.html(`
//                <option value="">Select Supply Type</option>
//                <option value="LT">LT</option>
//                <option value="HT">HT</option>
//            `);
//        }

//        $supplyTypeSelect.val(supplyType).trigger('change');

//        // Populate categories
//        const filteredCategories = allCategories.filter(cat => cat.lines === supplyType);
//        $categorySelect.empty().append('<option value="">Select Category</option>');
//        filteredCategories.forEach(cat => {
//            $categorySelect.append(`<option value="${cat.categoryId}">${cat.categoryName}</option>`);
//        });

//        if (filteredCategories.some(cat => cat.categoryId == model.categoryId)) {
//            $categorySelect.val(model.categoryId).trigger('change');
//        }

//        // Populate subcategories
//        const filteredSubcategories = allSubcategories.filter(sub =>
//            sub.categoryId == model.categoryId && sub.lines === supplyType
//        );
//        $subCategorySelect.empty().append('<option value="">Select Subcategory</option>');
//        filteredSubcategories.forEach(sub => {
//            $subCategorySelect.append(`<option value="${sub.subCategoryId}">${sub.subCategoryName}</option>`);
//        });

//        if (filteredSubcategories.some(sub => sub.subCategoryId == model.subCategoryId)) {
//            $subCategorySelect.val(model.subCategoryId).trigger('change');
//        }

//        toggleCreateButton();
//    }

//    // ✅ Enable or disable create/update button
//    function toggleCreateButton() {
//        const catSelected = $('#categorySelect').val();
//        const subCatSelected = $('#subCategorySelect').val();
//        $('#createBtn').prop('disabled', !(catSelected && subCatSelected));
//    }

//    // ✅ Generate table and add initial row
//    function generateTable(isLT, addInitialRow = true) {
//        const thead = document.getElementById("tableHead");
//        const tbody = document.getElementById("tableBody");

//        const columns = isLT
//            ? ["Slab From", "Slab To", "Billing Unit", "LT Fixed Charges", "LT Rate", "Add", "Delete"]
//            : ["Slab From", "Slab To", "Billing Unit", "HT Fixed Charges", "HT Rate", "HT 11kV", "HT 33kV", "HT 132 kV", "HT 220 kV", "Add", "Delete"];

//        thead.innerHTML = "";
//        const headRow = document.createElement("tr");
//        columns.forEach(col => {
//            const th = document.createElement("th");
//            th.textContent = col;
//            headRow.appendChild(th);
//        });
//        thead.appendChild(headRow);

//        tbody.innerHTML = "";

//        if (addInitialRow) {
//            addNewRow(isLT);
//        }
//    }

//    // ✅ Add empty row
//    function addNewRow(isLT) {
//        const tbody = document.getElementById("tableBody");
//        const row = document.createElement("tr");

//        row.innerHTML = `
//            <td><input type="number" class="form-control fromInput" min="0" /></td>
//            <td><input type="number" class="form-control toInput" min="0" /></td>
//            <td>
//                <select class="form-select billingUnitsInput">
//                    <option value="kWh">kWh</option>
//                    <option value="kVAh">kVAh</option>
//                </select>
//            </td>
//        `;

//        if (isLT) {
//            row.innerHTML += `
//                <td><input type="number" class="form-control fixedChargesInput" min="0" step="0.01" /></td>
//                <td><input type="number" class="form-control rateInput" min="0" step="0.01" /></td>
//            `;
//        } else {
//            row.innerHTML += `
//                <td><input type="number" class="form-control fixedChargesInput" min="0" step="0.01" /></td>
//                <td><input type="number" class="form-control rateInput" min="0" step="0.01" /></td>
//                <td><input type="number" class="form-control kv11Input" min="0" step="0.01" /></td>
//                <td><input type="number" class="form-control kv33Input" min="0" step="0.01" /></td>
//                <td><input type="number" class="form-control kv132Input" min="0" step="0.01" /></td>
//                <td><input type="number" class="form-control kv220Input" min="0" step="0.01" /></td>
//            `;
//        }

//        row.innerHTML += `
//            <td class="text-center"><i class="fas fa-plus text-success addRow" style="cursor:pointer;"></i></td>
//            <td class="text-center"><i class="fas fa-trash text-danger deleteRow" style="cursor:pointer;"></i></td>
//        `;

//        tbody.appendChild(row);
//    }

//    // ✅ Handle Add/Delete row buttons
//    $(document).on('click', '.addRow', function () {
//        const isLT = $('#supplyTypeSelect').val() === 'LT';
//        addNewRow(isLT);
//    });

//    $(document).on('click', '.deleteRow', function () {
//        $(this).closest('tr').remove();
//    });

//    $('#saveBtn').on('click', function (e) {
//        e.preventDefault();
//        saveSlabs(); // You already have this function in create JS
//    });

//    // ✅ Load dropdowns & prefill fields
//    loadDropdownsAndPrefill()
//        .then(() => {
//            console.log("Dropdowns and fields prefilled.");
//        })
//        .catch(() => {
//            console.error("Error loading dropdowns in edit mode.");
//        });

//});
