document.addEventListener("DOMContentLoaded", function () {
    if (typeof window.isEditMode !== 'undefined' && window.isEditMode) {
        console.log("Insert JS skipped because it's edit mode");
        return;
    }

    // ✅ Only runs if not in edit mode
    console.log("Insert JS executing...");

    let allCategories = [];
    let allSubcategories = [];

    let resData = []; // ✅ add this line


    // Load data
    loadCategoriesAndSubcategories();

    // Event bindings
    $('#categorySelect').on('change', function () {
        const selectedCategoryId = $(this).val();
        populateSubcategoryDropdown(selectedCategoryId);
        toggleCreateButton();
    });

    $('#subCategorySelect').on('change', toggleCreateButton);

    $('#createBtn').on('click', function () {
        const supplyType = $('#supplyTypeSelect').val();
        if (!supplyType) {
            alert("Please select a Supply Type");
            return;
        }

        const isLT = supplyType === 'LT';
        $('#slabTableContainer').removeClass('d-none');
        generateTable(isLT);
    });

    $('#saveBtn').on('click', function (e) {
        e.preventDefault();
        saveSlabs();
    });

    $(document).on('click', '.addRow', function () {
        const isLT = $('#supplyTypeSelect').val() === 'LT';
        addNewRow(isLT);
    });

    $(document).on('click', '.deleteRow', function () {
        $(this).closest('tr').remove();
    });

    function loadCategoriesAndSubcategories() {
        $.get('/GetCategories', function (res) {
            if (res.success) {
                allCategories = [];
                allSubcategories = [];
                resData = res.data; // Store raw data for filtering by lines later

                res.data.forEach(item => {
                    if (!allCategories.some(cat => cat.categoryId === item.categoryId)) {
                        allCategories.push({
                            categoryId: item.categoryId,
                            categoryName: item.category,
                            lines: item.lines
                        });
                    }

                    allSubcategories.push({
                        categoryId: item.categoryId,
                        subCategoryId: item.subCategoryId,
                        subCategoryName: item.subCategoryName,
                        lines: item.lines
                    });
                });

                // Do NOT populate categories here, wait for supplyType change
            } else {
                alert('Failed to load categories');
            }
        });
    }
    function populateSubcategoryDropdown(categoryId) {
        const selectedLines = $('#supplyTypeSelect').val();
        const subCategorySelect = $('#subCategorySelect');

        subCategorySelect.empty().append('<option value="">Select Subcategory</option>');

        if (!categoryId || !selectedLines) return;

        const filteredSubcategories = allSubcategories.filter(sub =>
            sub.categoryId == categoryId && sub.lines === selectedLines
        );

        filteredSubcategories.forEach(sub => {
            subCategorySelect.append(`<option value="${sub.subCategoryId}">${sub.subCategoryName}</option>`);
        });
    }

    function toggleCreateButton() {
        const catSelected = $('#categorySelect').val();
        const subCatSelected = $('#subCategorySelect').val();
        $('#createBtn').prop('disabled', !(catSelected && subCatSelected));
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

    function saveSlabs() {
        const isCreatingNewCategory = $('#newCategoryInput').is(':visible') && $('#newCategoryInput').val().trim() !== "";
        const isCreatingNewSubCategory = $('#newSubCategoryInput').is(':visible') && $('#newSubCategoryInput').val().trim() !== "";

        const selectedCategoryId = parseInt($('#categorySelect').val());
        const selectedSubCategoryId = parseInt($('#subCategorySelect').val());
        const supplyType = $('#supplyTypeSelect').val();

        if (!supplyType) {
            alert("Please select a Supply Type");
            return;
        }

        const categoryName = isCreatingNewCategory
            ? $('#newCategoryInput').val().trim()
            : $("#categorySelect option:selected").text();

        if (!categoryName) {
            alert("Please select or enter a category.");
            return;
        }


        const subCategoryName = isCreatingNewSubCategory
            ? $('#newSubCategoryInput').val().trim()
            : $("#subCategorySelect option:selected").text();

        if (!subCategoryName) {
            alert("Please select or enter a subcategory.");
            return;
        }

        const slabs = [];
        let isValid = true;

        $('#slabTable tbody tr').each(function (index) {
            const $row = $(this);
            const fromVal = parseInt($row.find('.fromInput').val());
            const toVal = parseInt($row.find('.toInput').val());
            const fixedCharges = parseFloat($row.find('.fixedChargesInput').val());
            const rate = parseFloat($row.find('.rateInput').val());

            if (isNaN(fromVal) || isNaN(toVal) || fromVal < 0 || toVal <= fromVal) {
                alert(`Invalid slab range in row ${index + 1}: 'To' must be greater than 'From'.`);
                isValid = false;
                return false; // break out of loop
            }

            if (isNaN(fixedCharges) || fixedCharges < 0) {
                alert(`Invalid fixed charges in row ${index + 1}`);
                isValid = false;
                return false;
            }

            if (isNaN(rate) || rate < 0) {
                alert(`Invalid rate in row ${index + 1}`);
                isValid = false;
                return false;
            }

            slabs.push({
                SlabId: 0,
                Category: categoryName,
                SubCategory: subCategoryName,
                From: fromVal,
                To: toVal,
                FixedCharges: fixedCharges,
                Rate: rate,
                Lines: supplyType,
                BillingUnits: $row.find('.billingUnitsInput').val(),
                _11KV: parseFloat($row.find('.kv11Input').val()) || 0,
                _33KV: parseFloat($row.find('.kv33Input').val()) || 0,
                _132KV: parseFloat($row.find('.kv132Input').val()) || 0,
                _220KV: parseFloat($row.find('.kv220Input').val()) || 0,
                CustomerCharges: parseFloat($row.find('.customerChargesInput').val()) || 0,
                ElectricityDutyCharges: parseFloat($row.find('.electricityDutyChargesInput').val()) || 0,
                MinimumCharges: parseFloat($row.find('.minimumChargesInput').val()) || 0,
            });
        });

        if (!isValid) {
            return; // Don't submit
        }

        if (slabs.length === 0) {
            alert("Please enter at least one slab.");
            return;
        }

        // Check for overlapping slabs
        for (let i = 0; i < slabs.length - 1; i++) {
            for (let j = i + 1; j < slabs.length; j++) {
                if (!(slabs[i].To <= slabs[j].From || slabs[j].To <= slabs[i].From)) {
                    alert(`Slab ${i + 1} overlaps with slab ${j + 1}`);
                    return;
                }
            }
        }


        const model = {
            CategoryId: isCreatingNewCategory ? null : selectedCategoryId,
            SubCategoryId: isCreatingNewSubCategory ? null : selectedSubCategoryId,
            Category: categoryName,
            CategoryName: categoryName,
            SubCategory: subCategoryName,
            SubCategoryName: subCategoryName,
            Lines: supplyType,
            SubCategoriesDetails: slabs
        };

        const token = $('#antiForgeryForm input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/SlabsAndTariffs/CreateSlabs',
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            contentType: 'application/json',
            data: JSON.stringify(model),
            success: function (res) {
                if (res.success) {
                    $('#successToast').toast('show');
                    setTimeout(() => window.location.href = res.redirectUrl, 1500);
                } else {
                    alert(res.message);
                }
            },
            error: function () {
                alert("Something went wrong while saving.");
            }
        });
    }


    $('#supplyTypeSelect').on('change', function () {
        const selectedLines = $(this).val();

        if (!selectedLines) {
            $('#categorySelect').empty().append('<option value="">Select Category</option>');
            $('#subCategorySelect').empty().append('<option value="">Select Subcategory</option>');
            return;
        }

        // Populate categories based on selected supply type
        const filteredCategories = allCategories.filter(cat =>
            resData.some(item => item.categoryId === cat.categoryId && item.lines === selectedLines)
        );

        const categorySelect = $('#categorySelect');
        categorySelect.empty().append('<option value="">Select Category</option>');
        filteredCategories.forEach(cat => {
            categorySelect.append(`<option value="${cat.categoryId}">${cat.categoryName}</option>`);
        });

        // Clear subcategory until category is selected
        $('#subCategorySelect').empty().append('<option value="">Select Subcategory</option>');
    });

});

