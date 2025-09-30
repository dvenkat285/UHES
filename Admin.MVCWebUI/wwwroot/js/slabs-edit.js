$(document).ready(function () {
    $('#btnUpdateSlab').on('click', function () {
        submitSlabUpdate();
    });
});

function submitSlabUpdate() {
    const model = buildUpdateModel();

    if (!validateModel(model)) {
        alert("Please complete all required fields.");
        return;
    }

    $.ajax({
        url: '/SlabsAndTariffs/SlabsUpdate',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (response) {
            if (response.success) {
                alert(response.message);
                window.location.href = response.redirectUrl;
            } else {
                alert("Error: " + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX error:", error);
            alert("An error occurred while updating the slab.");
        }
    });
}
function buildUpdateModel() {
    const lines = $('#Lines').val();

    return {
        CategoryId: parseInt($('#CategoryId').val()) || null,
        CategoryName: $('#Category').val(),
        Category: $('#Category').val(),
        Subcategories: [
            {
                SubCategoryId: parseInt($('#SubCategoryId').val()) || null,
                SubCategoryName: $('#SubCategory').val(),
                SubCategory: $('#SubCategory').val(),
                SubCategoryNumber: null,  // Add if you have a source
                Lines: lines,
                BillingUnits: null,  // Add if you have input
                FixedCharges: parseFloat($('#FixedCharges').val()) || null,
                CustomerCharges: null,  // Add if you have input
                KV11: lines === 'HT' ? parseFloat($('#Kv11').val()) || null : null,
                KV33: lines === 'HT' ? parseFloat($('#Kv33').val()) || null : null,
                KV132: lines === 'HT' ? parseFloat($('#Kv132').val()) || null : null,
                KV220: lines === 'HT' ? parseFloat($('#Kv220').val()) || null : null,
                ElectricityDutyCharges: null,  // Add if you have input
                MinimumCharges: null,  // Add if you have input
                Slabs: [
                    {
                        SlabTariffId: parseInt($('#SlabTariffId').val()) || null,
                        From: parseInt($('#FromUnits').val()) || null,
                        To: parseInt($('#ToUnits').val()) || null,
                        UnitParameter: null,  // Add if you have input
                        EnergyCharges: parseFloat($('#EnergyCharges').val()) || null
                    }
                ]
            }
        ]
    };
}

function validateModel(model) {
    if (!model.Category || !model.CategoryId) return false;

    const sub = model.Subcategories[0];
    if (!sub.SubCategory || !sub.SubCategoryId || !sub.Lines) return false;

    const slab = sub.Slabs[0];
    if (slab.From < 0 || slab.To < 0 || slab.Rate < 0) return false;

    return true;
}
